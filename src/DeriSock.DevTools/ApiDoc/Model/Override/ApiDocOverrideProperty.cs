namespace DeriSock.DevTools.ApiDoc.Model.Override;

using System.Text.Json.Serialization;

public class ApiDocOverrideProperty
{
  [JsonPropertyName("name")]
  public string? Name { get; set; } = null;

  [JsonPropertyName("codeName")]
  public string? CodeName { get; set; } = null;

  [JsonPropertyName("description")]
  public string? Description { get; set; } = null;

  [JsonPropertyName("deprecated")]
  public bool? Deprecated { get; set; } = default;

  [JsonPropertyName("required")]
  public bool? Required { get; set; } = default;

  [JsonPropertyName("apiDataType")]
  public string? ApiDataType { get; set; } = null;

  [JsonPropertyName("dataType")]
  public string? DataType { get; set; } = null;

  [JsonPropertyName("arrayDataType")]
  public string? ArrayDataType { get; set; } = null;

  [JsonPropertyName("converters")]
  public string[]? Converters { get; set; } = null;

  [JsonPropertyName("enumValues")]
  public string[]? EnumValues { get; set; } = default;

  [JsonPropertyName("enumIsSuggestion")]
  public bool? EnumIsSuggestion { get; set; } = default;

  [JsonPropertyName("valueLookupSource")]
  public string? ValueLookupSource { get; set; } = null;

  [JsonPropertyName("defaultValue")]
  public object? DefaultValue { get; set; } = null;

  [JsonPropertyName("maxLength")]
  public int? MaxLength { get; set; } = default;

  [JsonPropertyName("properties")]
  public ApiDocOverridePropertyCollection? Properties { get; set; } = default;
}
