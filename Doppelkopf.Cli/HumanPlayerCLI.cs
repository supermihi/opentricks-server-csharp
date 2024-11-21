using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Cli;

public class HumanPlayerCli(Player self) : IUnmanagedPlayer
{
  public void StartGame(IPlayerClient player) => _game = player;

  private GameView? _view;
  private IPlayerClient? _game;
  private bool _contractPrinted;
  private int _maxPrintedTrick = -1;

  private static string OwnCardsToString(IReadOnlyList<Card> cards) =>
    string.Join("  ", cards.Select((c, i) => $"({i}) {c.Display()}"));

  public Task OnStateChanged(GameView view)
  {
    if (_view is null)
    {
      Console.WriteLine("New game started. Your cards:");
      Console.WriteLine($"  {OwnCardsToString(view.OwnCards)}");
    }
    _view = view;
    switch (view.Phase)
    {
      case GamePhase.Auction:
        OnChangeInAuction(view.Auction!, view.Turn!.Value);
        break;
      case GamePhase.TrickTaking:
        OnChangeInTrickTaking(view.TrickTaking!, view.OwnCards, view.Turn!.Value);
        break;
      case GamePhase.Finished:
        OnFinish();
        break;
      default:
        break;
    }
    Console.Out.Flush();
    return Task.CompletedTask;
  }

  private void OnChangeInAuction(AuctionView auction, Player turn)
  {
    Console.WriteLine($"auction! {AuctionToString(auction)}");
    if (turn == self)
    {
      Console.WriteLine("  (o) declare OK");
      Console.WriteLine("  (w) declare wedding");
    }
  }

  private void OnChangeInTrickTaking(TrickTakingView trickTaking, IReadOnlyList<Card> cards, Player turn)
  {
    if (!_contractPrinted && trickTaking is { Contract: var contract })
    {
      Console.WriteLine(contract.HoldId is null ? "It's a normal game!" : $"{contract.Declarer} declares {contract.HoldId}");
      _contractPrinted = true;
    }
    if (trickTaking is { PreviousTrick: { } previousTrick } && previousTrick.Index > _maxPrintedTrick)
    {
      Console.WriteLine($"trick finished: {TrickToString(previousTrick)}; winner: {PlayerName(previousTrick.Winner!.Value)}");
      _maxPrintedTrick = previousTrick.Index;
    }
    if (trickTaking.CurrentTrick is { } trick)
    {
      Console.WriteLine(TrickToString(trick));
    }
    if (turn == self)
    {
      Console.WriteLine($"  {OwnCardsToString(cards)}");
      Console.WriteLine("your options:");
      Console.WriteLine("  (#) play card at position # (a number)");
    }
  }

  private static void OnFinish() { }

  private string TrickToString(TrickView trickView)
  {
    if (!trickView.Cards.Any())
    {
      return $"{PlayerName(trickView.Leader)} open(s)";
    }
    return string.Join(
      "  ",
      trickView.Cards.Zip(trickView.Leader.Cycle())
        .Select(cardAndPlayer => $"{PlayerName(cardAndPlayer.Second)}: {cardAndPlayer.First.Display()}"));
  }

  private string PlayerName(Player player)
  {
    if (player == self)
    {
      return "you";
    }
    return $"Player {player}";
  }

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
              Console.WriteLine("declaring healthy");
              await Play(PlayerAction.Declare.Healthy());
              break;
            case 'w' or 'W':
              Console.WriteLine("Declaring wedding");
              await Play(PlayerAction.Declare.Hold(HoldIds.Wedding));
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
      }
    }
    catch (Exception f)
    {
      Console.WriteLine(f);
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
    return Play(PlayerAction.Card(card));
  }

  private async Task Play(PlayerAction action)
  {
    var result = await _game!.Play(action);
    if (result is not { } error)
    {
      return;
    }
    Console.WriteLine($"invalid move: {error.Code} ({error.Message})");
  }
}
