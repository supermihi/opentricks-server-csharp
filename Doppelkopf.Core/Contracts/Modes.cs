using Doppelkopf.Core.Contracts.Impl;

namespace Doppelkopf.Core.Contracts;

/// <summary>
/// Defines available game modes.
/// </summary>
public record Modes(IReadOnlyCollection<IHold> Holds, INormalGameProvider NormalGame)
{
  public static Modes Default(RuleOptions rules)
  {
    var holds = new IHold[]
    {
      new Wedding(rules.HeartTenTieBreaking), Solo.Fleshless, Solo.JackSolo, Solo.QueenSolo,
      Solo.SuitSolo(Suit.Diamonds, rules.HeartTenTieBreaking), Solo.SuitSolo(Suit.Hearts, rules.HeartTenTieBreaking),
      Solo.SuitSolo(Suit.Spades, rules.HeartTenTieBreaking), Solo.SuitSolo(Suit.Clubs, rules.HeartTenTieBreaking)
    };
    return new Modes(holds, new NormalGameProvider(rules.HeartTenTieBreaking));
  }
}
