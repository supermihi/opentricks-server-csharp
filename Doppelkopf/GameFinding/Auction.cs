using Doppelkopf.Configuration;
using Doppelkopf.Errors;

namespace Doppelkopf.GameFinding;

public record Auction(InTurns<bool> Reservations, ByPlayer<IGameMode?> Declarations)
{
  public static readonly Auction Initial =
    new(new InTurns<bool>(Player.Player1), ByPlayer.Init<IGameMode?>(null));

  public (Auction, Contract?) Reserve(Player player, bool reserved, AuctionContext context)
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
    var contractOrNull = nextAuction.Evaluate(context);
    return (nextAuction, contractOrNull);
  }

  public (Auction, Contract?) Declare(Player player, IGameMode mode, AuctionContext context)
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
    if (mode.Kind == GameModeKind.NormalGame)
    {
      throw Err.Auction.Declare.NormalGame;
    }
    if (!context.Modes.Contains(mode))
    {
      throw Err.Auction.Declare.InvalidGameMode;
    }
    var newAuction = this with { Declarations = Declarations.Replace(player, mode) };
    var contractOrNull = Evaluate(context);
    return (newAuction, contractOrNull);
  }

  private record struct DeclarationEval(Player Player, IGameMode Mode, int Value);

  private Contract? Evaluate(AuctionContext context)
  {
    if (!Reservations.IsFull)
    {
      return null;
    }
    if (!Reservations.Any(r => r))
    {
      return new(context.Modes.NormalGame, null);
    }
    var reservedPlayers = Reservations.Items().Where(t => t.value).Select(t => t.player).ToArray();
    var maxValue = reservedPlayers.Any(p => context.NeedsCompulsorySolo[p])
      ? CompulsorySoloValue
      : VoluntarySoloValue;

    var valuesByPlayer = reservedPlayers
      .Where(player => Declarations[player] != null)
      .Select(player =>
      {
        var mode = Declarations[player]!;
        var value = DeclarationValue(mode, context.NeedsCompulsorySolo[player]);
        return new DeclarationEval(player, mode, value);
      })
      .ToArray();
    if (
      valuesByPlayer.Length == reservedPlayers.Length
      || valuesByPlayer.Max(t => t.Value) >= maxValue
    )
    {
      var (winner, mode, _) = valuesByPlayer.MaxBy(t => t.Value);
      return new Contract(mode, winner);
    }
    return null;
  }

  private const int NormalGameValue = 0;
  private const int SpecialGameValue = 1;
  private const int MarriageValue = 2;
  private const int VoluntarySoloValue = 3;
  private const int CompulsorySoloValue = 4;

  private static int DeclarationValue(IGameMode declaration, bool needsCompulsorySolo) =>
    declaration.Kind switch
    {
      GameModeKind.NormalGame => NormalGameValue,
      GameModeKind.Special => SpecialGameValue,
      GameModeKind.Marriage => MarriageValue,
      GameModeKind.Solo when !needsCompulsorySolo => VoluntarySoloValue,
      GameModeKind.Solo when needsCompulsorySolo => CompulsorySoloValue
    };
}

public sealed record AuctionContext(ByPlayer<bool> NeedsCompulsorySolo, GameModeCollection Modes);
