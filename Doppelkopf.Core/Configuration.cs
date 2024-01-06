using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Cards.Impl;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Contracts.Impl;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core;

public interface IGameConfiguration
{
  IReadOnlyList<IDeclarableContract> DeclarableContracts { get; }
}
public sealed class GameConfiguration : IGameConfiguration
{

  public GameConfiguration(bool withNines, TieBreakingMode heartTenTieBreaking)
  {
    Deck = withNines ? Decks.WithNines : Decks.WithoutNines;
    DeclarableContracts = new IDeclarableContract[]
    {
      Solo.MeatFree,
      Solo.JackSolo,
      Solo.QueenSolo,
      Solo.SuitSolo(Suit.Diamonds, heartTenTieBreaking),
      Solo.SuitSolo(Suit.Hearts, heartTenTieBreaking),
      Solo.SuitSolo(Suit.Spades, heartTenTieBreaking),
      Solo.SuitSolo(Suit.Clubs, heartTenTieBreaking)
    };
  }

  public IReadOnlyList<IDeclarableContract> DeclarableContracts { get; }

  public IReadOnlyList<Card> Deck { get; }

  public IGameFactory CreateGameFactory(int? seed)
  {
    var contracts = CreateContracts();
    var dealer = new PseudoRandomDealer(Deck, seed);
    return new GameFactory(contracts, dealer);
  }
}
