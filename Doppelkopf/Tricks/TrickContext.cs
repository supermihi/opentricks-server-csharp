using Doppelkopf.Conf;
using Doppelkopf.Contracts;

namespace Doppelkopf.Tricks;

public sealed record TrickContext(ICardTraitsProvider CardTraits,
  TrickConfiguration TrickConfig,
  bool IsLastTrick);
