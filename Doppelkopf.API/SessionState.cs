namespace Doppelkopf.API;

public sealed record SessionState(IReadOnlyList<int> PlayerIndexes, bool IsFinished, GameState? CurrentGame);
