using System.Collections;

namespace Doppelkopf.Contracts;

public sealed record AvailableContracts(
  IContract NormalGame,
  IContract Marriage,
  IReadOnlyCollection<IContract> Solos,
  IReadOnlyCollection<IContract> Specials
) : IEnumerable<IContract>
{
    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public IEnumerator<IContract> GetEnumerator()
    {
        return new[] { NormalGame, Marriage }.Concat(Solos).Concat(Specials).GetEnumerator();
    }
}
