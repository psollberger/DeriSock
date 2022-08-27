namespace DeriSock.Model;

using System;

using DeriSock.Converter;

using Newtonsoft.Json;

/// <summary>
///   Represents data that consinsts of a timestamp and a floating point numeric value.
/// </summary>
[JsonConverter(typeof(TimestampValueConverter))]
public class TimestampValueItem
{
  /// <summary>
  ///   The timestamp.
  /// </summary>
  public DateTime Timestamp { get; set; }

  /// <summary>
  ///   The floating point numeric value.
  /// </summary>
  public decimal Value { get; set; }
}
