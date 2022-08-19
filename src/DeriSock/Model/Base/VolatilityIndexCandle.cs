namespace DeriSock.Model;

using System;

using DeriSock.Converter;

using Newtonsoft.Json;

/// <summary>
///   Represents a volatility index candle.
/// </summary>
[JsonConverter(typeof(VolatilityIndexCandleConverter))]
public class VolatilityIndexCandle
{
  /// <summary>
  ///   The Timestamp of the candle.
  /// </summary>
  public DateTime Timestamp { get; set; }

  /// <summary>
  ///   The open value of the candle.
  /// </summary>
  public decimal Open { get; set; }

  /// <summary>
  ///   The high value of the candle.
  /// </summary>
  public decimal High { get; set; }

  /// <summary>
  ///   The low value of the candle.
  /// </summary>
  public decimal Low { get; set; }

  /// <summary>
  ///   The close value of the candle.
  /// </summary>
  public decimal Close { get; set; }
}
