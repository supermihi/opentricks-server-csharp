using Doppelkopf.Core.Utils;
using Doppelkopf.Errors;

namespace Doppelkopf.Core.Auctions.Impl;

internal sealed record Auction : IAuction
{
    private readonly ICardsByPlayer _cards;
    private readonly IByPlayer<bool> _needsCompulsorySolo;
    private InTurns<Declaration> _state;
    public static InTurns<Declaration> InitialState => new(Player.One);

    public Auction(
        ICardsByPlayer cards,
        IByPlayer<bool> needsCompulsorySolo,
        InTurns<Declaration> initialState
    )
    {
        _cards = cards;
        _needsCompulsorySolo = needsCompulsorySolo;
        _state = initialState;
    }

    public Auction(ICardsByPlayer cards, IByPlayer<bool> needsCompulsorySolo)
        : this(cards, needsCompulsorySolo, InitialState) { }

    public Player? Turn => _state.Next;

    public void DeclareReservation(Player player, IDeclarableContract contract)
    {
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
            return AuctionResult.NormalGame;
        }
        var (winner, declaration) = _state.Items.MaxBy(t => Score(t.item, t.player));
        return AuctionResult.Declared(winner, declaration.Contract!);
    }

    private int Score(Declaration declaration, Player player) =>
        declaration.Contract switch
        {
            null => 0,
            { Type: DeclaredContractType.Poverty } => 1,
            { Type: DeclaredContractType.Marriage } => 2,
            { Type: DeclaredContractType.Solo } when !_needsCompulsorySolo[player] => 3,
            { Type: DeclaredContractType.Solo } when _needsCompulsorySolo[player] => 4,
            _
                => throw new ArgumentOutOfRangeException(
                    nameof(declaration),
                    "unsupported declaration type"
                )
        };
}
