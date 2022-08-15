namespace DeriSock.Model;

using System;

using DeriSock.Converter;

using Newtonsoft.Json;

[JsonConverter(typeof(TimestampValueConverter))]
public class TimestampValueItem
{
  public DateTime Timestamp { get; set; }
  public decimal Value { get; set; }
}
