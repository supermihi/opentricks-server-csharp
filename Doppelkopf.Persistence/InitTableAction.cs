using Doppelkopf.Configuration;

namespace Doppelkopf.Persistence;

public record InitTable(IConfiguration Configuration, int NumberOfPlayers)
  : ITableActionPayload;

public record TableAction<T>(T Payload, ActionMeta Meta) where T : ITableActionPayload;