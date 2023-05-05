using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public sealed record Rules(RuleSet RuleSet, GameModeCollection Modes) : IRules
{

  public static readonly Rules DDKV =
    new(
      RuleSet.DDKV,
      new(
        new NormalGameMode(),
        new MarriageGameMode(),
        new IGameMode[]
        {
          new JackSolo(),
          new QueenSolo(),
          new SuitSolo(Suit.Diamonds),
          new SuitSolo(Suit.Hearts),
          new SuitSolo(Suit.Spades),
          new SuitSolo(Suit.Clubs),
          new MeatFree()
        },
        Enumerable.Empty<IGameMode>()
      )
    );
}
