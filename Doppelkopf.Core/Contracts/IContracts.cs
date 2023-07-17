namespace Doppelkopf.Core.Contracts;

public record AvailableContracts(IContract NormalGame, IReadOnlyCollection<IDeclarableContract> DeclarableContracts)
{
  public AvailableContracts(IContract normalGame, params IDeclarableContract[] declarableContracts) : this(
    normalGame,
    (IReadOnlyCollection<IDeclarableContract>)declarableContracts)
  { }
}
