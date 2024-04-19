using System.CommandLine;
using Doppelkopf.Bot;
using Doppelkopf.Cli;
using Doppelkopf.Core;


var rootCommand = new RootCommand("Doppelkopf client application");

rootCommand.SetHandler(
  async () =>
  {
    var conf = GameConfiguration.Default();
    var host = new SingleGameHost(conf.CreateGameFactory(12434));
    var cli = new HumanPlayerCli(Player.One);
    host.AddInteractiveClient(Player.One, cli);
    host.AddBot(Player.Four, new SimpleBot(Player.Four));
    host.AddBot(Player.Two, new SimpleBot(Player.Two));
    host.AddBot(Player.Three, new SimpleBot(Player.Three));


    var cliTask = Task.Run(() => cli.Run(CancellationToken.None));
    await Task.WhenAny(host.RunToCompletion(CancellationToken.None), cliTask);
    Console.WriteLine("done");
  });

return await rootCommand.InvokeAsync(args);
