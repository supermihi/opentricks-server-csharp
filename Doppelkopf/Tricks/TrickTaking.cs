using System.Collections.Immutable;
using Doppelkopf.Actions;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;

namespace Doppelkopf.Tricks;

/// <summary>
/// The trick taking part of a Doppelkopf game.
/// </summary>
/// <param name="Rules"></param>
/// <param name="Contract"></param>
/// <param name="Cards"></param>
/// <param name="CompletedTricks"></param>
/// <param name="CurrentTrick"></param>
public sealed record TrickTaking(
  RuleSet Rules,
  Contract Contract,
  ByPlayer<IImmutableList<Card>> Cards,
  IImmutableList<FinishedTrick> CompletedTricks,
  Trick? CurrentTrick
)
{
  public static TrickTaking Initial(
    RuleSet rules,
    Contract contract,
    ByPlayer<IImmutableList<Card>> cards
  ) =>
    new(rules, contract, cards, ImmutableList<FinishedTrick>.Empty, Trick.Initial(Player.Player1));

  public bool IsFinished => CurrentTrick is null;

  public IEnumerable<(TrickTaking result, IEvent @event)> PlayCard(Player player, Card card)
  {
    var currentTrick = CheckIsPlayersTurn(player);
    CheckPlayerHasCard(player, card);
    CheckCardIsValid(player, card, currentTrick);
    var game2 = WithCardPlayed(player, card, currentTrick);
    yield return (game2, new PlayCardEvent(player, card));
    if (game2.CurrentTrick!.IsFull)
    {
      var game3 = game2.WithTrickCompleted();
      yield return (
        game2.WithTrickCompleted(),
        new FinishTrickEvent(game3.CompletedTricks.Last().Winner)
      );
    }
  }

  private TrickTaking WithTrickCompleted()
  {
    if (CurrentTrick is not { IsFull: true })
    {
      throw new IllegalStateException("cannot complete trick that is not full");
    }
    var currentTrickNumber = CompletedTricks.Count + 1;
    var isLastTrick = currentTrickNumber == Rules.NumberOfTricks;
    var context = new TrickContext(Contract.Mode.TrickRules, Rules.Elders, isLastTrick);
    var currentTrickFinished = CurrentTrick.Finish(context);
    return this with
    {
      CompletedTricks = CompletedTricks.Add(currentTrickFinished),
      CurrentTrick = isLastTrick ? null : Trick.Initial(currentTrickFinished.Winner)
    };
  }

  private TrickTaking WithCardPlayed(Player player, Card card, Trick currentTrick)
  {
    return this with
    {
      Cards = Cards.Replace(player, Cards[player].Remove(card)),
      CurrentTrick = currentTrick.Add(card)
    };
  }

  private void CheckCardIsValid(Player player, Card card, Trick currentTrick)
  {
    if (!currentTrick.IsValidNextCard(card, Contract.Mode.TrickRules, Cards[player]))
    {
      throw InputException.Game.PlayCard.ForbiddenCard;
    }
  }

  private Trick CheckIsPlayersTurn(Player player)
  {
    if (CurrentTrick is null)
    {
      throw InputException.Game.PlayCard.GameFinished;
    }
    var currentPlayer = CurrentTrick.Turn;
    if (currentPlayer is null)
    {
      throw new ArgumentException("no current player");
    }
    if (currentPlayer != player)
    {
      throw InputException.Game.PlayCard.NotYourTurn;
    }
    return CurrentTrick;
  }

  private void CheckPlayerHasCard(Player player, Card card)
  {
    if (!Cards[player].Contains(card))
    {
      throw InputException.Game.PlayCard.DoNotHaveCard;
    }
  }
}
