using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

public class NormalGameProvider(TieBreakingMode heartTenTieBreaking) : INormalGameProvider
{
  public IContract CreateNormalGame(ICardsByPlayer initialCards) =>
    new NormalGameContract(heartTenTieBreaking, null, initialCards);
}
