using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Configuration;
using Doppelkopf.Errors;
using Doppelkopf.GameFinding;
using Doppelkopf.Tricks;

namespace Doppelkopf;

public sealed record Match(
  ByPlayer<IImmutableList<Card>> Cards,
  Auction Auction,
  TrickTaking? TrickTaking
)
{
  public static Match Init(ByPlayer<IImmutableList<Card>> cards) =>
    new(cards, Auction.Initial, null);

  public Contract? Contract => TrickTaking?.Contract;
  public bool IsFinished => TrickTaking?.IsFinished ?? false;

  public Match Reserve(Player player, bool reserved, MatchContext context)
  {
    var (newAuction, contract) = Auction.Reserve(player, reserved, context.AuctionContext);
    return UpdateOnReserveOrDeclare(contract, newAuction);
  }

  private Match UpdateOnReserveOrDeclare(Contract? contract, Auction newAuction)
  {
    if (contract is not null)
    {
      return this with { Auction = newAuction, TrickTaking = TrickTaking.Initial(contract, Cards) };
    }
    return this with { Auction = newAuction };
  }

  public Match Declare(Player player, IGameMode declaration, MatchContext context)
  {
    var (newAuction, contract) = Auction.Declare(player, declaration, context.AuctionContext);
    return UpdateOnReserveOrDeclare(contract, newAuction);
  }

  public Match PlayCard(Player player, Card card)
  {
    if (TrickTaking is null)
    {
      throw Err.Game.PlayCard.InvalidPhase;
    }
    var (newTrickTaking, finished) = TrickTaking.PlayCard(player, card);
    return this with { TrickTaking = newTrickTaking };
  }
}

public sealed record MatchContext(ByPlayer<bool> NeedsCompulsorySolo, GameModeCollection Modes)
{
  public AuctionContext AuctionContext => new(NeedsCompulsorySolo, Modes);
}
