using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Contracts;

public interface IContract
{
  ContractType Type { get; }
  string Id { get; }
  ICardTraitsProvider CardTraits { get; }
  bool CanDeclare(ByPlayer<IImmutableList<Card>> cards, Player declarer);
  PartyData GetPartyData(ByPlayer<IImmutableList<Card>> cards, Player? declarer = null);
}

public sealed record PartyData(ByPlayer<Party> PartyOf, int? ClarifyingTrick, Player? Soloist)
{
  public bool IsClarified => PartyOf.All(p => p != Party.NotClarified);
  public static PartyData NothingClarified => new(ByPlayer.Init(Party.NotClarified), null, null);
}

public enum Party
{
  Re,
  Contra,
  NotClarified
}
