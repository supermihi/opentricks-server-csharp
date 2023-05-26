using System.Text.Json.Serialization;
using Doppelkopf.Cards;
using Doppelkopf.Sessions;

namespace Doppelkopf.Server.Notifications;

public record PlayCardUpdate([property: JsonPropertyName("card")]
  Card Card,
  [property: JsonPropertyName("seat")]
  Seat Seat) : IUserNotification;
