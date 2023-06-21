namespace Doppelkopf.API;

public enum ContractType
{
  NormalGame,
  Marriage,
  DiamondsSolo,
  HeartsSolo,
  SpadesSolo,
  ClubsSolo,
  MeatFree,
  JacksSolo,
  QueensSolo
}

/// <summary>
///
/// </summary>
/// <param name="Announcer">User ID of the solo player (for solo) or announcer (for marriage)</param>
public record Contract(string? Announcer, ContractType Type);

/// <summary>
///
/// </summary>
/// <param name="Type"></param>
/// <param name="Actor">
///   Id of the user that initiated the action. Is <c>null</c> if the game itself is the "actor".
/// </param>
public record Event(EventType Type, string? Actor = null, string? Card = null, Contract? Contract = null,
  bool? Reserved = null,
  bool? SessionFinished = null);

public record Notification(TableState State, IReadOnlyList<Event> Events, DateTime Timestamp);
