namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class DeribitPriceRankingNotification
  {
    /// <summary>
    ///   Stock exchange status
    /// </summary>
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    /// <summary>
    ///   Stock exchange identifier
    /// </summary>
    [JsonProperty("identifier")]
    public string Identifier { get; set; }

    /// <summary>
    ///   Index price retrieved from stock's data
    /// </summary>
    [JsonProperty("original_price")]
    public decimal OriginalPrice { get; set; }

    /// <summary>
    ///   Adjusted stock exchange index price, used for Deribit price index calculations
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

    /// <summary>
    ///   The timestamp of the last update from stock exchange
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <inheritdoc cref="Timestamp" />
    [JsonIgnore]
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   The weight of the ranking given in percent
    /// </summary>
    [JsonProperty("weight")]
    public decimal Weight { get; set; }
  }
}
