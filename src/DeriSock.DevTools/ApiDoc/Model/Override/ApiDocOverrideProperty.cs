namespace DeriSock.DevTools.ApiDoc.Model.Override;

using System.Text.Json.Serialization;

public class ApiDocOverrideProperty
{
  [JsonPropertyName("insertBefore")]
  public string? InsertBefore { get; set; } = null;
  
  [JsonPropertyName("insertLast")]
  public bool? InsertLast { get; set; } = null;

  [JsonPropertyName("name")]
  public string? Name { get; set; } = null;

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

  public ApiDocProperty CreateApiProperty()
  {
    var prop = new ApiDocProperty()
    {
      Name = Name ?? string.Empty,
      Description = Description,
      Deprecated = Deprecated ?? false,
      Required = Required ?? false,
      ApiDataType = ApiDataType,
      DataType = DataType,
      ArrayDataType = ArrayDataType,
      Converters = (string[]?)Converters?.Clone() ?? null,
      EnumValues = (string[]?)EnumValues?.Clone() ?? null,
      EnumIsSuggestion = EnumIsSuggestion,
      ValueLookupSource = ValueLookupSource,
      DefaultValue = DefaultValue,
      MaxLength = MaxLength
    };

    if (Properties is { Count: > 0 }) {
      prop.Properties = new ApiDocPropertyCollection(Properties.Count);

      foreach (var (key, value) in Properties) {
        prop.Properties.Add(key, value.CreateApiProperty());
      }
    }

    return prop;
  }
}
