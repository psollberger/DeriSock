namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class MarketTrade
  {
    /// <summary>
    ///   Trade amount. For perpetual and futures - in USD units, for options it is amount of corresponding cryptocurrency
    ///   contracts, e.g., BTC or ETH.
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    ///   Block trade id - when trade was part of block trade
    /// </summary>
    [JsonProperty("block_trade_id")]
    public string BlockTradeId { get; set; }

    /// <summary>
    ///   Direction: <c>buy</c>, or <c>sell</c>
    /// </summary>
    [JsonProperty("direction")]
    public string Direction { get; set; }

    /// <summary>
    ///   Index Price at the moment of trade
    /// </summary>
    [JsonProperty("index_price")]
    public decimal IndexPrice { get; set; }

    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   Option implied volatility for the price (Option only)
    /// </summary>
    [JsonProperty("iv")]
    public decimal Iv { get; set; }

    /// <summary>
    ///   Optional field (only for trades caused by liquidation): "M" when maker side of trade was under liquidation, "T" when
    ///   taker side was under liquidation, "MT" when both sides of trade were under liquidation
    /// </summary>
    [JsonProperty("liquidation")]
    public string Liquidation { get; set; }

    /// <summary>
    ///   Price in base currency
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

    /// <summary>
    ///   Direction of the "tick" (0 = Plus Tick, 1 = Zero-Plus Tick, 2 = Minus Tick, 3 = Zero-Minus Tick).
    /// </summary>
    [JsonProperty("tick_direction")]
    public int TickDirection { get; set; }

    /// <summary>
    ///   The timestamp of the trade
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <inheritdoc cref="Timestamp" />
    [JsonIgnore]
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Unique (per currency) trade identifier
    /// </summary>
    [JsonProperty("trade_id")]
    public string TradeId { get; set; }

    /// <summary>
    ///   The sequence number of the trade within instrument
    /// </summary>
    [JsonProperty("trade_seq")]
    public long TradeSeq { get; set; }
  }
}
