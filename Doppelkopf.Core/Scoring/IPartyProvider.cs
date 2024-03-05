namespace Doppelkopf.Core.Scoring;

public interface IPartyProvider
{
  Party? GetParty(Player player);
  int? DefiningTrick { get; }
}
