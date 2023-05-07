using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public interface IRules
{
  GameModeCollection Modes { get; }

  RoundConfiguration Rounds { get; }
  
  IImmutableList<Card> Deck { get; }
}