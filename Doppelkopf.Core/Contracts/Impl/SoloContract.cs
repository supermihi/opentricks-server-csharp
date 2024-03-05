using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

internal class SoloContract : IContract
{
  internal SoloContract(ICardTraitsProvider cardTraits, Player soloist, string id)
  {
    CardTraits = cardTraits;
    Soloist = soloist;
    Id = id;
  }

  public ICardTraitsProvider CardTraits { get; }
  public Player Soloist { get; }
  public string Id { get; }
  public CardTraits GetTraits(Card card) => CardTraits.GetTraits(card);

  public Party? GetParty(Player player) => player == Soloist ? Party.Re : Party.Contra;

  public int? DefiningTrick => 0;

  public void OnTrickFinished(CompleteTrick trick)
  { }
}
