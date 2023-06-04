// See https://aka.ms/new-console-template for more information

using System.CommandLine;
using Doppelkopf.Client.CLI;

var hostOption = new Option<string>(
  name: "--host",
  description: "the opentricks server host",
  getDefaultValue: () => "http://localhost:5142");

var userOption = new Option<string>(new[] { "-u", "--user" }, description: "user name" , getDefaultValue: () => "michael");
var rootCommand = new RootCommand("Doppelkopf client application");
rootCommand.AddOption(hostOption);
rootCommand.AddOption(userOption);

rootCommand.SetHandler(async (host, user) =>
  {
    var client = new DoppelkopfClient(host, user);
    var cli = new InteractiveClient(client);
    await cli.Run();
  },
  hostOption,
  userOption);

return await rootCommand.InvokeAsync(args);
