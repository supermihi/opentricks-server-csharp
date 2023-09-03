namespace Doppelkopf.Core.Scoring;

public readonly record struct ExtraPointKind(string Id)
{
  public static readonly ExtraPointKind CaughtTheFox = new("caught_the_fox");
  public static readonly ExtraPointKind AgaintsTheElders = new("against_the_elders");
  public static readonly ExtraPointKind CharlieMiller = new("charlie_miller");
  public static readonly ExtraPointKind Doppelkopf = new("doppelkopf");
}
