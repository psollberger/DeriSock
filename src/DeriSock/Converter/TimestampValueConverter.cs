namespace DeriSock.Converter;

using System;
using System.Diagnostics;

using DeriSock.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Converts an array of [timestamp, value]
/// </summary>
public class TimestampValueConverter : JsonConverter<TimestampValueItem>
{
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, TimestampValueItem? value, JsonSerializer serializer)
  {
    Debug.Assert(value != null, nameof(value) + " != null");
    writer.WriteStartArray();
    writer.WriteValue(value.Timestamp.ToUnixTimeMilliseconds());
    writer.WriteValue(value.Value);
    writer.WriteEndArray();
  }

  /// <inheritdoc />
  public override TimestampValueItem ReadJson
  (
    JsonReader reader, Type objectType,
    TimestampValueItem? existingValue, bool hasExistingValue,
    JsonSerializer serializer
  )
  {
    var arr = JArray.Load(reader);

    return new TimestampValueItem
    {
      Timestamp = arr[0].Value<long>().ToDateTimeFromUnixTimeMilliseconds(),
      Value = arr[1].Value<decimal>()
    };
  }
}
