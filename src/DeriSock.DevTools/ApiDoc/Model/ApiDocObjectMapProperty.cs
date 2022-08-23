namespace DeriSock.DevTools.ApiDoc.Model;

using System.Text.Json.Serialization;

public class ApiDocObjectMapProperty
{
  /// <summary>
  ///   The description of the parameter or response property.
  /// </summary>
  [JsonPropertyName("description")]
  public string? Description { get; set; } = null;

  /// <summary>
  ///   Gets if the argument is required for the request or the property is always in the response
  /// </summary>
  [JsonPropertyName("required")]
  public bool? Required { get; set; } = null;

  [JsonIgnore]
  public bool HasValue => Description != null || Required != null;
}
