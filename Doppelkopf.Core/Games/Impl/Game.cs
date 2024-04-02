using System.Collections.Immutable;
using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Auctions.Impl;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Games.Impl;

internal class Game : IGame
{
  public GamePhase Phase { get; private set; } = GamePhase.Auction;

  public Game(CardsByPlayer cards, ByPlayer<bool> needCompulsorySolo, Modes modes)
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
  public IContract? Contract { get; private set; }

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

    Contract!.OnTrickFinished(finishedTrick);
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
      ErrorCodes.InvalidPhase.Throw();
    }
    var maxBids = ByParty.Init(party => _bids!.MaxBidOf(party)!);
    return Contract!.Evaluate(_trickTaking!.CompleteTricks, maxBids);
  }

  private void TryFinishAuction()
  {
    if (_auction.Evaluate() is { } auctionResult)
    {
      StartTrickTaking(auctionResult);
    }
  }

  public AuctionResult? AuctionResult { get; private set; }

  private void StartTrickTaking(AuctionResult auctionResult)
  {
    Contract = Modes.CreateContract(auctionResult, _dealtCards);
    AuctionResult = auctionResult;
    _trickTaking = new TrickTaking(Contract.Traits, _dealtCards);
    _bids = new Bids(Contract.Parties, _trickTaking);
    Phase = GamePhase.TrickTaking;
  }

  public ByPlayer<ImmutableArray<Card>> Cards => _trickTaking?.RemainingCards ?? _dealtCards;

  public Player? Turn =>
    Phase switch
    {
      GamePhase.Auction => _auction.Turn,
      GamePhase.TrickTaking => _trickTaking!.Turn,
      _ => null
    };

  public IEnumerable<Declaration> Declarations => _auction.Declarations;
  public Trick? CurrentTrick => _trickTaking?.CurrentTrick;
  public IReadOnlyList<CompleteTrick> CompleteTricks => _trickTaking?.CompleteTricks ?? Array.Empty<CompleteTrick>();
}
