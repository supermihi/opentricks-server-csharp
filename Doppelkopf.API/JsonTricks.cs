namespace Doppelkopf.API;

public sealed record JsonTricks(ByPlayerState<string?> CurrentTrick, ByPlayerState<int> TricksState);
