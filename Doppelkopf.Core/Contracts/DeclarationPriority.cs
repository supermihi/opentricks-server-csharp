namespace Doppelkopf.Core.Contracts;

public readonly record struct DeclarationPriority(int Default, int AsCompulsorySolo)
{
  public DeclarationPriority(int defaultAndAsCompulsorySolo) : this(
    defaultAndAsCompulsorySolo,
    defaultAndAsCompulsorySolo)
  {
  }

  public const int Healthy = 0;
  public const int Wedding = 10;
  public const int Solo = 20;
  public const int CompulsorySolo = 30;

  public int GetForCompulsorySolo(bool isCompulsorySolo) => isCompulsorySolo ? AsCompulsorySolo : Default;
}
