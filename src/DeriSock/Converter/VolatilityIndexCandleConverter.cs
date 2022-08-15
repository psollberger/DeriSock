namespace DeriSock.Converter;

using System;
using System.Diagnostics;

using DeriSock.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Converts an item of the volatility index as array of [timestamp, value, value, value, value]
/// </summary>
public class VolatilityIndexCandleConverter : JsonConverter<VolatilityIndexCandle>
{
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, VolatilityIndexCandle? value, JsonSerializer serializer)
  {
    Debug.Assert(value != null, nameof(value) + " != null");
    writer.WriteStartArray();
    writer.WriteValue(value.Timestamp.ToUnixTimeMilliseconds());
    writer.WriteValue(value.Open);
    writer.WriteValue(value.High);
    writer.WriteValue(value.Low);
    writer.WriteValue(value.Close);
    writer.WriteEndArray();    
  }

  /// <inheritdoc />
  public override VolatilityIndexCandle? ReadJson(JsonReader reader, Type objectType, VolatilityIndexCandle? existingValue, bool hasExistingValue, JsonSerializer serializer)
  {
    var arr = JArray.Load(reader);

    return new VolatilityIndexCandle
    {
      Timestamp = arr[0].Value<long>().ToDateTimeFromUnixTimeMilliseconds(),
      Open = arr[1].Value<decimal>(),
      High = arr[2].Value<decimal>(),
      Low = arr[3].Value<decimal>(),
      Close = arr[4].Value<decimal>()
    };
  }
}
