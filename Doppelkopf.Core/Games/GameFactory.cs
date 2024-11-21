using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Games;

public class GameFactory(GameConfiguration configuration, IDealer dealer) : IGameFactory
{
  private int _timesDealt;

  public IGame CreateGame(ByPlayer<bool> needsCompulsorySolo) =>
    new Game(dealer.ShuffleCards(_timesDealt++), needsCompulsorySolo, configuration);
}
