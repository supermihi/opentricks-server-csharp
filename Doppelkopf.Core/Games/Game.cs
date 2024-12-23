using Doppelkopf.API.Errors;
using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Games;

internal class Game : IGame
{
  public Game(CardsByPlayer cards, ByPlayer<bool> needCompulsorySolo, GameConfiguration configuration)
  {
    _dealtCards = cards;
    Configuration = configuration;
    _auction = new Auction(_dealtCards, Configuration.GameModes.Holds, needCompulsorySolo);
    _bids = null;
    _trickTaking = null;
  }

  public GameConfiguration Configuration { get; }
  public GamePhase Phase { get; private set; } = GamePhase.Auction;
  private readonly IAuction _auction;
  private IBids? _bids;
  private TrickTaking? _trickTaking;
  private readonly CardsByPlayer _dealtCards;
  public IContract? Contract { get; private set; }

  public void Play(Player player, PlayerAction action)
  {
    if (action.Declaration is { } declaration)
    {
      Declare(player, declaration);
    }
    else if (action.PlayedCard is { } card)
    {
      PlayCard(player, card);
    }
    else if (action.Bid is { } bid)
    {
      PlaceBid(player, bid);
    }
    else
    {
      throw new ArgumentOutOfRangeException(nameof(action), "undefined action");
    }
  }

  private void Declare(Player player, Declaration declaration)
  {
    _auction.Declare(player, declaration);
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
