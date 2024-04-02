using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Cards.Impl;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Contracts.Impl;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;

namespace Doppelkopf.Core;

public sealed class GameConfiguration : IGameConfiguration
{
  public RuleOptions Rules { get; }

  public GameConfiguration(RuleOptions rules, IEvaluator evaluator)
  {
    Rules = rules;
    var holds = new IHold[]
    {
      new Wedding(rules.HeartTenTieBreaking), Solo.Fleshless, Solo.JackSolo, Solo.QueenSolo,
      Solo.SuitSolo(Suit.Diamonds, rules.HeartTenTieBreaking), Solo.SuitSolo(Suit.Hearts, rules.HeartTenTieBreaking),
      Solo.SuitSolo(Suit.Spades, rules.HeartTenTieBreaking), Solo.SuitSolo(Suit.Clubs, rules.HeartTenTieBreaking)
    };
    GameModes = new Modes(holds, new NormalGameProvider(rules.HeartTenTieBreaking, new DdkvEvaluator()));
    Evaluator = evaluator;
  }

  public Modes GameModes { get; }
  public IEvaluator Evaluator { get; }

  public IReadOnlyList<Card> Deck => Rules.Deck;

  public IGameFactory CreateGameFactory(int? seed)
  {
    var dealer = new PseudoRandomDealer(Deck, seed);
    return new GameFactory(GameModes, dealer);
  }
}
