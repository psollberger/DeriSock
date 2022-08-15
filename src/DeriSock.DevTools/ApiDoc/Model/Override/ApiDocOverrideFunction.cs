namespace DeriSock.DevTools.ApiDoc.Model.Override;

using System.Text.Json.Serialization;

public class ApiDocOverrideFunction
{
  [JsonPropertyName("category")]
  public string? Category { get; set; } = null;

  [JsonPropertyName("name")]
  public string? Name { get; set; } = null;

  [JsonPropertyName("description")]
  public string? Description { get; set; } = null;

  [JsonPropertyName("excludeInInterface")]
  public bool? ExcludeInInterface { get; set; } = null;

  [JsonPropertyName("isSynchronous")]
  public bool? IsSynchronous { get; set; }

  [JsonPropertyName("deprecated")]
  public bool? Deprecated { get; set; } = default;

  [JsonPropertyName("request")]
  public ApiDocOverrideProperty? Request { get; set; } = default;

  [JsonPropertyName("response")]
  public ApiDocOverrideProperty? Response { get; set; } = default;
}
