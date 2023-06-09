using System.Collections.Immutable;
using Doppelkopf.Server.Model;

namespace Doppelkopf.Server.TableActions;

/// <param name="Table">The resulting table state of the latest action.</param>
/// <param name="Events">List of events.</param>
public sealed record TableActionResult(Table Table, IImmutableList<TableEvent> Events)
{
  public DateTime Timestamp { get; } = DateTime.UtcNow;

  public TableActionResult(Table result, params TableEvent[] events) : this(result, events.ToImmutableList())
  { }

  public TableActionResult Add(TableEvent @event)
  {
    return this with { Events = Events.Add(@event) };
  }
}
