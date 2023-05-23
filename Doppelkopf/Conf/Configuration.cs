using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;

namespace Doppelkopf.Conf;

public sealed record Configuration(
  SessionConfiguration Session,
  IReadOnlyList<Card> Deck,
  AvailableContracts Contracts,
  TrickConfiguration Tricks
)
{
    public static Configuration Default(
      EldersMode elders,
      SessionConfiguration rounds,
      IImmutableList<Card> deck
    )
    {
        return new(
          rounds,
          deck,
          new AvailableContracts(
            new NormalGame("contract_normal_game"),
            new Marriage("contract_marriage"),
            new[]
            {
          new Solo("contract_diamonds_solo", CardTraitsProvider.DiamondsTrump),
          new Solo("contract_hearts_solo", CardTraitsProvider.HeartsTrump),
          new Solo("contract_spades_solo", CardTraitsProvider.SpadesTrump),
          new Solo("contract_clubs_solo", CardTraitsProvider.ClubsTrump),
          new Solo("contract_meatfree", CardTraitsProvider.NoTrump),
          new Solo("contract_jacks_solo", CardTraitsProvider.JacksTrump),
          new Solo("contract_queens_solo", CardTraitsProvider.QueensTrump)
            },
            new IContract[] { }
          ),
          new(elders)
        );
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
