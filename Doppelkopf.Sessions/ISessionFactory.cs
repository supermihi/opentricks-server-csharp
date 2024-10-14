using Doppelkopf.Core;

namespace Doppelkopf.Sessions;

public interface ISessionFactory
{
  ISession CreateSession(SessionConfiguration configuration, GameConfiguration gameConfiguration);
}
