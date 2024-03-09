using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Games.Impl;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Games;

public class GameFactory : IGameFactory
{
  private readonly Modes _modes;
  private readonly IDealer _dealer;
  private int _timesDealt;

  public GameFactory(Modes modes, IDealer dealer)
  {
    _modes = modes;
    _dealer = dealer;
    _timesDealt = 0;
  }

  public IGame CreateGame(ByPlayer<bool> needsCompulsorySolo) =>
    new Game(_dealer.ShuffleCards(_timesDealt++), needsCompulsorySolo, _modes);
}
