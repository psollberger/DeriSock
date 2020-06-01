namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class UserOrder
  {
    /// <summary>
    ///   order state, "open", "filled", "rejected", "cancelled", "untriggered"
    /// </summary>
    [JsonProperty("order_state")]
    public string OrderState { get; set; }

    /// <summary>
    ///   Maximum amount within an order to be shown to other traders, 0 for invisible order.
    /// </summary>
    [JsonProperty("max_show")]
    public decimal MaxShow { get; set; }

    /// <summary>
    ///   true if created with API
    /// </summary>
    [JsonProperty("api")]
    public bool Api { get; set; }

    /// <summary>
    ///   It represents the requested order size. For perpetual and futures the amount is in USD units, for options it is
    ///   amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    ///   true if created via Deribit frontend (optional)
    /// </summary>
    [JsonProperty("web")]
    public bool Web { get; set; }

    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   advanced type: "usd" or "implv" (Only for options; field is omitted if not applicable).
    /// </summary>
    [JsonProperty("advanced")]
    public string Advanced { get; set; }

    /// <summary>
    ///   Whether the stop order has been triggered (Only for stop orders)
    /// </summary>
    [JsonProperty("triggered")]
    public bool Triggered { get; set; }

    /// <summary>
    ///   true if order made from block_trade trade, added only in that case.
    /// </summary>
    [JsonProperty("block_trade")]
    public bool BlockTrade { get; set; }

    /// <summary>
    ///   Original order type. Optional field
    /// </summary>
    [JsonProperty("original_order_type")]
    public string OriginalOrderType { get; set; }

    /// <summary>
    ///   Price in base currency
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

    /// <summary>
    ///   Order time in force: "good_til_cancelled", "fill_or_kill", "immediate_or_cancel"
    /// </summary>
    [JsonProperty("time_in_force")]
    public string TimeInForce { get; set; }

    /// <summary>
    ///   Options, advanced orders only - true if last modification of the order was performed by the pricing engine, otherwise
    ///   false.
    /// </summary>
    [JsonProperty("auto_replaced")]
    public bool AutoReplaced { get; set; }

    /// <summary>
    ///   Id of the stop order that was triggered to create the order (Only for orders that were created by triggered stop
    ///   orders).
    /// </summary>
    [JsonProperty("stop_order_id")]
    public string StopOrderId { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("last_update_timestamp")]
    public long LastUpdateTimestamp { get; set; }

    /// <inheritdoc cref="LastUpdateTimestamp" />
    [JsonIgnore]
    public DateTime LastUpdateDateTime => LastUpdateTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   true for post-only orders only
    /// </summary>
    [JsonProperty("post_only")]
    public bool PostOnly { get; set; }

    /// <summary>
    ///   true if the order was edited (by user or - in case of advanced options orders - by pricing engine), otherwise false.
    /// </summary>
    [JsonProperty("replaced")]
    public bool Replaced { get; set; }

    /// <summary>
    ///   Filled amount of the order. For perpetual and futures the filled_amount is in USD units, for options - in units or
    ///   corresponding cryptocurrency contracts, e.g., BTC or ETH.
    /// </summary>
    [JsonProperty("filled_amount")]
    public decimal FilledAmount { get; set; }

    /// <summary>
    ///   Average fill price of the order
    /// </summary>
    [JsonProperty("average_price")]
    public decimal AveragePrice { get; set; }

    /// <summary>
    ///   Unique order identifier
    /// </summary>
    [JsonProperty("order_id")]
    public string OrderId { get; set; }

    /// <summary>
    ///   true for reduce-only orders only
    /// </summary>
    [JsonProperty("reduce_only")]
    public bool ReduceOnly { get; set; }

    /// <summary>
    ///   Commission paid so far (in base currency)
    /// </summary>
    [JsonProperty("commission")]
    public decimal Commission { get; set; }

    /// <summary>
    ///   Name of the application that placed the order on behalf of the user (optional).
    /// </summary>
    [JsonProperty("app_name")]
    public string AppName { get; set; }

    /// <summary>
    ///   stop price (Only for future stop orders)
    /// </summary>
    [JsonProperty("stop_price")]
    public decimal StopPrice { get; set; }

    /// <summary>
    ///   user defined label (up to 64 characters)
    /// </summary>
    [JsonProperty("label")]
    public string Label { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("creation_timestamp")]
    public long CreationTimestamp { get; set; }

    /// <inheritdoc cref="CreationTimestamp" />
    [JsonIgnore]
    public DateTime CreationDateTime => CreationTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Direction: buy, or sell
    /// </summary>
    [JsonProperty("direction")]
    public string Direction { get; set; }

    /// <summary>
    ///   true if order was automatically created during liquidation
    /// </summary>
    [JsonProperty("is_liquidation")]
    public bool IsLiquidation { get; set; }

    /// <summary>
    ///   order type, "limit", "market", "stop_limit", "stop_market"
    /// </summary>
    [JsonProperty("order_type")]
    public string OrderType { get; set; }

    /// <summary>
    ///   Option price in USD (Only if advanced="usd")
    /// </summary>
    [JsonProperty("usd")]
    public decimal Usd { get; set; }

    /// <summary>
    ///   Profit and loss in base currency.
    /// </summary>
    [JsonProperty("profit_loss")]
    public decimal ProfitLoss { get; set; }

    /// <summary>
    ///   Implied volatility in percent. (Only if advanced="implv")
    /// </summary>
    [JsonProperty("implv")]
    public decimal ImpliedVolatility { get; set; }

    /// <summary>
    ///   Trigger type (Only for stop orders). Allowed values: "index_price", "mark_price", "last_price".
    /// </summary>
    [JsonProperty("trigger")]
    public string Trigger { get; set; }
  }
}
