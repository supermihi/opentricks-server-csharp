using System.Collections.Immutable;

namespace Doppelkopf.Server.Bots;

public record BotIds(IReadOnlyCollection<UserId> Bots)
{
  public static BotIds Default = new(ImmutableArray.Create<UserId>(new("bot1"), new("bot2"), new("bot3"), new("bot4")));

  public bool IsBot(UserId user) => Bots.Contains(user);
}
