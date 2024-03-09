using Doppelkopf.Core.Contracts;

namespace Doppelkopf.Core.Auctions;

public sealed record Declaration
{
  public static readonly Declaration Ok = new((IHold?)null);

  public static Declaration FromHold(IHold hold) => new(hold);

  private Declaration(IHold? hold) => Hold = hold;

  public bool IsHealthy => Hold == null;
  public IHold? Hold { get; }
}
