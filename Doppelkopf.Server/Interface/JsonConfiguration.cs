using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace Doppelkopf.Server.Interface;

public static class JsonConfiguration
{
  public static void SetupJsonOptions(JsonOptions options)
  {
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(namingPolicy: JsonNamingPolicy.CamelCase,
      allowIntegerValues: false));
  }
}
