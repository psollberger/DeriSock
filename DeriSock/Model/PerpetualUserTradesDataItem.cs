namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class PerpetualUserTradesDataItem
{
  /// <summary>
  ///   Current index price
  /// </summary>
  [JsonProperty("index_price")]
  public decimal IndexPrice { get; set; }

  /// <summary>
  ///   Historical interest 8h value
  /// </summary>
  [JsonProperty("interest_8h")]
  public decimal Interest8H { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("timestamp")]
  public long Timestamp { get; set; }

  /// <inheritdoc cref="Timestamp" />
  [JsonIgnore]
  public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();
}
