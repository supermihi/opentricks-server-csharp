using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

public class NormalGameProvider(TieBreakingMode heartTenTieBreaking) : INormalGameProvider
{
  public IContract CreateNormalGame(CardsByPlayer initialCards)
  {
    var hasClubQueen = initialCards.Apply(cards => cards.Contains(Card.ClubsQueen));
    var isWedding = hasClubQueen.Values.Count(has => has) == 1;
    if (isWedding)
    {
      return new WeddingContract(
        heartTenTieBreaking,
        hasClubQueen.Single(kvp => kvp.Value).Key,
        false);
    }

    return new NormalGameContract(heartTenTieBreaking, hasClubQueen);
  }
}
