namespace DeriSock.Converter;

using System;
using System.Diagnostics;

using DeriSock.Model;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Converts an array of [action, price, amount]
/// </summary>
public class ActionPriceAmountArrayConverter : JsonConverter<ActionPriceAmountItem>
{
  /// <inheritdoc />
  public override void WriteJson(JsonWriter writer, ActionPriceAmountItem? value, JsonSerializer serializer)
  {
    Debug.Assert(value != null, nameof(value) + " != null");

    writer.WriteStartArray();
    writer.WriteValue(value!.Action);
    writer.WriteValue(value.Price);
    writer.WriteValue(value.Amount);
    writer.WriteEndArray();
  }

  /// <inheritdoc />
  public override ActionPriceAmountItem ReadJson
  (
    JsonReader reader, Type objectType,
    ActionPriceAmountItem? existingValue, bool hasExistingValue,
    JsonSerializer serializer
  )
  {
    var arr = JArray.Load(reader);

    return new ActionPriceAmountItem
    {
      Action = arr[0].Value<string>()!,
      Price = arr[1].Value<decimal>(),
      Amount = arr[2].Value<decimal>()
    };
  }
}
