using System.Collections.Immutable;
using Doppelkopf.Contracts;
using Doppelkopf.Utils;

namespace Doppelkopf.Games;

public sealed record Auction(InTurns<bool> Reservations,
  ImmutableStack<(Player player, IContract contract)> Declarations)
{
  public static readonly Auction Initial =
      new(new InTurns<bool>(Player.Player1), ImmutableStack<(Player, IContract)>.Empty);

  public (Auction, AuctionResult?) Reserve(Player player, bool reserved, AuctionContext context)
  {
    if (Reservations.Next is null)
    {
      throw Errors.Generic.InvalidPhase;
    }
    if (Reservations.Next != player)
    {
      throw Errors.Generic.OtherPlayersTurn(Reservations.Next.Value);
    }
    var nextAuction = this with { Reservations = Reservations.Add(reserved) };
    var resultOrNull = nextAuction.Evaluate(context);
    return (nextAuction, resultOrNull);
  }

  public (Auction, AuctionResult?) Declare(Player player,
    IContract contract,
    AuctionContext context)
  {
    if (!Reservations.IsFull)
    {
      throw Errors.Generic.InvalidPhase;
    }
    var reservation = Reservations[player];
    if (!reservation)
    {
      throw Errors.Auction.NoReservation;
    }
    var turn = ReservedPlayers.FirstOrDefault(HasDeclared);
    if (turn != player)
    {
      throw Errors.Generic.OtherPlayersTurn(turn);
    }
    if (!contract.CanDeclare(context.Cards, player))
    {
      throw Errors.Auction.CannotDeclareContract;
    }
    if (!context.Contracts.Contains(contract))
    {
      throw new ArgumentException("invalid contract not contained in game configuration");
    }
    var newAuction = this with { Declarations = Declarations.Push((player, contract)) };
    var resultOrNull = Evaluate(context);
    return (newAuction, resultOrNull);
  }

  private record struct DeclarationEval(Player Player, IContract Contract, int Value);

  private IEnumerable<Player> ReservedPlayers => Reservations.Players.Where(reserved => Reservations[reserved]);
  private bool HasDeclared(Player p) => Declarations.Any(d => d.player == p);

  private AuctionResult? Evaluate(AuctionContext context)
  {
    if (!Reservations.IsFull)
    {
      return null;
    }
    if (!Reservations.Any(r => r))
    {
      var contract = context.Contracts.NormalGame;
      var parties = contract.GetPartyData(context.Cards);
      return new(contract, parties);
    }
    var reservedPlayers = ReservedPlayers.ToArray();
    var maxValue = reservedPlayers.Any(p => context.NeedsCompulsorySolo[p])
        ? CompulsorySoloValue
        : VoluntarySoloValue;

    var evalByPlayer = Declarations.Select(
          declaration =>
          {
            var (player, contract) = declaration;
            var needsCompulsorySolo = context.NeedsCompulsorySolo[player];
            var value = DeclarationValue(contract, needsCompulsorySolo);
            return new DeclarationEval(player, contract, value);
          })
        .ToArray();
    if (evalByPlayer.Length == reservedPlayers.Length || evalByPlayer.Max(t => t.Value) >= maxValue)
    {
      var (winner, contract, _) = evalByPlayer.MaxBy(t => t.Value);
      var parties = contract.GetPartyData(context.Cards, winner);
      return new(contract, parties);
    }
    return null;
  }

  private const int NormalGameValue = 0;
  private const int SpecialGameValue = 1;
  private const int MarriageValue = 2;
  private const int VoluntarySoloValue = 3;
  private const int CompulsorySoloValue = 4;

  private static int DeclarationValue(IContract declaration, bool needsCompulsorySolo) =>
      declaration.Type switch
      {
        ContractType.NormalGame => NormalGameValue,
        ContractType.Special => SpecialGameValue,
        ContractType.Marriage => MarriageValue,
        ContractType.Solo when !needsCompulsorySolo => VoluntarySoloValue,
        ContractType.Solo when needsCompulsorySolo => CompulsorySoloValue
      };
}
