using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Auctions.Impl;

public static class CardTrainsProviderExtensions
{
  public static bool FollowsSuit(this ICardTraitsProvider cardTraits, Card defining, Card follow)
  {
    return cardTraits.Get(defining).TrickSuit == cardTraits.Get(follow).TrickSuit;
  }
}
