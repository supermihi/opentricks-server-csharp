using Doppelkopf.Core.Contracts;
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

  public void DeclareReservation(Player player, IHold contract)
  {
    if (!_holds.Contains(contract))
    {
      ErrorCodes.ContractNotAvailable.Throw();
    }

    if (!contract.IsAllowed(_cards[player]))
    {
      ErrorCodes.ContractNotAllowed.Throw();
    }

    var declaration = Declaration.FromHold(contract);
    Declarations = Declarations.AddChecked(player, declaration);
  }

  public void DeclareOk(Player player) => Declarations = Declarations.AddChecked(player, Declaration.Ok);

  public AuctionResult? Evaluate()
  {
    if (!Declarations.IsFull)
    {
      return null;
    }

    if (Declarations.All(d => d.IsHealthy))
    {
      return new AuctionResult(null, null, false);
    }

    var (winner, declaration) = Declarations.Items.MaxBy(t => Priority(t.item, t.player));
    var declaredContract = declaration.Hold!;
    return new AuctionResult(
      declaredContract,
      winner,
      declaredContract.IsSolo && _needsCompulsorySolo[winner]);
  }

  private int Priority(Declaration declaration, Player player)
  {
    if (declaration.Hold is not { } contract)
    {
      return DeclarationPriority.Healthy;
    }

    var isCompulsorySolo = _needsCompulsorySolo[player] && contract.IsSolo;
    return contract.Priority.GetForCompulsorySolo(isCompulsorySolo);
  }
}
