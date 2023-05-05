using Doppelkopf.Configuration;

namespace Doppelkopf;

public sealed record Contract(IGameMode Mode, Player? Soloist);