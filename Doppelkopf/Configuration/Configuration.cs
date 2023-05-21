using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public sealed class Configuration : IConfiguration {
  public SessionConfiguration Rounds { get; }
  public IReadOnlyList<Card> Deck { get; }
  public GameModes Modes { get; }

  public Configuration(EldersMode elders, SessionConfiguration rounds, IImmutableList<Card> deck) {
    Rounds = rounds;
    Deck = deck;
    Modes = new(
        new NormalGameMode(elders),
        new MarriageGameMode(elders),
        new JackSolo(elders),
        new QueenSolo(elders),
        new SuitSolo(Suit.Diamonds, elders),
        new SuitSolo(Suit.Hearts, elders),
        new SuitSolo(Suit.Spades, elders),
        new SuitSolo(Suit.Clubs, elders),
        new MeatFree()
    );
  }

  public static readonly Configuration DDKV =
      new(EldersMode.FirstWins, SessionConfiguration.DDKV, Decks.WithNines);
  public static readonly Configuration Minikopf =
      new(
          EldersMode.FirstWinsExceptInLastTrick,
          new(NumberOfGames: 6, CompulsorySolos: true),
          ImmutableArray.Create<Card>(
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
          )
      );
}