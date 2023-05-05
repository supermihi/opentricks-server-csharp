using Doppelkopf.Configuration;

namespace Doppelkopf.GameFinding;

public record Auction(IRules Rules, InTurns<bool> Reservations, ByPlayer<IGameMode?> Declarations)
{
  public static Auction Initial(IRules rules) =>
    new(rules, new InTurns<bool>(Player.Player1), ByPlayer.Init<IGameMode?>(null));
}

public enum DeclarationType
{
  Marriage,
  Solo
}
