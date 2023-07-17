using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts;

public interface IContract
{
  ICardTraitsProvider CardTraits { get; }
  IPartyProvider CreatePartyProvider(Player? declarer, ICardsByPlayer initialCards);
  IReadOnlyList<IExtraPointRule> ExtraPointRules { get; }
  ContractType Type { get; }
}
