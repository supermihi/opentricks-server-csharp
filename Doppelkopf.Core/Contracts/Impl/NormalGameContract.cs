using Doppelkopf.Core.Cards;
using Doppelkopf.Core.Scoring;
using Doppelkopf.Core.Tricks;

namespace Doppelkopf.Core.Contracts.Impl;

/// <summary>
/// Normal game contract, including all kinds of wedding (announced and silent).
/// </summary>
public class NormalGameContract : IContract
{
  private readonly ICardTraitsProvider _cardTraits;
  private readonly Player? _suitor;
  private Player? _spouse;
  private readonly IReadOnlyCollection<Player> _playersWithClubQueen;

  public NormalGameContract(TieBreakingMode heartsTenTieBreaking, Player? announcedSuitor, ICardsByPlayer
    initialCards)
  {
    _cardTraits = CardTraitsProvider.SuitSolo(Suit.Diamonds, heartsTenTieBreaking);
    _playersWithClubQueen =
      Enum.GetValues<Player>().Where(p => initialCards.GetCards(p).Contains(Card.ClubsQueen)).ToArray();
    if (_playersWithClubQueen.Count == 2)
    {
      if (announcedSuitor is not null)
      {
        throw new ArgumentException("announced suitor does not have both clubs queens", nameof(announcedSuitor));
      }

      Status = WeddingStatus.NoWedding;
    }
    else
    {
      _suitor = _playersWithClubQueen.Single();
      if (announcedSuitor is { } declarer)
      {
        if (declarer != _suitor)
        {
          throw new ArgumentException("announced suitor does not have both clubs queens", nameof(announcedSuitor));
        }

        Status = WeddingStatus.LookingForSpouse;
      }
      else
      {
        Status = WeddingStatus.SilentWedding;
      }
    }
  }

  private WeddingStatus Status { get; set; }
  public CardTraits GetTraits(Card card) => _cardTraits.GetTraits(card);

  public Party? GetParty(Player player) =>
    Status switch
    {
      WeddingStatus.NoWedding => _playersWithClubQueen.Contains(player) ? Party.Re : Party.Contra,
      WeddingStatus.Wedded => player == _suitor || player == _spouse ? Party.Re : Party.Contra,
      WeddingStatus.SilentWedding or WeddingStatus.NoSpouseFound => player == _suitor ? Party.Re : Party.Contra,
      WeddingStatus.LookingForSpouse => player == _suitor ? Party.Re : null,
      _ => throw new ArgumentException("unexpected Kind")
    };

  public int? DefiningTrick { get; private set; }

  public void OnTrickFinished(CompleteTrick trick)
  {
    if (Status != WeddingStatus.LookingForSpouse)
    {
      return;
    }

    const int maxTrickIndexToFindSpouse = 2;
    if (trick.Winner != _suitor)
    {
      DefiningTrick = trick.Index;
      _spouse = trick.Winner;
      Status = WeddingStatus.Wedded;
    }

    if (trick.Index == maxTrickIndexToFindSpouse)
    {
      DefiningTrick = trick.Index;
      Status = WeddingStatus.NoSpouseFound;
    }
  }
}
