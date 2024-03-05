using Doppelkopf.Core.Contracts;
using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Auctions.Impl;

internal sealed record Auction : IAuction
{
  private readonly ICardsByPlayer _cards;
  private readonly IByPlayer<bool> _needsCompulsorySolo;
  private InTurns<Declaration> _state;
  private readonly IReadOnlyCollection<IHold> _holds;
  public static InTurns<Declaration> InitialState => new(Player.One);

  public Auction(ICardsByPlayer cards,
    IReadOnlyCollection<IHold> holds,
    IByPlayer<bool> needsCompulsorySolo,
    InTurns<Declaration> initialState)
  {
    _cards = cards;
    _holds = holds;
    _needsCompulsorySolo = needsCompulsorySolo;
    _state = initialState;
  }

  public Auction(ICardsByPlayer cards, IReadOnlyCollection<IHold> holds,
    IByPlayer<bool> needsCompulsorySolo)
    : this(cards, holds, needsCompulsorySolo, InitialState)
  {
  }

  public Player? Turn => _state.Next;

  public void DeclareReservation(Player player, IHold contract)
  {
    if (!_holds.Contains(contract))
    {
      ErrorCodes.ContractNotAvailable.Throw();
    }

    if (!contract.IsAllowed(_cards.GetCards(player)))
    {
      ErrorCodes.ContractNotAllowed.Throw();
    }

    var declaration = Declaration.FromContract(contract);
    _state = _state.AddChecked(player, declaration);
  }

  public void DeclareOk(Player player) => _state = _state.AddChecked(player, Declaration.Ok);

  public AuctionResult? Evaluate()
  {
    if (!_state.IsFull)
    {
      return null;
    }

    if (_state.All(d => d.IsHealthy))
    {
      return new AuctionResult(null, null, null);
    }

    var (winner, declaration) = _state.Items.MaxBy(t => Priority(t.item, t.player));
    var declaredContract = declaration.Contract!;
    return new AuctionResult(
      declaredContract,
      winner,
      declaredContract.IsSolo ? _needsCompulsorySolo[winner] : null);
  }

  private int Priority(Declaration declaration, Player player)
  {
    if (declaration.Contract is not { } contract)
    {
      return DeclarationPriority.Healthy;
    }

    var (defaultPrio, compulsorySoloPrio) = contract.Priority;
    return _needsCompulsorySolo[player] && contract.IsSolo ? compulsorySoloPrio : defaultPrio;
  }
}
