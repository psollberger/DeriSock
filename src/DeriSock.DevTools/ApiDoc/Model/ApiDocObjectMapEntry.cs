namespace DeriSock.DevTools.ApiDoc.Model;

using System;
using System.Text.Json.Serialization;

public class ApiDocObjectMapEntry
{
  [JsonPropertyName("description")]
  public string Description { get; set; } = string.Empty;

  [JsonPropertyName("properties")]
  public ApiDocPropertyCollection Properties { get; set; } = ApiDocPropertyCollection.Empty;

  [JsonPropertyName("methods")]
  public string[] Methods { get; set; } = Array.Empty<string>();

  [JsonPropertyName("subscriptions")]
  public string[] Subscriptions { get; set; } = Array.Empty<string>();
}
