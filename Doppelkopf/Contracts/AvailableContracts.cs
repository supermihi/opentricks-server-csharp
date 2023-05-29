using System.Collections;

namespace Doppelkopf.Contracts;

public sealed record AvailableContracts(IContract NormalGame,
  IContract Marriage,
  IReadOnlyCollection<IContract> Solos,
  IReadOnlyCollection<IContract> Specials) : IEnumerable<IContract>
{
  IEnumerator IEnumerable.GetEnumerator()
  {
    return GetEnumerator();
  }

  public IEnumerator<IContract> GetEnumerator()
  {
    return new[] { NormalGame, Marriage }.Concat(Solos).Concat(Specials).GetEnumerator();
  }

  public static readonly AvailableContracts Default = new(
    new NormalGame("contract_normal_game"),
    new Marriage("contract_marriage"),
    new[]
    {
      new Solo("contract_diamonds_solo", CardTraitsProvider.DiamondsTrump),
      new Solo("contract_hearts_solo", CardTraitsProvider.HeartsTrump),
      new Solo("contract_spades_solo", CardTraitsProvider.SpadesTrump),
      new Solo("contract_clubs_solo", CardTraitsProvider.ClubsTrump),
      new Solo("contract_meatfree", CardTraitsProvider.NoTrump),
      new Solo("contract_jacks_solo", CardTraitsProvider.JacksTrump),
      new Solo("contract_queens_solo", CardTraitsProvider.QueensTrump)
    },
    new IContract[] { }
  );
}
