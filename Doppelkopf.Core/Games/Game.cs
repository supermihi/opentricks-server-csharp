using Doppelkopf.Core.Auctions;
using Doppelkopf.Core.Auctions.Impl;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Scoring.Impl;
using Doppelkopf.Core.Tricks.Impl;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Games;

public class Game : IGame
{
  public GamePhase Phase { get; private set; } = GamePhase.Auction;
  public Game(CardsByPlayer cards, IByPlayer<bool> needCompulsorySolo, AvailableContracts availableContracts)
  {
    _initialCards = cards;
    Contracts = availableContracts;
    _auction = new Auction(_initialCards, availableContracts, needCompulsorySolo);
    _bids = null;
    _trickTaking = null;
  }

  public void MakeReservation(Player player, IDeclarableContract contract)
  {
    _auction.DeclareReservation(player, contract);
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
    if (finishedTrick != null)
    {
      _partyProvider.OnTrickFinished(finishedTrick);
      if (!_trickTaking.TryStartNextTrick())
      {
        Phase = GamePhase.Finished;
      }
    }
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
    var points = ByPlayer.Init(p => _trickTaking!.CompleteTricks.Select(t => t.Cards[p].Points()).Sum());
    var parties = ByPlayer.Init(p => _partyProvider.GetParty(p)!.Value);
    var rePoints = points.Items.Where(i => parties[i.player] == Party.Re).Sum(i => i.item);

    return new GameEvaluation(parties, points)
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
    _trickTaking = new TrickTaking(auctionResult.Contract.CardTraits, _initialCards);
    _partyProvider = auctionResult.Contract.CreatePartyProvider(auctionResult.Declarer, _initialCards);
    _bids = new ScoreController(_partyProvider, _trickTaking);
    Phase = GamePhase.TrickTaking;
  }

  private readonly IAuction _auction;
  private IBids? _bids;
  private TrickTaking? _trickTaking;
  private readonly CardsByPlayer _initialCards;
  public  AvailableContracts Contracts { get; }
  private IPartyProvider _partyProvider;
}
