using System.Collections;

namespace Doppelkopf.Core.Scoring.Impl;

public sealed record ByParty<T>(T Re, T Contra) : IEnumerable<T>
{
  public T GetValue(Party p) => p == Party.Re ? Re : Contra;

  public ByParty(Func<Party, T> init) : this(init(Party.Re), init(Party.Contra))
  { }

  public IEnumerator<T> GetEnumerator()
  {
    var enumerable = new[] { Re, Contra };
    return ((IEnumerable<T>)enumerable).GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

public static class ByParty
{
  public static ByParty<T> New<T>(T re, T contra) => new(re, contra);
  public static ByParty<T> Init<T>(Func<Party, T> init) => new(init(Party.Re), init(Party.Contra));
  public static ByParty<T> Both<T>(T value) => new(value, value);
}
