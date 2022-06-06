namespace DeriSock.Model;

using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
/// Public market data request for volatility index candles.
/// </summary>

[JsonConverter(typeof(VolatilityIndexCandleConverter))]
public class VolatilityIndexCandle
{
  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  public long Timestamp { get; set; }
  
  /// <summary>
  ///   Open
  /// </summary>
  public decimal Open { get; set; }

  /// <summary>
  ///   High
  /// </summary>
  public decimal High { get; set; }

  /// <summary>
  ///   Low
  /// </summary>
  public decimal Low { get; set; }

  /// <summary>
  ///   Close
  /// </summary>
  public decimal Close { get; set; }

  /// <inheritdoc cref="Timestamp" />
  [JsonIgnore]
  public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();
}

public class VolatilityIndexCandleConverter : JsonConverter<VolatilityIndexCandle>
{
  public override bool CanWrite => false;

  public override void WriteJson(JsonWriter writer, VolatilityIndexCandle value, JsonSerializer serializer)
  {
    throw new NotSupportedException();
  }

  public override VolatilityIndexCandle ReadJson(
    JsonReader reader, Type objectType,
    VolatilityIndexCandle existingValue, bool hasExistingValue,
    JsonSerializer serializer)
  {
    var arr = JArray.Load(reader);
    return new VolatilityIndexCandle
    {
      Timestamp = arr[0].Value<long>(), 
      Open = arr[1].Value<decimal>(),
      High = arr[2].Value<decimal>(),
      Low = arr[3].Value<decimal>(),
      Close = arr[4].Value<decimal>(),
    };
  }
}

