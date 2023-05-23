using Doppelkopf.Contracts;
using Doppelkopf.Sessions;

namespace Doppelkopf.Persistence;

public record Declare(Seat Seat, IContract Contract) : ISeatedTableAction;
