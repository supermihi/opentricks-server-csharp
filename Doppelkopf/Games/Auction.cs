using Doppelkopf.Contracts;
using Doppelkopf.Errors;
using Doppelkopf.Utils;

namespace Doppelkopf.Games;

public sealed record Auction(InTurns<bool> Reservations, ByPlayer<IContract?> Declarations)
{
  public static readonly Auction Initial =
      new(new InTurns<bool>(Player.Player1), ByPlayer.Init((IContract?)null));

  public (Auction, AuctionResult?) Reserve(Player player, bool reserved, AuctionContext context)
  {
    if (Reservations.IsFull)
    {
      throw Err.Auction.Reserve.InvalidPhase;
    }
    if (Reservations.Next != player)
    {
      throw Err.Auction.Reserve.NotYourTurn;
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
      throw Err.Auction.Declare.InvalidPhase;
    }
    var reservation = Reservations[player];
    if (!reservation)
    {
      throw Err.Auction.Declare.NotReserved;
    }
    if (!contract.CanDeclare(context.Cards, player))
    {
      throw Err.Auction.Declare.NotAllowed;
    }
    if (!context.Contracts.Contains(contract))
    {
      throw Err.Auction.Declare.InvalidContract;
    }
    var newAuction = this with { Declarations = Declarations.Replace(player, contract) };
    var resultOrNull = Evaluate(context);
    return (newAuction, resultOrNull);
  }

  private record struct DeclarationEval(Player Player, IContract Contract, int Value);

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
    var reservedPlayers = Reservations.Players.Where(reserved => Reservations[reserved]).ToArray();
    var maxValue = reservedPlayers.Any(p => context.NeedsCompulsorySolo[p])
        ? CompulsorySoloValue
        : VoluntarySoloValue;

    var evalByPlayer = reservedPlayers
        .Where(player => Declarations[player] != null)
        .Select(
          player =>
          {
            var contract = Declarations[player]!;
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
