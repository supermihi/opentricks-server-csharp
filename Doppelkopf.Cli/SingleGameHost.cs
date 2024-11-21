using System.Threading.Channels;
using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Cli;

public sealed class SingleGameHost(IGameFactory gameFactory)
{
  private readonly IGame _game = gameFactory.CreateGame(ByPlayer.Init(false));
  private readonly Dictionary<Player, IBot> _bots = [];
  private readonly Channel<(Player, PlayerAction)> _interactiveActions = Channel.CreateBounded<(Player, PlayerAction)>(100);
  private readonly Dictionary<Player, IUnmanagedPlayer> _interactiveClients = [];
  public void AddBot(Player player, IBot bot) => _bots[player] = bot;

  public void AddInteractiveClient(Player player, IUnmanagedPlayer client)
  {
    _interactiveClients[player] = client;
    //var playerClient = new ChannelPlayerClient(player, _interactiveActions);
    var playerClient = new LockingSynchronousClient(_game, player);
    client.StartGame(playerClient);
  }

  private async Task NotifyInteractiveClients()
  {
    foreach (var (player, interactiveClient) in _interactiveClients)
    {
      await interactiveClient.OnStateChanged(_game.ToPlayerGameView(player));
    }
  }

  public async Task RunToCompletion(CancellationToken cancellationToken)
  {
    try
    {
      await NotifyInteractiveClients();
      await Run(cancellationToken);
    }
    catch (Exception e)
    {
      Console.WriteLine(e);
    }
  }

  private async Task Run(CancellationToken cancellationToken)
  {
    while (_game.GetTurn() is { } turn)
    {
      await Task.Delay(100, cancellationToken);
      if (_bots.TryGetValue(turn, out var bot))
      {
        var view = _game.ToPlayerGameView(turn);
        await bot.OnGameStateChanged(view, new LockingSynchronousClient(_game, turn));
        await NotifyInteractiveClients();
      }
      /*var timeout = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
      var cancel = CancellationTokenSource.CreateLinkedTokenSource(timeout.Token, cancellationToken);
      try
      {
        await ExecutePlayerAction(turn, cancel.Token);
      }
      catch (OperationCanceledException)
      {
      }*/
    }
    var result = _game.Evaluate();
    var type = _game.AuctionResult!.Hold?.Id ?? "normal game";
    Console.WriteLine(
      $"{type} finished! The winner is: {result.Winner}, scoring {result.ScoreByParty()[result.Winner!.Value]}");
    Console.WriteLine(result.Parties);
    Console.WriteLine(result.Totals);
    foreach (var score in result.Scores)
    {
      Console.WriteLine($"{score.Id} for {score.Party}");
    }
  }

  private async Task ExecutePlayerAction(Player turn, CancellationToken cancellationToken)
  {
    var (player, action) = await _interactiveActions.Reader.ReadAsync(cancellationToken);
    try
    {
      _game.Play(player, action);
    }
    catch (InvalidMoveException ime)
    {
      Console.WriteLine($"{ime.Code}");
    }
  }
}

