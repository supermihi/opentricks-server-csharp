namespace Doppelkopf.Core.Auctions;

public sealed record AuctionResult
{
  public static AuctionResult NormalGame => new(true, null);

  public static AuctionResult Declared(Player declarer, IDeclarableContract contract) =>
      new(false, new Contract(declarer, contract));

  private AuctionResult(bool isNormalGame, Contract? contract)
  {
    IsNormalGame = isNormalGame;
    Contract = contract;
  }

  public bool IsNormalGame { get; }
  public Contract? Contract { get; }
}
