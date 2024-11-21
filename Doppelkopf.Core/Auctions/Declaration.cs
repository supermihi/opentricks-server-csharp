using System.Diagnostics.CodeAnalysis;

namespace Doppelkopf.Core.Auctions;

public sealed record Declaration
{
  public static readonly Declaration Fine = new(null);

  public static Declaration FromHold(string holdId) => new(holdId);

  private Declaration(string? holdId) => HoldId = holdId;
  [MemberNotNullWhen(returnValue: false, member: nameof(HoldId))]
  public bool IsFine => HoldId == null;
  public string? HoldId { get; }
  public static implicit operator Declaration(string holdId) => FromHold(holdId);
}
