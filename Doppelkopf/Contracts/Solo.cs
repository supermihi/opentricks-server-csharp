using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Contracts;

public class Solo : IContract
{
  public string Id { get; }
  public ICardTraitsProvider CardTraits { get; }

  public Solo(string id, ICardTraitsProvider cardTraits)
  {
    Id = id;
    CardTraits = cardTraits;
  }

  public ContractType Type => ContractType.Solo;

  public PartyData GetPartyData(ByPlayer<IImmutableList<Card>> cards, Player? declarer)
  {
    if (declarer is not { } soloist)
    {
      throw new ArgumentException("solo contract needs a declarer");
    }
    var parties = ByPlayer.Init(Party.Contra).Replace(soloist, Party.Re);
    return new PartyData(parties, 0, soloist);
  }

  public bool CanDeclare(ByPlayer<IImmutableList<Card>> cards, Player declarer) => true;
}
