namespace Doppelkopf.Core.Contracts;

public readonly record struct DeclarationPriority(int Default, int AsCompulsorySolo)
{
  public static DeclarationPriority NoSolo(int priority) => new(priority, priority);
  public const int Healthy = 0;
  public const int Wedding = 10;
  public const int Solo = 20;
  public const int CompulsorySolo = 30;
}
