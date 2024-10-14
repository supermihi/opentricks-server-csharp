using Doppelkopf.Bot;
using Doppelkopf.Core;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Cli;

public sealed class SingleGameHost(IGameFactory gameFactory) : IDisposable
{
  private readonly IGame _game = gameFactory.CreateGame(ByPlayer.Init(false));
  private readonly Dictionary<Player, IBot> _bots = [];
  private readonly Dictionary<Player, IInteractiveClient> _interactiveClients = [];
  public void AddBot(Player player, IBot bot) => _bots[player] = bot;
  private readonly ManualResetEventSlim _mre = new();

  public void AddInteractiveClient(Player player, IInteractiveClient client)
  {
    _interactiveClients[player] = client;
    client.StartGame(new PlayerClient(_game, player, () => _mre.Set()));
  }

  private void NotifyInteractiveClients()
  {
    foreach (var (player, interactiveClient) in _interactiveClients)
    {
      interactiveClient.OnStateChanged(_game.ToPlayerGameView(player));
    }
  }

  public Task RunToCompletion(CancellationToken cancellationToken)
  {
    while (true)
    {
      var turn = _game.GetTurn();
      if (turn is not { } player)
      {
        break;
      }
      var view = _game.ToPlayerGameView(player);


      if (_bots.TryGetValue(player, out var bot))
      {
        bot.OnGameStateChanged(view, new PlayerClient(_game, player, null)).GetAwaiter().GetResult();
      }
      else
      {
        _mre.Wait(cancellationToken);
        _mre.Reset();
      }
      NotifyInteractiveClients();
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
    return Task.CompletedTask;
  }

  public void Dispose() => _mre.Dispose();
}
