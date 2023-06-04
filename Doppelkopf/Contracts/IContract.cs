using System.Collections.Immutable;
using Doppelkopf.Cards;
using Doppelkopf.Utils;

namespace Doppelkopf.Contracts;

public interface IContract : IEquatable<IContract>
{
  ContractType Type { get; }
  string Id { get; }
  ICardTraitsProvider CardTraits { get; }
  bool CanDeclare(ByPlayer<IImmutableList<Card>> cards, Player declarer);
  PartyData GetPartyData(ByPlayer<IImmutableList<Card>> cards, Player? declarer = null);

  bool IEquatable<IContract>.Equals(IContract? other) => other is { Id: var otherId } && Id == otherId;
}
