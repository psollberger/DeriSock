namespace DeriSock.Converter;

using System;
using System.Diagnostics;

using DeriSock.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Converts the 'object or string' data type
/// </summary>
public class ObjectOrStringConverter : JsonConverter<ObjectOrStringItem>
{
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, ObjectOrStringItem? value, JsonSerializer serializer)
  {
    Debug.Assert(value != null, nameof(value) + " != null");

    if (value!.ObjectValue != null)
      value.ObjectValue.WriteTo(writer);
    else if (!string.IsNullOrEmpty(value.StringValue))
      writer.WriteValue(value.StringValue);
    else
      writer.WriteNull();
  }

  /// <inheritdoc />
  public override ObjectOrStringItem ReadJson
  (
    JsonReader reader, Type objectType,
    ObjectOrStringItem? existingValue, bool hasExistingValue,
    JsonSerializer serializer
  )
  {
    var value = JToken.Load(reader);
    var result = new ObjectOrStringItem();

    switch (value.Type) {
      case JTokenType.String:
        result.StringValue = value.Value<string>();
        break;

      case JTokenType.Object:
        result.ObjectValue = JObject.Load(reader);
        break;
    }

    return result;
  }
}
