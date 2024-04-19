using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Cards.Impl;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;

namespace Doppelkopf.Core;

public sealed record GameConfiguration(
  RuleOptions Rules,
  IReadOnlyCollection<Card> Deck,
  Modes GameModes,
  IEvaluator Evaluator)
{
  public static GameConfiguration Default(RuleOptions? rules = null)
  {
    rules ??= RuleOptions.Default;
    var modes = Modes.Default(rules);
    return new GameConfiguration(rules, Decks.WithNines, modes, new DdkvEvaluator());
  }

  public IGameFactory CreateGameFactory(int? seed)
  {
    var dealer = new PseudoRandomDealer(Deck, seed);
    return new GameFactory(this, dealer);
  }
}
