namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  [JsonConverter(typeof(VolatilityItemConverter))]
  public class VolatilityItem
  {
    public long Timestamp { get; set; }
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();
    public decimal Value { get; set; }
  }

  public class VolatilityItemConverter : JsonConverter<VolatilityItem>
  {
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, VolatilityItem value, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }

    public override VolatilityItem ReadJson(
      JsonReader reader, Type objectType,
      VolatilityItem existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var arr = JArray.Load(reader);
      return new VolatilityItem {Timestamp = arr[0].Value<long>(), Value = arr[1].Value<decimal>()};
    }
  }
}
