using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Auctions.Impl;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Games.Impl;

internal class Game : IGame
{
  public GamePhase Phase { get; private set; } = GamePhase.Auction;

  public Game(CardsByPlayer cards, IByPlayer<bool> needCompulsorySolo, Modes modes)
  {
    _dealtCards = cards;
    Modes = modes;
    _auction = new Auction(_dealtCards, modes.Holds, needCompulsorySolo);
    _bids = null;
    _trickTaking = null;
  }

  public Modes Modes { get; }

  private readonly IAuction _auction;
  private IBids? _bids;
  private TrickTaking? _trickTaking;
  private readonly CardsByPlayer _dealtCards;
  private IContract? _contract;

  public void DeclareHold(Player player, IHold hold)
  {
    _auction.DeclareReservation(player, hold);
    TryFinishAuction();
  }

  public void DeclareOk(Player player)
  {
    _auction.DeclareOk(player);
    TryFinishAuction();
  }

  public PlayCardResult PlayCard(Player player, Card card)
  {
    if (_trickTaking is null)
    {
      throw ErrorCodes.InvalidPhase.ToException();
    }

    var finishedTrick = _trickTaking.PlayCard(player, card);
    GameEvaluation? evaluation = null;
    if (finishedTrick == null)
    {
      return new PlayCardResult(finishedTrick, evaluation);
    }

    _contract!.OnTrickFinished(finishedTrick);
    if (_trickTaking.TryStartNextTrick())
    {
      return new PlayCardResult(finishedTrick, evaluation);
    }

    Phase = GamePhase.Finished;
    evaluation = Evaluate();

    return new PlayCardResult(finishedTrick, evaluation);
  }

  public void PlaceBid(Player player, Bid bid)
  {
    if (_bids is null)
    {
      throw ErrorCodes.InvalidPhase.ToException();
    }

    _bids.PlaceBid(player, bid);
  }

  public GameEvaluation Evaluate()
  {
    if (Phase != GamePhase.Finished)
    {
      throw ErrorCodes.InvalidPhase.ToException();
    }

    var parties = ByPlayer.Init(p => _contract!.GetParty(p)!.Value);
    var (winner, score) = Evaluation.Evaluate(
      _trickTaking!.CompleteTricks,
      parties,
      new ByParty<Bid?>(_bids!.MaxBidOf(Party.Re), _bids!.MaxBidOf(Party.Contra)));
    throw new NotImplementedException();
  }

  private void TryFinishAuction()
  {
    if (_auction.Evaluate() is { } auctionResult)
    {
      StartTrickTaking(auctionResult);
    }
  }

  private void StartTrickTaking(AuctionResult auctionResult)
  {
    if (Phase != GamePhase.Auction)
    {
      throw new InvalidOperationException("can only start trick taking when in auction phase");
    }

    _contract = Modes.CreateContract(auctionResult, _dealtCards);
    _trickTaking = new TrickTaking(_contract, _dealtCards);
    _bids = new Bids(_contract, _trickTaking);
    Phase = GamePhase.TrickTaking;
  }
}
