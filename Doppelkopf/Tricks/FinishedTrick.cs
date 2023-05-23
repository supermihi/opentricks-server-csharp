namespace Doppelkopf.Tricks;

public sealed record FinishedTrick(Trick Trick, Player Winner)
{
    public static FinishedTrick FromTrick(Trick trick, TrickContext context)
    {
        {
            var winner = trick.Winner(context);
            return new(trick, winner);
        }
    }
}
