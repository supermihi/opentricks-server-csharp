namespace Doppelkopf.Server;

public record struct TableId(string Id) {
  public static TableId New() => new(Guid.NewGuid().ToString());
}