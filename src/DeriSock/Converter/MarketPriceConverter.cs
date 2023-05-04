using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DeriSock.Converter;

public class MarketPriceConverter : JsonConverter
{
  public override bool CanConvert(Type objectType)
  {
    return objectType == typeof(decimal);
  }

  public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
  {
    if (reader.TokenType == JsonToken.String)
    {
      string value = reader.Value.ToString();
      if (value == "market_price")
      {
        // Set the value of "mark_price" to 0
        return 0.0m;
      }
    }
    else if (reader.TokenType == JsonToken.Null)
    {
      return 0.0m;
    }

    // If "mark_price" is not "market_price", return the value as a decimal
    return serializer.Deserialize<decimal>(reader);
  }

  public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
  {
    throw new NotImplementedException();
  }
}
