using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Tricks.Impl;

internal class TrickTaking : ITrickTakingInteractor, ITrickTakingProgress
{
  private readonly ITrickEvaluator _trickEvaluator;
  private readonly ITrickSuitProvider _trickSuitProvider;
  private TrickTakingState _state;
  public Trick CurrentTrick => _state.CurrentTrick;
  public IReadOnlyList<ITrick> Tricks => _state.Tricks;
  public ICardsByPlayer RemainingCards => _state.RemainingCards;

  public TrickTaking(ITrickEvaluator trickEvaluator, ITrickSuitProvider trickSuitProvider, TrickTakingState state)
  {
    _trickEvaluator = trickEvaluator;
    _trickSuitProvider = trickSuitProvider;
    _state = state;
  }

  public void PlayCard(Player player, Card card)
  {
    if (CurrentTrick.IsComplete)
    {
      ErrorCodes.InvalidPhase.Throw();
    }
    CurrentTrick.Cards.CheckIsTurn(player);
    CheckPlayerHasCard(player, card);
    CheckIsAllowedCard(_state.RemainingCards[player], card);
    var updatedCurrentTrick = CurrentTrick.AddCard(card);
    var remainingCards = _state.RemainingCards.Remove(player, card);
    _state = new TrickTakingState(remainingCards, _state.Tricks[..^1].Add(updatedCurrentTrick))
        .UpdateCurrentAndStartNextIfNeeded(_trickEvaluator);
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
    if (CurrentTrick.Cards.Count == 0)
    {
      return;
    }
    if (!FollowsSuit(card) && playerCards.Any(FollowsSuit))
    {
      ErrorCodes.CardNotAllowed.Throw();
    }
  }

  private bool FollowsSuit(Card card) =>
      _trickSuitProvider.GetTrickSuit(card) == _trickSuitProvider.GetTrickSuit(CurrentTrick!.Cards.First());
}
