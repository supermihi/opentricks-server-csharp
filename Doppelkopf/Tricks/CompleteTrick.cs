namespace Doppelkopf.Tricks;

public sealed record CompleteTrick(Trick Trick, Player Winner)
{
  public static CompleteTrick FromTrick(Trick trick, TrickContext context)
  {
    {
      var winner = trick.Winner(context);
      return new(trick, winner);
    }
  }
}
