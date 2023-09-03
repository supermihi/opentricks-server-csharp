using Doppelkopf.Core;
using Doppelkopf.Core.Games;

namespace Doppelkopf.Sessions;

public interface ISessionFactory
{
  ISession CreateSession(SessionConfiguration configuration, GameConfiguration gameConfiguration);
}
