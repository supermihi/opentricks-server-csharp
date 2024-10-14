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

  public Game(CardsByPlayer cards, ByPlayer<bool> needCompulsorySolo, GameConfiguration configuration)
  {
    _dealtCards = cards;
    Configuration = configuration;
    _auction = new Auction(_dealtCards, Configuration.GameModes.Holds, needCompulsorySolo);
    _bids = null;
    _trickTaking = null;
  }

  public GameConfiguration Configuration { get; }

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

  public void PlayCard(Player player, Card card)
  {
    if (_trickTaking is null)
    {
      throw ErrorCodes.InvalidPhase.ToException();
    }

    var finishedTrick = _trickTaking.PlayCard(player, card);
    if (finishedTrick == null)
    {
      return;
    }

    Contract!.OnTrickFinished(finishedTrick);
    if (_trickTaking.TryStartNextTrick())
    {
      return;
    }

    Phase = GamePhase.Finished;
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
    return Configuration.Evaluator.Evaluate(_trickTaking!.CompleteTricks, maxBids, Contract!.Parties.GetAll());
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
    Contract = Configuration.GameModes.CreateContract(auctionResult, _dealtCards);
    AuctionResult = auctionResult;
    _trickTaking = new TrickTaking(Contract.Traits, _dealtCards);
    _bids = new Bids(Contract.Parties, _trickTaking);
    Phase = GamePhase.TrickTaking;
  }

  public CardsByPlayer Cards => _trickTaking?.RemainingCards ?? _dealtCards;

  public Player? GetTurn() =>
    Phase switch
    {
      GamePhase.Auction => _auction.Turn,
      GamePhase.TrickTaking => _trickTaking!.Turn,
      _ => null
    };

  public IEnumerable<Declaration> Declarations => _auction.Declarations;
  public Trick? CurrentTrick => _trickTaking?.CurrentTrick;
  public IReadOnlyList<CompleteTrick> CompleteTricks => _trickTaking?.CompleteTricks ?? Array.Empty<CompleteTrick>();

  public int Age
  {
    get
    {
      var auctionAge = _auction.Declarations.Count;
      if (Phase == GamePhase.Auction)
      {
        return auctionAge;
      }
      var gameAge = _bids!.PlacedBids.Count + _trickTaking!.CompleteTricks.Count * Rules.NumPlayers
        + (_trickTaking.CurrentTrick?.Cards.Count ?? 0);
      return auctionAge + gameAge;
    }
  }
}
