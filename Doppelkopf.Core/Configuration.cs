using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Cards.Impl;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Contracts.Impl;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core;

public sealed record GameConfiguration(TieBreakingMode HeartTenTieBreaking, bool WithNines)
{
  public AvailableContracts CreateContracts()
  {
    var marriage = new Marriage(HeartTenTieBreaking);
    var meatFree = Solo.MeatFree;
    var jackSolo = Solo.JackSolo;
    var queenSolo = Solo.QueenSolo;
    var diamondsSolo = Solo.SuitSolo(Suit.Diamonds, HeartTenTieBreaking);
    var heartsSolo = Solo.SuitSolo(Suit.Hearts, HeartTenTieBreaking);
    var spadesSolo = Solo.SuitSolo(Suit.Spades, HeartTenTieBreaking);
    var clubsSolo = Solo.SuitSolo(Suit.Clubs, HeartTenTieBreaking);
    return new(
      new NormalGame(HeartTenTieBreaking),
      marriage,
      meatFree,
      jackSolo,
      queenSolo,
      diamondsSolo,
      heartsSolo,
      spadesSolo,
      clubsSolo
    );
  }

  public IReadOnlyList<Card> Deck => WithNines ? Decks.WithNines : Decks.WithoutNines;

  public IGameFactory CreateGameFactory(int? seed)
  {
    var contracts = CreateContracts();
    var dealer = new PseudoRandomDealer(Deck, seed);
    return new GameFactory(contracts, dealer);
  }
}
