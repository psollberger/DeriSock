namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

/// <summary>
/// Public market data request for volatility index candles.
/// </summary>
public class VolatilityIndexData
{
  /// <summary>
  ///   Continuation - to be used as the end_timestamp parameter on the next request. NULL when no continuation.
  /// </summary>
  [JsonProperty("continuation")]
  public long? Continuation { get; set; }
  
  [JsonProperty("data")]
  public VolatilityIndexCandle[] Candles { get; set; }
}
