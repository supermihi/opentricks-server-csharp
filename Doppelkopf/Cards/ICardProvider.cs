using System.Collections.Immutable;

namespace Doppelkopf.Cards;

public interface ICardProvider
{
  ByPlayer<IImmutableList<Card>> ShuffleCards(int gameIndex);
  Card GetById(string cardId);
}
