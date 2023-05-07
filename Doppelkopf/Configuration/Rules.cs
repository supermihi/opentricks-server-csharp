using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public sealed class Rules : IRules
{
  public RoundConfiguration Rounds { get; }
  public IImmutableList<Card> Deck { get; }
  public GameModeCollection Modes { get; }

  public Rules(EldersMode elders, RoundConfiguration rounds, IImmutableList<Card> deck)
  {
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

  public static readonly Rules DDKV =
    new(EldersMode.FirstWins, new(NumberOfGames: 20, CompulsorySolos: true), Decks.WithNines);
  public static readonly Rules Minikopf =
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
