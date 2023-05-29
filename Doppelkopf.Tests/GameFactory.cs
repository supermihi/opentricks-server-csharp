using Doppelkopf.Games;

namespace Doppelkopf.Tests;

public static class GameFactory
{
  public static Game InitialMinikopfGame()
  {
    var config = Configuration.Minikopf;
    var cards = config.Cards.ShuffleCards(0);
    var context = new GameContext(ByPlayer.Init(false), config.Contracts, config.Tricks);
    return Game.Init(context, cards);
  }
}
