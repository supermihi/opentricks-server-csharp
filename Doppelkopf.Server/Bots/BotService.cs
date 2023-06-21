using System.Threading.Channels;
using Doppelkopf.API;
using Doppelkopf.Cards;
using Doppelkopf.Games;
using Doppelkopf.Server.Controllers;
using Doppelkopf.Server.Notifications;
using Doppelkopf.Server.TableActions;

namespace Doppelkopf.Server.Bots;

public class BotService : BackgroundService, INotificationHandler
{
  private readonly BotIds _ids;
  private readonly ITableService _tableService;
  private readonly Channel<TableActionResult> _channel;

  public BotService(NotificationDispatcher dispatcher, BotIds ids, ITableService tableService)
  {
    _ids = ids;
    _tableService = tableService;
    _channel = Channel.CreateBounded<TableActionResult>(10_000);
    dispatcher.Subscribe(this);
  }

  public Task OnTableAction(TableActionResult result, UserId actor)
  {
    var botsAtTable = result.Table.Users.Where(u => _ids.Bots.Contains(u)).ToArray();

    if (botsAtTable.Length > 0)
    {
      if (!_channel.Writer.TryWrite(result))
      {
        throw new Exception("bot channel full");
      }
    }
    return Task.CompletedTask;
  }

  protected override async Task ExecuteAsync(CancellationToken stoppingToken)
  {
    await foreach (var entry in _channel.Reader.ReadAllAsync(stoppingToken))
    {
      await ProcessAction(entry);
    }
  }

  private async Task ProcessAction(TableActionResult actionResult)
  {
    var (table, _) = actionResult;
    if (table.Session is null or { IsFinished: true })
    {
      return;
    }
    var game = table.Session.Game;
    var (auctionPhase, auctionTurn) = game.Auction.PhaseAndTurn();
    TableRequest? request;
    UserId user;
    if (auctionPhase != Auction.AuctionPhase.Finished)
    {
      user = table.UserForPlayer(auctionTurn);
      if (!_ids.IsBot(user))
      {
        return;
      }
      if (auctionPhase == Auction.AuctionPhase.Reservations)
      {
        request = new TableRequest(RequestType.Reserve) { IsReserved = false };
      }
      else
      {
        request = new TableRequest(RequestType.Declare)
        {
          ContractId = table.Session.Configuration.Contracts.Solos.First().Id
        };
      }
    }
    else
    {
      var trick = game.TrickTaking?.CurrentTrick;
      if (trick?.Turn is null)
      {
        return;
      }
      var player = trick.Turn.Value;
      user = table.UserForPlayer(player);
      if (!_ids.IsBot(user))
      {
        return;
      }
      var validCard = game.Cards[player]
          .Cast<Card?>()
          .FirstOrDefault(card => trick.IsValidNextCard(card!.Value, game.Cards[player], game.Contract!.CardTraits));
      if (validCard is null)
      {
        throw new ArgumentException("no valid card, but my turn");
      }
      request = new TableRequest(RequestType.PlayCard) { CardId = validCard.Value.Id };
    }
    Console.WriteLine($"bot {user} acting request {request}");
    await _tableService.Act(table, user, request);
  }
}
