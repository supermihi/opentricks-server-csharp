using Doppelkopf.Configuration;
using Doppelkopf.Sessions;

namespace Doppelkopf.Persistence;

public record Declare(Seat Seat, IGameMode Mode) : ISeatedTableAction;
