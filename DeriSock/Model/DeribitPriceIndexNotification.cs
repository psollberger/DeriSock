namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class DeribitPriceIndexNotification
  {
    /// <summary>
    ///   Index identifier, matches (base) cryptocurrency with quote currency
    /// </summary>
    [JsonProperty("index_name")]
    public string IndexName { get; set; }

    /// <summary>
    ///   Current index price
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

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
