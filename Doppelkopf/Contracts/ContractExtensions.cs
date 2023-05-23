using Doppelkopf.Cards;

namespace Doppelkopf.Contracts;

public static class ContractExtensions
{
    public static bool FollowsSuit(this ICardTraitsProvider cardTraits, Card defining, Card follow)
    {
        return cardTraits.Get(defining).TrickSuit == cardTraits.Get(follow).TrickSuit;
    }
}
