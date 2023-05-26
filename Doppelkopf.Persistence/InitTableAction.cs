using Doppelkopf.Conf;

namespace Doppelkopf.Persistence;

public record InitTable(Configuration Configuration, int NumberOfPlayers)
    : ITableActionPayload;

public record TableAction<T>(T Payload, ActionMeta Meta) where T : ITableActionPayload;
