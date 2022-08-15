namespace DeriSock.Model;

using DeriSock.Converter;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Represents either an object or a string value
/// </summary>
[JsonConverter(typeof(ObjectOrStringConverter))]
public class ObjectOrStringItem
{
  /// <summary>
  ///   Is not null when the data is an object
  /// </summary>
  public JObject? ObjectValue { get; set; }

  /// <summary>
  ///   Is not null when the data is a string
  /// </summary>
  public string? StringValue { get; set; }
}
