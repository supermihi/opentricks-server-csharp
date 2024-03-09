using System.CommandLine;
using Doppelkopf.Bot;
using Doppelkopf.Cli;
using Doppelkopf.Core;
using Doppelkopf.Core.Tricks;


var rootCommand = new RootCommand("Doppelkopf client application");

rootCommand.SetHandler(
  async () =>
  {
    var conf = new GameConfiguration(new RuleOptions(TieBreakingMode.FirstWins, true));
    var host = new SingleGameHost(conf.CreateGameFactory(1234));
    host.AddBot(Player.One, new SimpleBot(Player.One));
    host.AddBot(Player.Two, new SimpleBot(Player.Two));
    host.AddBot(Player.Three, new SimpleBot(Player.Three));
    host.AddBot(Player.Four, new SimpleBot(Player.Four));
    await host.RunToCompletion();
  });

return await rootCommand.InvokeAsync(args);

