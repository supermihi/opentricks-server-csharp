using Doppelkopf.Configuration;

namespace Doppelkopf.Persistence;

public record InitTableAction(IRules Rules, int NumberOfPlayers, int Version) : ITableAction;