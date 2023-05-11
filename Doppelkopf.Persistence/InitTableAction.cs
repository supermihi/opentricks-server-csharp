using Doppelkopf.Configuration;

namespace Doppelkopf.Persistence;

public record InitTable(IRules Rules, int NumberOfPlayers)
  : ITableActionPayload;

public record TableAction<T>(T Payload, ActionMeta Meta) where T : ITableActionPayload;