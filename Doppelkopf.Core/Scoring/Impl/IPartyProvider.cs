namespace Doppelkopf.Core.Scoring.Impl;

public interface IPartyProvider
{
  Party? GetParty(Player player);
  int? DeclaringTrick { get; }
}