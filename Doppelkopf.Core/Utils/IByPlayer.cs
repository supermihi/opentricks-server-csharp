namespace Doppelkopf.Core.Utils;

public interface IByPlayer<out T> : IEnumerable<T>
{
  T this[Player p] { get; }
}
