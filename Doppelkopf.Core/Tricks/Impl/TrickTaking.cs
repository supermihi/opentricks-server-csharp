using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Tricks.Impl;

internal class TrickTaking(ICardTraitsProvider cardTraitsProvider, TrickTakingState state) : ITrickTakingProgress
{
  public Trick? CurrentTrick => state.CurrentTrick;
  public IReadOnlyList<CompleteTrick> CompleteTricks => state.CompleteTricks;
  public CardsByPlayer RemainingCards => state.RemainingCards;
  public Player? Turn => CurrentTrick?.Cards.Next;

  public TrickTaking(ICardTraitsProvider cardTraitsProvider, CardsByPlayer givenCards)
    : this(cardTraitsProvider, TrickTakingState.Initial(givenCards))
  {
  }

  /// <summary>
  ///
  /// </summary>
  /// <param name="player"></param>
  /// <param name="card"></param>
  /// <returns>If the card finishes a trick, that one is returned; <c>null</c> otherwise.</returns>
  public CompleteTrick? PlayCard(Player player, Card card)
  {
    if (CurrentTrick is null)
    {
      ErrorCodes.InvalidPhase.Throw();
    }

    CurrentTrick.Cards.CheckIsTurn(player);
    CheckPlayerHasCard(player, card);
    CheckIsAllowedCard(state.RemainingCards[player], card);

    var remainingCards = state.RemainingCards.Remove(player, card);
    var updatedCurrentTrick = CurrentTrick.AddCard(card);
    if (updatedCurrentTrick.Cards.IsFull)
    {
      var completedTrick = updatedCurrentTrick.Complete(cardTraitsProvider);
      state = new TrickTakingState(
        remainingCards,
        CurrentTrick: null,
        CompleteTricks: state.CompleteTricks.Add(completedTrick));
      return completedTrick;
    }

    state = state with { RemainingCards = remainingCards, CurrentTrick = updatedCurrentTrick };
    return null;
  }

  public bool TryStartNextTrick()
  {
    if (CurrentTrick != null)
    {
      throw new InvalidOperationException("valid only after a trick has been completed");
    }

    var previousTrick = state.CompleteTricks[^1];
    if (previousTrick.Remaining == 0)
    {
      return false;
    }

    var nextTrick = new Trick(previousTrick.Winner, previousTrick.Index + 1, previousTrick.Remaining - 1);
    state = state with { CurrentTrick = nextTrick };
    return true;
  }

  private void CheckPlayerHasCard(Player player, Card card)
  {
    if (!state.RemainingCards[player].Contains(card))
    {
      ErrorCodes.CardNotOwned.Throw();
    }
  }

  private void CheckIsAllowedCard(IEnumerable<Card> playerCards, Card card)
  {
    if (CurrentTrick!.Cards.Count == 0)
    {
      return;
    }

    if (!FollowsSuit(card) && playerCards.Any(FollowsSuit))
    {
      ErrorCodes.CardNotAllowed.Throw();
    }
  }

  private bool FollowsSuit(Card card) =>
    cardTraitsProvider.Get(card).TrickSuit
    == cardTraitsProvider.Get(CurrentTrick!.Cards.First()).TrickSuit;
}
