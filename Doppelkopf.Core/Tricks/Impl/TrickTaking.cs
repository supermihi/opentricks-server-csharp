using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Tricks.Impl;

internal class TrickTaking : ITrickTakingProgress
{
  private readonly ICardTraitsProvider _cardTraitsProvider;
  private TrickTakingState _state;
  public Trick? CurrentTrick => _state.CurrentTrick;
  public IReadOnlyList<CompleteTrick> CompleteTricks => _state.CompleteTricks;
  public CardsByPlayer RemainingCards => _state.RemainingCards;
  public Player? Turn => CurrentTrick?.Cards.Next;

  public TrickTaking(ICardTraitsProvider cardTraitsProvider, TrickTakingState state)
  {
    _cardTraitsProvider = cardTraitsProvider;
    _state = state;
  }

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
    CheckIsAllowedCard(_state.RemainingCards[player], card);

    var remainingCards = _state.RemainingCards.Remove(player, card);
    var updatedCurrentTrick = CurrentTrick.AddCard(card);
    if (updatedCurrentTrick.Cards.IsFull)
    {
      var completedTrick = updatedCurrentTrick.Complete(_cardTraitsProvider);
      _state = new TrickTakingState(
        remainingCards,
        CurrentTrick: null,
        CompleteTricks: _state.CompleteTricks.Add(completedTrick));
      return completedTrick;
    }

    _state = _state with { RemainingCards = remainingCards, CurrentTrick = updatedCurrentTrick };
    return null;
  }

  public bool TryStartNextTrick()
  {
    if (CurrentTrick != null)
    {
      throw new InvalidOperationException("valid only after a trick has been completed");
    }

    var previousTrick = _state.CompleteTricks[^1];
    if (previousTrick.Remaining == 0)
    {
      return false;
    }

    var nextTrick = new Trick(previousTrick.Winner, previousTrick.Index + 1, previousTrick.Remaining - 1);
    _state = _state with { CurrentTrick = nextTrick };
    return true;
  }

  private void CheckPlayerHasCard(Player player, Card card)
  {
    if (!_state.RemainingCards[player].Contains(card))
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
    _cardTraitsProvider.GetTraits(card).TrickSuit
    == _cardTraitsProvider.GetTraits(CurrentTrick!.Cards.First()).TrickSuit;
}
