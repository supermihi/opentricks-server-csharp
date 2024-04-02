using System.Collections;
using Doppelkopf.Core.Utils;

namespace Doppelkopf.Core.Scoring.Impl;

public sealed record ByParty<T>(T Re, T Contra) : IEnumerable<T>
{
  public T GetValue(Party p) => p == Party.Re ? Re : Contra;
  public T this[Party p] => GetValue(p);

  public ByParty(Func<Party, T> init) : this(init(Party.Re), init(Party.Contra))
  {
  }

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
  public static ByParty<T> Init<T>(Func<Party, T> init) => new(init);
  public static ByParty<T> Both<T>(T value) => new(value, value);

  public static ByParty<int> Add(this ByParty<int> a, ByParty<int> b) => Init(p => a[p] + b[p]);

  public static Player? Soloist(this ByPlayer<Party> parties)
  {
    var rePlayers = parties.Items.Where(i => i.value == Party.Re).ToArray();
    return rePlayers.Length == 1 ? rePlayers[0].player : null;
  }
}
