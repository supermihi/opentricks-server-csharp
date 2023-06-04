using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Errors;
using Doppelkopf.Tricks;
using Doppelkopf.Utils;

namespace Doppelkopf.Games;

/// <summary>
/// The trick taking part of a Doppelkopf game.
/// </summary>
public sealed record TrickTaking(IContract Contract,
  TrickConfiguration Config,
  ByPlayer<IImmutableList<Card>> Cards,
  IImmutableList<CompleteTrick> CompleteTricks,
  Trick? CurrentTrick)
{
  public static TrickTaking Initial(IContract contract,
    TrickConfiguration config,
    ByPlayer<IImmutableList<Card>> cards) =>
      new(contract, config, cards, ImmutableList<CompleteTrick>.Empty, new(Player.Player1));

  public bool IsFinished => CurrentTrick is null;

  /// <summary>
  /// Let <paramref name="player"/> play the given <paramref name="card"/>.
  /// </summary>
  /// <returns>The result of the operation, and a <c>bool</c> showing whether the played card finished the current
  /// trick; in the latter case, the call must be followed by <see cref="FinishTrick"/>.
  /// </returns>
  public (TrickTaking result, bool finishedTrick) PlayCard(Player player, Card card)
  {
    var currentTrick = CheckIsPlayersTurn(player);
    CheckPlayerHasCard(player, card);
    CheckCardIsValid(player, card, currentTrick);

    var nextGame = WithCardPlayed(player, card, currentTrick);
    return (nextGame, nextGame.CurrentTrick!.IsFull);
  }

  public (TrickTaking result, bool finishedGame) FinishTrick()
  {
    if (CurrentTrick is not { IsFull: true })
    {
      throw new IllegalStateException("cannot complete trick that is not full");
    }
    var tricksLeft = Cards[Player.Player1].Count;
    var currentTrickIsLast = tricksLeft == 0;
    var context = new TrickContext(Contract.CardTraits, Config, currentTrickIsLast);
    var currentTrickFinished = CompleteTrick.FromTrick(CurrentTrick, context);
    var nextGame =  this with { CompleteTricks = CompleteTricks.Add(currentTrickFinished),
        CurrentTrick = currentTrickIsLast ? null : new(currentTrickFinished.Winner)
    };
    return (nextGame, currentTrickIsLast);
  }

  private TrickTaking WithCardPlayed(Player player, Card card, Trick currentTrick)
  {
    var playersCardsWithoutJustPlayed = Cards.Replace(player, Cards[player].Remove(card));
    var trickWithJustPlayed = currentTrick.AddCard(card);

    return this with { Cards = playersCardsWithoutJustPlayed, CurrentTrick = trickWithJustPlayed, };
  }

  private void CheckCardIsValid(Player player, Card card, Trick currentTrick)
  {
    if (!currentTrick.IsValidNextCard(card, Cards[player], Contract.CardTraits))
    {
      throw Err.TrickTaking.PlayCard.Forbidden;
    }
  }

  private Trick CheckIsPlayersTurn(Player player)
  {
    if (CurrentTrick is null)
    {
      throw Err.TrickTaking.PlayCard.InvalidPhase;
    }
    var currentPlayer = CurrentTrick.Turn;
    if (currentPlayer is null)
    {
      throw new ArgumentException("no current player");
    }
    if (currentPlayer != player)
    {
      throw Err.TrickTaking.PlayCard.NotYourTurn;
    }
    return CurrentTrick;
  }

  private void CheckPlayerHasCard(Player player, Card card)
  {
    if (!Cards[player].Contains(card))
    {
      throw Err.TrickTaking.PlayCard.DoNotHaveCard;
    }
  }
}
