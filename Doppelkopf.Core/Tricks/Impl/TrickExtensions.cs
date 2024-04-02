using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Tricks.Impl;

internal static class TrickExtensions
{
  public static CompleteTrick Complete(this Trick trick, ICardTraitsProvider traits)
  {
    var winner = trick.GetWinner(traits);
    return new CompleteTrick(trick.Leader, winner, ByPlayer.Init(p => trick.Cards[p]), trick.Index, trick.Remaining);
  }

  public static Player GetWinner(this Trick trick, ICardTraitsProvider traits)
  {
    var winner = trick.Leader;
    var bestCard = trick.Cards.First();
    foreach (var player in trick.Cards.Players.Skip(1))
    {
      var card = trick.Cards[player];
      var takesTrick = card.TakesFrickFrom(bestCard, traits, trick.Remaining == 0);
      if (takesTrick)
      {
        winner = player;
        bestCard = card;
      }
    }

    return winner;
  }

  private static bool TakesFrickFrom(this Card next, Card previousBest, ICardTraitsProvider traits, bool isLastTrick)
  {
    var previousTraits = traits.Get(previousBest);
    var nextTraits = traits.Get(next);
    return (previousTraits.TrickSuit, nextTraits.TrickSuit) switch
    {
      (TrickSuit.Trump, not TrickSuit.Trump) => false,
      (not TrickSuit.Trump, TrickSuit.Trump) => true,
      var (x, y) when x != y => false,
      _ => (nextTraits.RankInTrickSuit - previousTraits.RankInTrickSuit) switch
      {
        > 0 => true,
        < 0 => false,
        0 => nextTraits.TieBreaking switch
        {
          TieBreakingMode.FirstWins => false,
          TieBreakingMode.SecondWins => true,
          TieBreakingMode.SecondWinsInLastTrick => isLastTrick,
          _ => throw new ArgumentOutOfRangeException("unexpected tie breaking mode", (Exception?)null)
        }
      }
    };
  }
}
