namespace DeriSock.DevTools.ApiDoc.Model;

using System.Text.Json.Serialization;

public class ApiDocObjectMapProperty
{
  /// <summary>
  ///   The description of the parameter or response property.
  /// </summary>
  [JsonPropertyName("description")]
  public string? Description { get; set; } = null;
}
