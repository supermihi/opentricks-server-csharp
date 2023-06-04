using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Contracts;
using Doppelkopf.Utils;

namespace Doppelkopf.Games;

public sealed record Game(GameContext Context,
  ByPlayer<IImmutableList<Card>> Cards,
  Auction Auction,
  PartyData PartyData,
  TrickTaking? TrickTaking)
{
  public static Game Init(GameContext context, ByPlayer<IImmutableList<Card>> cards) =>
      new(context, cards, Auction.Initial, PartyData.NothingClarified, null);

  public bool IsFinished => TrickTaking?.IsFinished ?? false;
  public IContract? Contract => TrickTaking?.Contract;

  public Game Reserve(Player player, bool reserved)
  {
    var (newAuction, result) = Auction.Reserve(player, reserved, AuctionContext);
    return UpdateOnReserveOrDeclare(result, newAuction);
  }

  private AuctionContext AuctionContext => new(Context.NeedsCompulsorySolo, Context.Contracts, Cards);

  private Game UpdateOnReserveOrDeclare(AuctionResult? result, Auction newAuction)
  {
    if (result is not null)
    {
      return this with
      {
          Auction = newAuction,
          PartyData = result.PartyData,
          TrickTaking = TrickTaking.Initial(result.Contract, Context.TrickConfiguration, Cards)
      };
    }
    return this with { Auction = newAuction };
  }

  public Game Declare(Player player, IContract declaration)
  {
    var (newAuction, contract) = Auction.Declare(player, declaration, AuctionContext);
    return UpdateOnReserveOrDeclare(contract, newAuction);
  }

  public (Game result, bool finishedTrick) PlayCard(Player player, Card card)
  {
    if (TrickTaking is null)
    {
      throw Errors.Generic.InvalidPhase;
    }
    var (newTrickTaking, finished) = TrickTaking.PlayCard(player, card);
    return (this with { TrickTaking = newTrickTaking }, finished);
  }

  public (Game result, bool finishedTrickTaking) FinishTrick()
  {
    if (TrickTaking is null)
    {
      throw Errors.Generic.InvalidPhase;
    }
    var (nextTrickTaking, finished) = TrickTaking.FinishTrick();
    return (this with { TrickTaking = nextTrickTaking }, finished);
  }
}
