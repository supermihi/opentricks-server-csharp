using System.Collections.Immutable;

namespace Doppelkopf.Core.Scoring.Impl;

public static class ExtraPoints
{
  public static readonly IReadOnlyList<IExtraPointRule> Default = ImmutableList.Create<IExtraPointRule>(
    new CharlieMiller(),
    new Doppelkopf(),
    new CaughtTheFox());
}
