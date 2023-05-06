using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Errors;

namespace Doppelkopf.Tricks;

/// <summary>
/// The trick taking part of a Doppelkopf game.
/// </summary>
public sealed record TrickTaking(
  Contract Contract,
  ByPlayer<IImmutableList<Card>> Cards,
  IImmutableList<FinishedTrick> CompletedTricks,
  Trick? CurrentTrick
)
{
  public static TrickTaking Initial(Contract contract, ByPlayer<IImmutableList<Card>> cards) =>
    new(contract, cards, ImmutableList<FinishedTrick>.Empty, new(Player.Player1));

  public bool IsFinished => CurrentTrick is null;

  public (TrickTaking result, bool finishedTrick) PlayCard(Player player, Card card)
  {
    var currentTrick = CheckIsPlayersTurn(player);
    CheckPlayerHasCard(player, card);
    CheckCardIsValid(player, card, currentTrick);
    var gameWithCardPlayed = WithCardPlayed(player, card, currentTrick);
    return gameWithCardPlayed.CurrentTrick!.IsFull
      ? (gameWithCardPlayed.WithTrickCompleted(), true)
      : (gameWithCardPlayed, false);
  }

  private TrickTaking WithTrickCompleted()
  {
    if (CurrentTrick is not { IsFull: true })
    {
      throw new IllegalStateException("cannot complete trick that is not full");
    }
    var tricksLeft = Cards.Player1.Count;
    var currentTrickIsLast = tricksLeft == 0;
    var context = new TrickContext(Contract.Mode.TrickRules, currentTrickIsLast);
    var currentTrickFinished = FinishedTrick.FromTrick(CurrentTrick, context);
    return this with
    {
      CompletedTricks = CompletedTricks.Add(currentTrickFinished),
      CurrentTrick = currentTrickIsLast ? null : new(currentTrickFinished.Winner)
    };
  }

  private TrickTaking WithCardPlayed(Player player, Card card, Trick currentTrick)
  {
    var playersCardsWithoutJustPlayed = Cards.Replace(player, Cards[player].Remove(card));
    var trickWithJustPlayed = currentTrick.AddCard(card);

    return this with
    {
      Cards = playersCardsWithoutJustPlayed,
      CurrentTrick = trickWithJustPlayed,
    };
  }

  private void CheckCardIsValid(Player player, Card card, Trick currentTrick)
  {
    if (!currentTrick.IsValidNextCard(card, Contract.Mode.TrickRules, Cards[player]))
    {
      throw Err.Game.PlayCard.Forbidden;
    }
  }

  private Trick CheckIsPlayersTurn(Player player)
  {
    if (CurrentTrick is null)
    {
      throw Err.Game.PlayCard.InvalidPhase;
    }
    var currentPlayer = CurrentTrick.Turn;
    if (currentPlayer is null)
    {
      throw new ArgumentException("no current player");
    }
    if (currentPlayer != player)
    {
      throw Err.Game.PlayCard.NotYourTurn;
    }
    return CurrentTrick;
  }

  private void CheckPlayerHasCard(Player player, Card card)
  {
    if (!Cards[player].Contains(card))
    {
      throw Err.Game.PlayCard.DoNotHaveCard;
    }
  }
}
