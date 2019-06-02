namespace DeriSock.Model
{
  using System.Runtime.Serialization;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;

  [JsonConverter(typeof(StringEnumConverter))]
  public enum OrderType
  {
    [EnumMember(Value = "buy")]
    Buy,
    [EnumMember(Value = "sell")]
    Sell
  }
}
