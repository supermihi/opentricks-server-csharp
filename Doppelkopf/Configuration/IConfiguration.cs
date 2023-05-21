using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public interface IConfiguration
{
  GameModes Modes { get; }

  SessionConfiguration Rounds { get; }

  IReadOnlyList<Card> Deck { get; }
}
