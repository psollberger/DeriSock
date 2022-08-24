namespace DeriSock.DevTools.ApiDoc.Model;

using System.Text.Json.Serialization;

public class ApiDocObjectMapEntry
{
  [JsonPropertyName("hash")]
  public string Hash { get; set; } = string.Empty;

  [JsonPropertyName("description")]
  public string? Description { get; set; }

  [JsonPropertyName("properties")]
  public ApiDocObjectMapPropertyCollection? Properties { get; set; }

  [JsonPropertyName("methods")]
  public string[]? MethodPaths { get; set; }

  [JsonPropertyName("subscriptions")]
  public string[]? SubscriptionPaths { get; set; }

  public string? GetFirstSourcePath()
  {
    if (MethodPaths is { Length: > 0 })
      return MethodPaths[0];

    if (SubscriptionPaths is { Length: > 0 })
      return SubscriptionPaths[0];

    return null;
  }
}
