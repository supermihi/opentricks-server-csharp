using System.Text.Json;
using System.Text.Json.Serialization;

namespace Doppelkopf.API;

public static class JsonConfiguration
{
  public static void SetupJsonOptions(JsonSerializerOptions options)
  {
    options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.Converters.Add(
      new JsonStringEnumConverter(
        namingPolicy: JsonNamingPolicy.CamelCase,
        allowIntegerValues: false));
  }

  public static readonly JsonSerializerOptions Options = CreateJsonOptions();

  public static JsonSerializerOptions CreateJsonOptions()
  {
    var result = new JsonSerializerOptions();
    SetupJsonOptions(result);
    return result;
  }
}
