namespace DeriSock.DevTools.ApiDoc.Model.Override;

using System.Text.Json.Serialization;

public class ApiDocOverrideDocument
{
  [JsonPropertyName("methods")]
  public ApiDocOverrideFunctionCollection? Methods { get; set; }

  [JsonPropertyName("subscriptions")]
  public ApiDocOverrideFunctionCollection? Subscriptions { get; set; }
}
