namespace Doppelkopf.Core.Contracts;

public interface INormalGameProvider
{
  IContract CreateNormalGame(ICardsByPlayer initialCards);
}
