using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Sessions;
using Doppelkopf.Tricks;

namespace Doppelkopf;

public sealed record Configuration(SessionConfiguration Session,
  ICardProvider Cards,
  AvailableContracts Contracts,
  TrickConfiguration Tricks)
{
  public static Configuration Default(EldersMode elders,
    SessionConfiguration rounds,
    ICardProvider cards)
  {
    return new(rounds, cards, AvailableContracts.Default, new(elders));
  }

  public static readonly Configuration DDKV = Default(
    EldersMode.FirstWins,
    SessionConfiguration.DDKV,
    new RandomCardProvider(Decks.WithNines)
  );
  public static readonly Configuration Minikopf = Default(
    EldersMode.FirstWinsExceptInLastTrick,
    new(NumberOfGames: 6, CompulsorySolos: true),
    new RandomCardProvider(
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
      ))
  );
}
