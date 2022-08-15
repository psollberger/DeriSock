namespace DeriSock.DevTools.ApiDoc.Model;

using System;
using System.Text.Json.Serialization;

public class ApiDocEnumMapEntry
{
  [JsonPropertyName("description")]
  public string Description { get; set; } = string.Empty;

  [JsonPropertyName("enumValues")]
  public string[] EnumValues { get; set; } = Array.Empty<string>();

  [JsonPropertyName("methods")]
  public string[] Methods { get; set; } = Array.Empty<string>();

  [JsonPropertyName("subscriptions")]
  public string[] Subscriptions { get; set; } = Array.Empty<string>();
}
