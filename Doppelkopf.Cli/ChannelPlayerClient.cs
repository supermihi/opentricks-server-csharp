using System.Threading.Channels;
using Doppelkopf.API;
using Doppelkopf.Core;
using Doppelkopf.Core.Games;
using Doppelkopf.Errors;

namespace Doppelkopf.Cli;

internal sealed class ChannelPlayerClient(Player Player, ChannelWriter<(Player, PlayerAction)> Channel) : IPlayerClient
{
  public async Task<ErrorCode?> Play(PlayerAction action)
  {
    await Channel.WriteAsync((Player, action));
    return null;
  }
}
