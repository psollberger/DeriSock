namespace DeriSock.DevTools.ApiDoc.Model;

using System;
using System.Text.Json.Serialization;

/// <summary>
///   For the lack of a better name, this is called <see cref="ApiDocProperty" />.
///   It represents a method request/response parameter/property or
///   a subscription registration parameter / notification property
/// </summary>
public class ApiDocProperty : IApiDocPropertyNode, IEquatable<ApiDocProperty>
{
  [JsonIgnore]
  public IApiDocPropertyNode? Parent { get; private set; }

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
  ///   If the normal name of the property conflicts with code generation, this field defines an alternative name to use
  /// </summary>
  [JsonPropertyName("codeName")]
  public string? CodeName { get; set; } = null;

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

  public void UpdateParent(IApiDocPropertyNode? parent)
  {
    Parent = parent;

    if (Properties != null)
      foreach (var (key, value) in Properties) {
        value.Name = key;
        value.UpdateParent(this);
      }
  }

  public bool Equals(ApiDocProperty? other)
  {
    if (ReferenceEquals(null, other))
      return false;

    if (ReferenceEquals(this, other))
      return true;

    return Name == other.Name && ApiDataType == other.ApiDataType;
  }

  public override bool Equals(object? obj)
    => ReferenceEquals(this, obj) || obj is ApiDocProperty other && Equals(other);

  public override int GetHashCode()
    => HashCode.Combine(Name, ApiDataType);

  public static bool operator ==(ApiDocProperty? left, ApiDocProperty? right)
    => Equals(left, right);

  public static bool operator !=(ApiDocProperty? left, ApiDocProperty? right)
    => !Equals(left, right);
}
