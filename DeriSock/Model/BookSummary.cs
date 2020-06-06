namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class BookSummary
  {
    /// <summary>
    ///   The current best ask price, null if there aren't any asks
    /// </summary>
    [JsonProperty("ask_price")]
    public decimal? AskPrice { get; set; }

    /// <summary>
    ///   Base currency
    /// </summary>
    [JsonProperty("base_currency")]
    public string BaseCurrency { get; set; }

    /// <summary>
    ///   The current best bid price, null if there aren't any bids
    /// </summary>
    [JsonProperty("bid_price")]
    public decimal? BidPrice { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("creation_timestamp")]
    public long CreationTimestamp { get; set; }

    /// <inheritdoc cref="CreationTimestamp" />
    [JsonIgnore]
    public DateTime CreationDateTime => CreationTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Current funding (perpetual only)
    /// </summary>
    [JsonProperty("current_funding")]
    public decimal CurrentFunding { get; set; }

    /// <summary>
    ///   Estimated delivery price, in USD. For more details, see Documentation > General > Expiration Price
    /// </summary>
    [JsonProperty("estimated_delivery_price")]
    public decimal? EstimatedDeliveryPrice { get; set; }

    /// <summary>
    ///   Funding 8h (perpetual only)
    /// </summary>
    [JsonProperty("funding_8h")]
    public decimal Funding8H { get; set; }

    /// <summary>
    ///   Price of the 24h highest trade, null if there weren't any trades
    /// </summary>
    [JsonProperty("high")]
    public decimal? High { get; set; }

    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   Interest rate used in implied volatility calculations (options only)
    /// </summary>
    [JsonProperty("interest_rate")]
    public decimal InterestRate { get; set; }

    /// <summary>
    ///   The price of the latest trade, null if there weren't any trades
    /// </summary>
    [JsonProperty("last")]
    public decimal? Last { get; set; }

    /// <summary>
    ///   Price of the 24h lowest trade, null if there weren't any trades
    /// </summary>
    [JsonProperty("low")]
    public decimal? Low { get; set; }

    /// <summary>
    ///   The current instrument market price
    /// </summary>
    [JsonProperty("mark_price")]
    public decimal MarkPrice { get; set; }

    /// <summary>
    ///   The average of the best bid and ask, null if there aren't any asks or bids
    /// </summary>
    [JsonProperty("mid_price")]
    public decimal? MidPrice { get; set; }

    /// <summary>
    ///   The total amount of outstanding contracts in the corresponding amount units. For perpetual and futures the amount is
    ///   in USD units, for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.
    /// </summary>
    [JsonProperty("open_interest")]
    public decimal OpenInterest { get; set; }

    /// <summary>
    ///   24-hour price change expressed as a percentage, null if there weren't any trades
    /// </summary>
    [JsonProperty("price_change")]
    public decimal? PriceChange { get; set; }

    /// <summary>
    ///   Quote currency
    /// </summary>
    [JsonProperty("quote_currency")]
    public string QuoteCurrency { get; set; }

    /// <summary>
    ///   Name of the underlying future, or 'index_price' (options only)
    /// </summary>
    [JsonProperty("underlying_index")]
    public string UnderlyingIndex { get; set; }

    /// <summary>
    ///   underlying price for implied volatility calculations (options only)
    /// </summary>
    [JsonProperty("underlying_price")]
    public decimal UnderlyingPrice { get; set; }

    /// <summary>
    ///   The total 24h traded volume (in base currency)
    /// </summary>
    [JsonProperty("volume")]
    public decimal Volume { get; set; }

    /// <summary>
    ///   Volume in usd (futures only)
    /// </summary>
    [JsonProperty("volume_usd")]
    public decimal VolumeUsd { get; set; }
  }
}
