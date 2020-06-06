namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class PerpetualInterestRateNotification
  {
    /// <summary>
    ///   Current index price
    /// </summary>
    [JsonProperty("index_price")]
    public decimal IndexPrice { get; set; }

    /// <summary>
    ///   Current interest
    /// </summary>
    [JsonProperty("interest")]
    public decimal Interest { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <inheritdoc cref="Timestamp" />
    [JsonIgnore]
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();
  }
}
