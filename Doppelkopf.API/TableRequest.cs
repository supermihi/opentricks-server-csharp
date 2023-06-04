using System.Text.Json.Serialization;

namespace Doppelkopf.API;

public record TableRequest([property: JsonRequired] RequestType Type)
{

  public int? ExpectedVersion { get; init; }

  /// <summary>
  /// Whether the player has a reservation ("vorbehalt"); for <see cref="Type"/> equals
  /// <see cref="RequestType.Reserve"/>.
  /// </summary>
  public bool? IsReserved { get; init; }

  /// <summary>
  /// Declared contract ID; for <see cref="Type"/> equals <see cref="RequestType.Declare"/>.
  /// </summary>
  public string? ContractId { get; init; }

  /// <summary>
  /// ID of the card to play; for <see cref="Type"/> equals <see cref="RequestType.PlayCard"/>.
  /// </summary>
  public string? CardId { get; init; }
}
