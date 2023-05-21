using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.GameFinding;
using Doppelkopf.Tricks;

namespace Doppelkopf.Games;

public sealed record Game(
  ByPlayer<IImmutableList<Card>> Cards,
  Auction Auction,
  TrickTaking? TrickTaking
)
{
  public static Game Init(ByPlayer<IImmutableList<Card>> cards) =>
    new(cards, Auction.Initial, null);

  public Contract? Contract => TrickTaking?.Contract;
  public bool IsFinished => TrickTaking?.IsFinished ?? false;

  public Game Reserve(Player player, bool reserved, GameContext context)
  {
    var (newAuction, contract) = Auction.Reserve(player, reserved, context.AuctionContext);
    return UpdateOnReserveOrDeclare(contract, newAuction);
  }

  private Game UpdateOnReserveOrDeclare(Contract? contract, Auction newAuction)
  {
    if (contract is not null)
    {
      return this with { Auction = newAuction, TrickTaking = TrickTaking.Initial(contract, Cards) };
    }
    return this with { Auction = newAuction };
  }

  public Game Declare(Player player, IGameMode declaration, GameContext context)
  {
    var (newAuction, contract) = Auction.Declare(player, declaration, context.AuctionContext);
    return UpdateOnReserveOrDeclare(contract, newAuction);
  }

  public Game PlayCard(Player player, Card card)
  {
    if (TrickTaking is null)
    {
      throw Err.TrickTaking.PlayCard.InvalidPhase;
    }
    var (newTrickTaking, _) = TrickTaking.PlayCard(player, card);
    return this with { TrickTaking = newTrickTaking };
  }
}