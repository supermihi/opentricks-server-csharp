using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Games;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Auctions.Impl;

internal sealed record Auction : IAuction
{
  private readonly CardsByPlayer _cards;
  private readonly ByPlayer<bool> _needsCompulsorySolo;
  public InTurns<Declaration> Declarations { get; private set; }
  private readonly IReadOnlyCollection<IHold> _holds;
  public static InTurns<Declaration> InitialState => new(Player.One);

  public Auction(CardsByPlayer cards,
    IReadOnlyCollection<IHold> holds,
    ByPlayer<bool> needsCompulsorySolo,
    InTurns<Declaration> initialDeclarations)
  {
    _cards = cards;
    _holds = holds;
    _needsCompulsorySolo = needsCompulsorySolo;
    Declarations = initialDeclarations;
  }

  public Auction(CardsByPlayer cards, IReadOnlyCollection<IHold> holds,
    ByPlayer<bool> needsCompulsorySolo)
    : this(cards, holds, needsCompulsorySolo, InitialState)
  {
  }

  public Player? Turn => Declarations.Next;

  public void Declare(Player player, Declaration declaration)
  {
    if (!declaration.IsFine)
    {
      CheckHold(player, declaration.HoldId);
    }
    Declarations = Declarations.AddChecked(player, declaration);
  }

  private IHold GetHoldChecked(string holdId)
  {
    var hold = _holds.FirstOrDefault(h => h.Id == holdId);
    if (hold is null)
    {
      ErrorCodes.ContractNotAvailable.Throw();
    }
    return hold;
  }

  private void CheckHold(Player player, string holdId)
  {
    var hold = GetHoldChecked(holdId);
    if (!hold.IsAllowed(_cards[player]))
    {
      ErrorCodes.ContractNotAllowed.Throw();
    }
  }

  public AuctionResult? Evaluate()
  {
    if (!Declarations.IsFull)
    {
      return null;
    }

    if (Declarations.All(d => d.IsFine))
    {
      return new AuctionResult(null, null, false);
    }

    var (winner, declaration) = Declarations.Items.MaxBy(t => Priority(t.item, t.player));
    var winningHoldId = declaration.HoldId!;
    var winningHold = GetHoldChecked(winningHoldId);
    return new AuctionResult(
      winningHold,
      winner,
      winningHold.IsSolo && _needsCompulsorySolo[winner]);
  }

  private int Priority(Declaration declaration, Player player)
  {
    if (declaration.HoldId is not { } holdId)
    {
      return DeclarationPriority.Fine;
    }
    var hold = GetHoldChecked(holdId);
    var isCompulsorySolo = _needsCompulsorySolo[player] && hold.IsSolo;
    return hold.Priority.GetForCompulsorySolo(isCompulsorySolo);
  }
}
