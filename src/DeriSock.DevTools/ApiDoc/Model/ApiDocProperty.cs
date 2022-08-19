namespace DeriSock.DevTools.ApiDoc.Model;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

/// <summary>
///   For the lack of a better name, this is called <see cref="ApiDocProperty" />.
///   It represents a method request/response parameter/property or
///   a subscription registration parameter / notification property
/// </summary>
public class ApiDocProperty : IApiDocPropertyNode
{
  [JsonIgnore]
  public IApiDocPropertyNode? Parent { get; private set; }

  [JsonIgnore]
  public ApiDocFunctionType FunctionType { get; set; } = ApiDocFunctionType.Undefined;

  /// <summary>
  ///   <para>
  ///     The name of the parameter or response property.
  ///   </para>
  ///   <para>
  ///     If this value is empty on a response, it means the result property itself holds the value, without any further properties.<br />
  ///     In this case, the <see cref="Properties" /> property MUST be empty.
  ///   </para>
  /// </summary>
  [JsonIgnore]
  public string Name { get; set; } = string.Empty;

  /// <summary>
  ///   The description of the parameter or response property.
  /// </summary>
  [JsonPropertyName("description")]
  public string? Description { get; set; } = null;

  /// <summary>
  ///   Gets, if this property is deprecated.
  /// </summary>
  [JsonPropertyName("deprecated")]
  public bool Deprecated { get; set; } = default;

  /// <summary>
  ///   Defines if the request parameter is required or if the response property is optional
  /// </summary>
  [JsonPropertyName("required")]
  public bool Required { get; set; } = default;

  /// <summary>
  ///   The data type of the property as defined in the API documentation.
  /// </summary>
  [JsonPropertyName("apiDataType")]
  public string? ApiDataType { get; set; } = null;

  /// <summary>
  ///   Normalized ApiDataType
  /// </summary>
  [JsonPropertyName("dataType")]
  public string? DataType { get; set; } = null;

  /// <summary>
  ///   If this property is an array, this defines the type within that array
  /// </summary>
  [JsonPropertyName("arrayDataType")]
  public string? ArrayDataType { get; set; } = null;

  /// <summary>
  ///   Defines JSON converters to be used for this property
  /// </summary>
  [JsonPropertyName("converters")]
  public string[]? Converters { get; set; } = null;

  /// <summary>
  ///   Only set in request parameters. Defines the possible enumeration values.
  /// </summary>
  [JsonPropertyName("enumValues")]
  public string[]? EnumValues { get; set; } = default;

  /// <summary>
  ///   If the enum is only a suggestion but not binding (i.e. the property can also take other values)
  /// </summary>
  [JsonPropertyName("enumIsSuggestion")]
  public bool? EnumIsSuggestion { get; set; } = default;

  /// <summary>
  ///   Defines the method that can be used to retrieve the valid values for this property
  /// </summary>
  [JsonPropertyName("valueLookupSource")]
  public string? ValueLookupSource { get; set; } = null;

  /// <summary>
  ///   Default value for a request parameter
  /// </summary>
  [JsonPropertyName("defaultValue")]
  public object? DefaultValue { get; set; } = default;

  /// <summary>
  ///   Maximum length of the value
  /// </summary>
  [JsonPropertyName("maxLength")]
  public int? MaxLength { get; set; } = default;

  /// <summary>
  ///   Contains nested properties, should there be any.
  /// </summary>
  [JsonPropertyName("properties")]
  public ApiDocPropertyCollection? Properties { get; set; } = default;

  /// <summary>
  ///   Gets, if any of the properties contained in this property is required
  /// </summary>
  [JsonIgnore]
  public bool IsAnyPropertyRequired
  {
    get
    {
      if (Properties == null)
        return Required;

      if (Required)
        return true;

      if (Properties.Count < 1)
        return false;

      return Properties.Any(x => x.Value.Required);
    }
  }

  [JsonIgnore]
  public bool IsObject => DataType is "object" || ArrayDataType is "object";

  [JsonIgnore]
  public bool IsArray => DataType is "array";

  public void Accept(IApiDocDocumentVisitor visitor)
  {
    visitor.VisitProperty(this);

    if (Properties != null)
      foreach (var (_, value) in Properties)
        value.Accept(visitor);
  }

  public void UpdateRelations(IApiDocPropertyNode? parent)
  {
    Parent = parent;

    if (Parent != null)
      FunctionType = Parent.FunctionType;

    if (Properties != null)
      foreach (var (key, value) in Properties) {
        value.Name = key;
        value.UpdateRelations(this);
      }
  }

  public string ComputePropertyIdHash()
  {
    var sha1 = SHA1.Create();

    if (!(Properties is { Count: > 0 }))
      return Convert.ToHexString(sha1.ComputeHash(Array.Empty<byte>()));

    var sb = new StringBuilder(Properties.Count * 30);

    foreach (var (_, property) in Properties) {
      sb.Append(property.Name);
      sb.Append(property.DataType);
      sb.Append(property.ArrayDataType);
      sb.Append(property.Required);
      sb.Append(property.Deprecated);
      sb.Append(string.Join(',', property.EnumValues ?? Array.Empty<string>()));
      sb.Append(property.EnumIsSuggestion ?? false);

      if (property.Properties is {Count: > 0} && (property.DataType == "object" || property.ArrayDataType == "object")) {
        foreach (var (_, subProperty) in property.Properties) {
          sb.Append(subProperty.ComputePropertyIdHash());
        }
      }
    }

    return Convert.ToHexString(sha1.ComputeHash(Encoding.UTF8.GetBytes(sb.ToString())));
  }
}
