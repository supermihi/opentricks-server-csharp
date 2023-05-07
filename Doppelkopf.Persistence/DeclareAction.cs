using Doppelkopf.Configuration;
using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public record DeclareAction(Seat Seat, IGameMode Mode, int Version) : ISeatedTableAction;