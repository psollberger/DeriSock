namespace DeriSock.Converter;

using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <inheritdoc />
public class MicrosecondsTimestampConverter : JsonConverter<DateTime>
{
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, DateTime value, JsonSerializer serializer)
  {
    writer.WriteValue(value.ToUnixTimeMicroseconds());
  }

  /// <inheritdoc />
  public override DateTime ReadJson(JsonReader reader, Type objectType, DateTime existingValue, bool hasExistingValue, JsonSerializer serializer)
  {
    var jsonValue = JToken.Load(reader);
    return jsonValue.Value<long>().ToDateTimeFromUnixTimeMicroseconds();
  }
}
