using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public sealed record Rules(RuleSet RuleSet, GameModeCollection Modes) : IRules
{
  public static readonly Rules DDKV =
    new(
      RuleSet.DDKV,
      new(
        new NormalGameMode(EldersMode.FirstWins),
        new MarriageGameMode(EldersMode.FirstWins),
        new JackSolo(EldersMode.FirstWins),
        new QueenSolo(EldersMode.FirstWins),
        new SuitSolo(Suit.Diamonds, EldersMode.FirstWins),
        new SuitSolo(Suit.Hearts, EldersMode.FirstWins),
        new SuitSolo(Suit.Spades, EldersMode.FirstWins),
        new SuitSolo(Suit.Clubs, EldersMode.FirstWins),
        new MeatFree()
      )
    );
}
