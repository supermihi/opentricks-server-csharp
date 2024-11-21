namespace Doppelkopf.Core.Scoring.Impl;

public static class ExtraPoints
{
  public static readonly IReadOnlyList<IExtraScoreRule> Default = [new CharlieMiller(), new Doppelkopf(), new CaughtTheFox()];
}
