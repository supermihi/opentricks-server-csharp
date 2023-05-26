using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Contracts;

public class NormalGame : IContract
{
  public string Id { get; }
  public ContractType Type => ContractType.NormalGame;

  public NormalGame(string id = "normal_game")
  {
    Id = id;
  }

  public ICardTraitsProvider CardTraits => CardTraitsProvider.DiamondsTrump;

  public PartyData GetPartyData(ByPlayer<IImmutableList<Card>> cards, Player? declarer)
  {
    if (declarer != null)
    {
      throw new ArgumentException("normal game cannot be declared");
    }
    var parties = ByPlayer.Init(
      player => cards[player].Any(c => c == Card.QueenOfClubs) ? Party.Re : Party.Contra
    );
    return new(parties, 0, null);
  }

  public bool CanDeclare(ByPlayer<IImmutableList<Card>> cards, Player declarer) => false;
}
