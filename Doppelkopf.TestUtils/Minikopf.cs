using System.Collections.Immutable;
using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.TestUtils;

public static class Configurations
{
  private static GameConfiguration CreateMinikopfConfiguration()
  {
    var rules = new RuleOptions(TieBreakingMode.SecondWinsInLastTrick, 6, true);
    var deck = ImmutableArray.Create<Card>(
      new(Suit.Hearts, Rank.Ten),
      new(Suit.Hearts, Rank.Ten),
      new(Suit.Clubs, Rank.Jack),
      new(Suit.Clubs, Rank.Queen),
      new(Suit.Clubs, Rank.Queen),
      new(Suit.Diamonds, Rank.Ace),
      new(Suit.Diamonds, Rank.Ace),
      new(Suit.Spades, Rank.Ace),
      new(Suit.Hearts, Rank.Ace),
      new(Suit.Hearts, Rank.Ace),
      new(Suit.Hearts, Rank.King),
      new(Suit.Hearts, Rank.Nine)
    );
    return new GameConfiguration(rules, deck, Modes.Default(rules), new DdkvEvaluator());
  }

  public static readonly GameConfiguration Minikopf = CreateMinikopfConfiguration();
}
