namespace DeriSock.Converter;

using System;
using System.Diagnostics;

using DeriSock.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Converts an array of [price, amount]
/// </summary>
public class PriceAmountArrayConverter : JsonConverter<PriceAmountItem>
{
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, PriceAmountItem? value, JsonSerializer serializer)
  {
    Debug.Assert(value != null, nameof(value) + " != null");
    writer.WriteStartArray();
    writer.WriteValue(value.Price);
    writer.WriteValue(value.Amount);
    writer.WriteEndArray();
  }

  /// <inheritdoc />
  public override PriceAmountItem ReadJson
  (
    JsonReader reader, Type objectType,
    PriceAmountItem? existingValue, bool hasExistingValue,
    JsonSerializer serializer
  )
  {
    var arr = JArray.Load(reader);

    return new PriceAmountItem
    {
      Price = arr[0].Value<decimal>(),
      Amount = arr[1].Value<decimal>()
    };
  }
}
