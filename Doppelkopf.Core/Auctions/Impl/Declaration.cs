namespace Doppelkopf.Core.Auctions.Impl;

internal sealed record Declaration
{
    public static readonly Declaration Ok = new((IDeclarableContract?)null);

    public static Declaration FromContract(IDeclarableContract contract) => new(contract);

    private Declaration(IDeclarableContract? contract)
    {
        Contract = contract;
    }

    public bool IsHealthy => Contract == null;
    public IDeclarableContract? Contract { get; }
}
