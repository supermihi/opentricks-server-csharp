using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Doppelkopf.API;

namespace Doppelkopf.Client.CLI;

public class DoppelkopfClient
{
  private readonly string _host;
  public string User { get; }
  private readonly HttpClient _httpClient;
  private readonly HttpClientHandler _handler;

  public DoppelkopfClient(string host, string user)
  {
    _host = host;
    User = user;
    _handler = new HttpClientHandler();
    _handler.CookieContainer = new CookieContainer();
    _httpClient = new HttpClient(_handler);
  }

  public async Task<TableState> CreateTable(CreateTableRequest request)
  {
    var response = await Post("/table", request);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<TableState>(JsonConfiguration.Options);
    return result!;
  }

  public async Task<IReadOnlyList<TableState>> ListTables()
  {
    var response = await _httpClient.GetAsync($"{_host}/table");
    response.EnsureSuccessStatusCode();
    var content = await response.Content.ReadAsStringAsync();
    var result = JsonSerializer.Deserialize<IReadOnlyList<TableState>>(content, JsonConfiguration.Options);
    return result!;
  }

  public async Task Login()
  {
    var response = await Post("/login", new LoginRequest(User, User, "secret"));
    response.EnsureSuccessStatusCode();
    Console.WriteLine("logged in");
  }

  private Task<HttpResponseMessage> Post<T>(string path, T payload) =>
      _httpClient.PostAsJsonAsync($"{_host}{path}", payload, JsonConfiguration.Options);

  private Task<HttpResponseMessage> Patch<T>(string path, T payload) =>
      _httpClient.PatchAsJsonAsync($"{_host}{path}", payload, JsonConfiguration.Options);

  private Task<HttpResponseMessage> Get(string path) => _httpClient.GetAsync($"{_host}{path}");

  public async Task<TableState> GetTable(string tableId)
  {
    var table = await Get($"/table/{tableId}");
    return (await table.Content.ReadFromJsonAsync<TableState>(JsonConfiguration.Options))!;
  }

  public async Task<TableState> Act(string tableId, TableRequest request)
  {
    var response = await Patch($"/table/{tableId}", request);
    response.EnsureSuccessStatusCode();
    var result = await response.Content.ReadFromJsonAsync<Notification>(JsonConfiguration.Options);
    return result!.State;
  }

  public async Task SubscribeToUpdates()
  {
    var response = await _httpClient.GetStreamAsync($"{_host}/updates");
    var buffer = new byte[1024 * 1024];
    while (true)
    {
      var result = await response.ReadAsync(buffer);
      if (result == 0)
      {
        Console.WriteLine("stream ended");
      }
      var not = JsonSerializer.Deserialize<Notification>(buffer[..result], JsonConfiguration.Options);
      Console.WriteLine($"received notification {not}");
    }
  }
}
