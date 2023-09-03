using Doppelkopf.Users.API;

namespace Doppelkopf.Service;

public interface ITableFactory
{
  ITable Create(string name, UserId owner);
}
