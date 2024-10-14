using Doppelkopf.Core.Scoring.Impl;

namespace Doppelkopf.Core.Scoring;

public interface IPartyProvider
{
  Party? Get(Player player);
  int? DefiningTrick { get; }

  static IPartyProvider Solo(Player soloist) => StaticPartyProvider.Solo(soloist);
}
