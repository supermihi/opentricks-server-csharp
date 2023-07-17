using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Games;

public interface IGameFactory
{
  IGame CreateGame(IByPlayer<bool> needsCompulsorySolo);
}

public  class GameFactory : IGameFactory {
  private readonly AvailableContracts _availableContracts;
  private readonly IDealer _dealer;
  private int _timesDealt;

  public GameFactory(AvailableContracts availableContracts, IDealer dealer)
  {
    _availableContracts = availableContracts;
    _dealer = dealer;
    _timesDealt = 0;
  }
  public IGame CreateGame(IByPlayer<bool> needsCompulsorySolo)
  {
    return new Game(_dealer.ShuffleCards(_timesDealt++), needsCompulsorySolo, _availableContracts);
  }
}
