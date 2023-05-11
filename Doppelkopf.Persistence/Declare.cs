using Doppelkopf.Configuration;
using Doppelkopf.Tables;

namespace Doppelkopf.Persistence;

public record Declare(Seat Seat, IGameMode Mode) : ISeatedTableAction;
