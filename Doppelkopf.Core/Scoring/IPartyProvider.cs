namespace Doppelkopf.Core.Scoring;

public interface IPartyProvider
{
  Party? Get(Player player);
  int? DefiningTrick { get; }
}
