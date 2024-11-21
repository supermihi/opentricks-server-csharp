namespace Doppelkopf.API;

/// <summary>
/// Built-in hold IDs.
/// </summary>
public static class HoldIds
{
  public const string Wedding = "wedding";
  public const string JackSolo = "jack_solo";
  public const string QueenSolo = "queen_solo";
  public const string FleshlessSolo = "fleshless_solo";
  public const string DiamondsSolo = "diamonds_solo";
  public const string HeartsSolo = "hearts_solo";
  public const string SpadesSolo = "spades_solo";
  public const string ClubsSolo = "clubs_solo";

  public static string SuitSolo(Suit s) =>
    s switch
    {
      Suit.Diamonds => DiamondsSolo,
      Suit.Hearts => HeartsSolo,
      Suit.Spades => SpadesSolo,
      Suit.Clubs => ClubsSolo,
      _ => throw new ArgumentException("invalid suit", nameof(s))
    };
}
