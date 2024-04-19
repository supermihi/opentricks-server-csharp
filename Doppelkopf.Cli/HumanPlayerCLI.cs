using System.Globalization;
using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Cli;

public class HumanPlayerCli(Player self) : IInteractiveClient
{
  public void StartGame(IPlayerClient player) => _game = player;

  private GameView? _view;
  private IPlayerClient? _game;
  private bool _contractPrinted;
  private int _maxPrintedTrick = -1;

  private static string OwnCardsToString(GameView view) =>
    string.Join("  ", view.OwnCards.Select((c, i) => $"({i}) {c.Display()}"));

  public Task OnStateChanged(GameView view)
  {
    if (_view is null)
    {
      Console.WriteLine("new game started");
      Console.WriteLine("your cards:");
      Console.WriteLine($"  {OwnCardsToString(view)}");
    }
    _view = view;
    if (!_contractPrinted && _view.TrickTaking is { Contract: var contract })
    {
      Console.WriteLine(contract.HoldId is null ? "normal game!" : $"{contract.Declarer} declares {contract.HoldId}");
      _contractPrinted = true;
    }
    if (_view.TrickTaking is { PreviousTrick: { } previousTrick } && previousTrick.Index > _maxPrintedTrick)
    {
      Console.WriteLine($"trick finished: {TrickToString(previousTrick)}; winner: {previousTrick.Winner}");
      _maxPrintedTrick = previousTrick.Index;
    }
    if (_view.Turn != self)
    {
      return Task.CompletedTask;
    }
    Console.WriteLine("it's your turn");

    if (_view.TrickTaking is not { } trickTaking)
    {
      Console.WriteLine($"auction! {AuctionToString(_view.Auction!)}");
      Console.WriteLine("please choose");
      Console.WriteLine("  (o) declare OK");
      Console.WriteLine("  (w) declare wedding");
    }
    else if (trickTaking.CurrentTrick is { } trick)
    {
      Console.WriteLine($"current trick: {TrickToString(trick)}");
      Console.WriteLine($"  {OwnCardsToString(view)}");
      Console.WriteLine("your options:");
      Console.WriteLine("  (#) play card at position # (a number)");
    }
    return Task.CompletedTask;
  }

  private static string TrickToString(TrickView trickView) =>
    string.Join(
      "  ",
      trickView.Cards.Zip(trickView.Leader.Cycle())
        .Select(cardAndPlayer => $"{cardAndPlayer.Second}: {cardAndPlayer.First.Display()}"));

  private static string AuctionToString(AuctionView auction) =>
    string.Join("  ", auction.Holds.Zip(auction.Leader.Cycle()).Select(t => $"{t.Second}: {t.First}"));

  public async Task Run(CancellationToken stoppingToken)
  {
    try
    {
      while (true)
      {
        var key = Console.ReadKey(true);
        if (_game == null)
        {
          continue;
        }
        try
        {
          switch (key.KeyChar)
          {
            case 'o' or 'O':
              await _game.DeclareHold(null);
              break;
            case 'w' or 'W':
              await _game.DeclareHold(ContractIds.Wedding);
              break;
            case >= '0' and <= '9':
              await PlayCardAt(int.Parse(key.KeyChar.ToString(), CultureInfo.CurrentCulture));
              break;
            case 'z':
              await PlayCardAt(10);
              break;
            case 'e':
              await PlayCardAt(11);
              break;
            default:
              Console.WriteLine($"invalid key: {key.KeyChar}");
              break;
          }
        }
        catch (InvalidMoveException e)
        {
          Console.WriteLine($"{e.Code}: {e.Message}");
        }
        catch (Exception f)
        {
          Console.WriteLine(f);
        }
      }
    }
    finally
    {
      Console.WriteLine("client finished");
    }
  }

  private Task PlayCardAt(int index)
  {
    if (index >= _view!.OwnCards.Count)
    {
      Console.WriteLine("invalid index");
      return Task.CompletedTask;
    }
    var card = _view!.OwnCards[index];
    Console.WriteLine($"trying to play {card.Display()} ...");
    return _game!.PlayCard(card);
  }
}
