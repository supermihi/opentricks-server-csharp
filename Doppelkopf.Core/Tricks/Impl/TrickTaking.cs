using Doppelkopf.Core.Auctions.Impl;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Tricks.Impl;

public class TrickTaking : ITrickTaking
{
  private readonly ICardTraitsProvider _cardTraitsProvider;
  private TrickTakingState _state;
  public InTurns<Card>? CurrentTrick => _state.CurrentTrick;

  public TrickTaking(ICardTraitsProvider cardTraitsProvider, TrickTakingState state)
  {
    _cardTraitsProvider = cardTraitsProvider;
    _state = state;
  }

  public void PlayCard(Player player, Card card)
  {
    if (CurrentTrick is null)
    {
      ErrorCodes.InvalidPhase.Throw();
    }
    CurrentTrick.CheckIsTurn(player);
    CheckPlayerHasCard(player, card);
    CheckIsAllowedCard(_state.RemainingCards[player], card);
    var currentTrickWithNewCard = CurrentTrick.Add(card);
    var remainingCards = _state.RemainingCards.Replace(
      player,
      _state.RemainingCards[player].Remove(card, EqualityComparer<Card>.Default));
    if (currentTrickWithNewCard.IsFull)
    {
      var completedTrick = _cardTraitsProvider.Complete(CurrentTrick, IsLastTrick);
      var nextTrick = IsLastTrick ? null : new InTurns<Card>(completedTrick.Winner);
      _state = new TrickTakingState(
        CompletedTricks: _state.CompletedTricks.Add(completedTrick),
        CurrentTrick: nextTrick,
        RemainingCards: remainingCards);
      return;
    }
    _state = _state with { CurrentTrick = currentTrickWithNewCard };
  }

  private bool IsLastTrick => _state.RemainingCards.Any(c => c.Any());

  private void CheckPlayerHasCard(Player player, Card card)
  {
    if (!_state.RemainingCards[player].Contains(card))
    {
      ErrorCodes.CardNotOwned.Throw();
    }
  }

  private void CheckIsAllowedCard(IEnumerable<Card> playerCards, Card card)
  {
    if (CurrentTrick!.Count == 0)
    {
      return;
    }
    if (!FollowsSuit(card) && playerCards.Any(FollowsSuit))
    {
      ErrorCodes.CardNotAllowed.Throw();
    }
  }

  private bool FollowsSuit(Card card) => _cardTraitsProvider.FollowsSuit(CurrentTrick!.First(), card);
}
