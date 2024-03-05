using Doppelkopf.Core.Contracts;

namespace Doppelkopf.Core.Auctions.Impl;

internal sealed record Declaration
{
  public static readonly Declaration Ok = new((IHold?)null);

  public static Declaration FromContract(IHold contract) => new(contract);

  private Declaration(IHold? contract) => Contract = contract;

  public bool IsHealthy => Contract == null;
  public IHold? Contract { get; }
}
