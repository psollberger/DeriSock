namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class DateTimeValuePair
  {
    [JsonProperty(Order = 1)]
    public long Timestamp { get; set; }

    [JsonIgnore]
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();

    [JsonProperty(Order = 2)]
    public decimal Value { get; set; }
  }
}
