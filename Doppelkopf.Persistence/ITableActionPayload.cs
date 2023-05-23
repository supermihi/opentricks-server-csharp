namespace Doppelkopf.Persistence;

public interface ITableActionPayload
{

}

public record ActionMeta(int Version, DateTime InitialUtc);
