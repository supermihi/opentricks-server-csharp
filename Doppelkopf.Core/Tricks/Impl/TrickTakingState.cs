using System.Collections.Immutable;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks.Impl;

internal sealed record TrickTakingState(CardsByPlayer RemainingCards, ImmutableArray<Trick> Tricks)
{
  public static TrickTakingState Initial(CardsByPlayer initialCards) =>
      new(
        initialCards,
        ImmutableArray.Create(new Trick(new InTurns<Card>(Player.One), null))
      );

  public bool CurrentTrickIsLast()
  {
    var remainingCards = RemainingCards[Player.One].Length;
    return remainingCards == 0 || (remainingCards == 1 && !Tricks[^1].Cards.Contains(Player.One));
  }

  public Trick CurrentTrick => Tricks[^1];
}
