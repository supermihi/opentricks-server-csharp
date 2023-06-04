using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Doppelkopf.API;

namespace Doppelkopf.Client.CLI;

public class DoppelkopfClient
{
  private readonly string _host;
  private readonly string _user;
  private readonly HttpClient _httpClient;
  private readonly HttpClientHandler _handler;

  public DoppelkopfClient(string host, string user)
  {
    _host = host;
    _user = user;
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
    var response = await Post("/login", new LoginRequest(_user, _user, "secret"));
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

  public async Task<JsonElement> Act(string tableId, TableRequest request)
  {
    var result = await Patch($"/table/{tableId}", request);
    result.EnsureSuccessStatusCode();
    return await result.Content.ReadFromJsonAsync<JsonElement>(JsonConfiguration.Options);
  }
}
