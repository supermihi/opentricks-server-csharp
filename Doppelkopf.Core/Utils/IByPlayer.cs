namespace Doppelkopf.Core.Utils;

public interface IByPlayer<out T>
{
  T this[Player p] { get; }
}
