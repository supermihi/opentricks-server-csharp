using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;

namespace Doppelkopf.Conf;

public sealed record Configuration(SessionConfiguration Session,
  IReadOnlyList<Card> Deck,
  AvailableContracts Contracts,
  TrickConfiguration Tricks)
{
  public static Configuration Default(EldersMode elders,
    SessionConfiguration rounds,
    IImmutableList<Card> deck)
  {
    return new(rounds, deck, AvailableContracts.Default, new(elders));
  }

  public static readonly Configuration DDKV = Default(
    EldersMode.FirstWins,
    SessionConfiguration.DDKV,
    Decks.WithNines
  );
  public static readonly Configuration Minikopf = Default(
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
