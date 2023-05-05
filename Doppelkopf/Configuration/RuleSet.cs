using System.Collections.Immutable;
using Doppelkopf.Cards;

namespace Doppelkopf.Configuration;

public sealed record RuleSet(
  bool WithoutNines,
  EldersMode Elders,
  bool ThrowOnFiveNines,
  int NumberOfGames,
  bool CompulsorySolos
)
{
  public static RuleSet DDKV = new(false, EldersMode.FirstWins, false, 20, true);
  public IImmutableList<Card> Deck => WithoutNines ? Decks.WithoutNines : Decks.WithNines;
  public int NumberOfTricks => Deck.Count / Constants.NumberOfPlayers;
}

public enum EldersMode
{
  FirstWins,
  SecondWins,
  FirstWinsExceptInLastTrick
}
