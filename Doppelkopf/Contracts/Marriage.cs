using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Contracts;

public class Marriage : IContract
{
  public ContractType Type => ContractType.Marriage;

  public string Id { get; }

  public Marriage(string id)
  {
    Id = id;
  }

  public ICardTraitsProvider CardTraits => CardTraitsProvider.DiamondsTrump;

  public bool CanDeclare(ByPlayer<IImmutableList<Card>> cards, Player declarer)
  {
    return cards[declarer].Count(c => c == Card.QueenOfClubs) == 2;
  }

  public PartyData GetPartyData(ByPlayer<IImmutableList<Card>> cards, Player? declarer)
  {
    if (declarer is null)
    {
      throw new ArgumentException("marriage needs a declarer");
    }
    return new(ByPlayer.Init(Party.NotClarified).Replace(declarer.Value, Party.Re), null, null);
  }
}
