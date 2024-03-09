namespace Doppelkopf.Core.Contracts;

public interface INormalGameProvider
{
  IContract CreateNormalGame(CardsByPlayer initialCards);
}
