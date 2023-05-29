using Doppelkopf.Games;

namespace Doppelkopf.Server.Interface;

public static class JsonByPlayer
{
  public static JsonByPlayer<TJson> FromByPlayer<TJson, T>(ByPlayer<T> byPlayer, Func<Player, T, TJson> map)
  {
    return Create(p => map(p, byPlayer[p]));
  }

  public static JsonByPlayer<TJson> FromInTurns<TJson, T>(InTurns<T> inTurns, Func<Player, T, TJson> map,
    TJson defaultValue) where T : notnull
  {
    return Create(p => inTurns.TryGet(p, out var playerValue) ? map(p, playerValue) : defaultValue);
  }

  public static JsonByPlayer<T> Create<T>(Func<Player, T> factory)
  {
    return new(factory(Player.Player1), factory(Player.Player2), factory(Player.Player3), factory(Player.Player4));
  }
}

public sealed record JsonGame(JsonByPlayer<IReadOnlyList<string>> Cards, JsonAuction? Auction,
  string? Contract, JsonTricks? Tricks)
{
  public static JsonGame FromGame(Game game, Player? maskFor)
  {
    var cards = JsonByPlayer.FromByPlayer(
      game.Cards,
      (player, cards) => player == maskFor
          ? (IReadOnlyList<string>)cards.Select(c => c.Id).ToArray()
          : new string[] { });
    var auction = game.TrickTaking is null ? JsonAuction.FromAuction(game.Auction, maskFor) : null;
    var contract = game.TrickTaking?.Contract.Id;
    var tricks = game.TrickTaking is null or { IsFinished: true } ? null : JsonTricks.FromTrickTaking(game.TrickTaking);
    return new(cards, auction, contract, tricks);
  }
}
