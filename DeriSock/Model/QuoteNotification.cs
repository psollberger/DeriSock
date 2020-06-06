namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class QuoteNotification
  {
    /// <summary>
    ///   It represents the requested order size of all best asks
    /// </summary>
    [JsonProperty("best_ask_amount")]
    public decimal BestAskAmount { get; set; }

    /// <summary>
    ///   The current best ask price, null if there aren't any asks
    /// </summary>
    [JsonProperty("best_ask_price")]
    public decimal BestAskPrice { get; set; }

    /// <summary>
    ///   It represents the requested order size of all best bids
    /// </summary>
    [JsonProperty("best_bid_amount")]
    public decimal BestBidAmount { get; set; }

    /// <summary>
    ///   The current best bid price, null if there aren't any bids
    /// </summary>
    [JsonProperty("best_bid_price")]
    public decimal BestBidPrice { get; set; }

    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

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
