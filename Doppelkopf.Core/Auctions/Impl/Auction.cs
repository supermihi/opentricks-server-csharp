using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Auctions.Impl;

internal sealed record Auction : IAuction
{
  private readonly ICardsByPlayer _cards;
  private readonly IByPlayer<bool> _needsCompulsorySolo;
  private InTurns<Declaration> _state;
  private readonly IReadOnlyCollection<IDeclarableContract> _contracts;
  public static InTurns<Declaration> InitialState => new(Player.One);

  public Auction(ICardsByPlayer cards,
    IReadOnlyCollection<IDeclarableContract> contracts,
    IByPlayer<bool> needsCompulsorySolo,
    InTurns<Declaration> initialState)
  {
    _cards = cards;
    _contracts = contracts;
    _needsCompulsorySolo = needsCompulsorySolo;
    _state = initialState;
  }

  public Auction(ICardsByPlayer cards, IReadOnlyCollection<IDeclarableContract> contracts,
    IByPlayer<bool> needsCompulsorySolo)
      : this(cards, contracts, needsCompulsorySolo, InitialState)
  { }

  public Player? Turn => _state.Next;

  public void DeclareReservation(Player player, IDeclarableContract contract)
  {
    if (!_contracts.Contains(contract))
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

  public void DeclareOk(Player player)
  {
    _state = _state.AddChecked(player, Declaration.Ok);
  }

  public AuctionResult? Evaluate()
  {
    if (!_state.IsFull)
    {
      return null;
    }

    if (_state.All(d => d.IsHealthy))
    {
      return new(null, null, null);
    }
    var (winner, declaration) = _state.Items.MaxBy(t => Priority(t.item, t.player));
    var declaredContract = declaration.Contract!;
    return new(
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
