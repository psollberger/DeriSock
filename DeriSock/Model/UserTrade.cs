namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class UserTrade
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
  ///   Direction: buy, or sell
  /// </summary>
  [JsonProperty("direction")]
  public string Direction { get; set; }

  /// <summary>
  ///   User's fee in units of the specified fee_currency
  /// </summary>
  [JsonProperty("fee")]
  public decimal Fee { get; set; }

  /// <summary>
  ///   Currency, i.e "BTC", "ETH"
  /// </summary>
  [JsonProperty("fee_currency")]
  public string FeeCurrency { get; set; }

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
  ///   User defined label (presented only when previously set for order by user)
  /// </summary>
  [JsonProperty("label")]
  public string Label { get; set; }

  /// <summary>
  ///   Optional field (only for trades caused by liquidation): "M" when maker side of trade was under liquidation, "T" when
  ///   taker side was under liquidation, "MT" when both sides of trade were under liquidation
  /// </summary>
  [JsonProperty("liquidation")]
  public string Liquidation { get; set; }

  /// <summary>
  ///   Describes what was role of users order: "M" when it was maker order, "T" when it was taker order
  /// </summary>
  [JsonProperty("liquidity")]
  public string Liquidity { get; set; }

  /// <summary>
  ///   Always null, except for a self-trade which is possible only if self-trading is switched on for the account (in that
  ///   case this will be id of the maker order of the subscriber)
  /// </summary>
  [JsonProperty("matching_id")]
  public string MatchingId { get; set; }

  /// <summary>
  ///   Id of the user order (maker or taker), i.e. subscriber's order id that took part in the trade
  /// </summary>
  [JsonProperty("order_id")]
  public string OrderId { get; set; }

  /// <summary>
  ///   Order type: "limit, "market", or "liquidation"
  /// </summary>
  [JsonProperty("order_type")]
  public string OrderType { get; set; }

  /// <summary>
  ///   true if user order is post-only
  /// </summary>
  [JsonProperty("post_only")]
  public string PostOnly { get; set; }

  /// <summary>
  ///   Price in base currency
  /// </summary>
  [JsonProperty("price")]
  public decimal Price { get; set; }

  /// <summary>
  ///   true if user order is reduce-only
  /// </summary>
  [JsonProperty("reduce_only")]
  public string ReduceOnly { get; set; }

  /// <summary>
  ///   true if the trade is against own order. This can only happen when your account has self-trading enabled. Contact an
  ///   administrator if you think you need that
  /// </summary>
  [JsonProperty("self_trade")]
  public bool SelfTrade { get; set; }

  /// <summary>
  ///   order state, "open", "filled", "rejected", "cancelled", "untriggered" or "archive" (if order was archived)
  /// </summary>
  [JsonProperty("state")]
  public string State { get; set; }

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

  /// <summary>
  ///   Underlying price for implied volatility calculations (Options only)
  /// </summary>
  [JsonProperty("underlying_price")]
  public decimal UnderlyingPrice { get; set; }
}
