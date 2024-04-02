using System.Collections.Immutable;

namespace Doppelkopf.Core.Scoring.Impl;

public static class ExtraPoints
{
  public static readonly IReadOnlyList<IExtraScoreRule> Default = ImmutableList.Create<IExtraScoreRule>(
    new CharlieMiller(),
    new Doppelkopf(),
    new CaughtTheFox());
}
