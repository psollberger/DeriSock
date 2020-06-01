namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class UserStopOrder
  {
    /// <summary>
    ///   It represents the requested order size. For perpetual and futures the amount is in USD units, for options it is
    ///   amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    ///   Direction: buy, or sell
    /// </summary>
    [JsonProperty("direction")]
    public string Direction { get; set; }

    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("last_update_timestamp")]
    public long LastUpdateTimestamp { get; set; }

    /// <inheritdoc cref="LastUpdateTimestamp" />
    [JsonIgnore]
    public DateTime LastUpdateDateTime => LastUpdateTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Unique order identifier
    /// </summary>
    [JsonProperty("order_id")]
    public string OrderId { get; set; }

    /// <summary>
    ///   order state, "open", "filled", "rejected", "cancelled", "untriggered"
    /// </summary>
    [JsonProperty("order_state")]
    public string OrderState { get; set; }

    /// <summary>
    ///   Price in base currency
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

    /// <summary>
    ///   user or system request type for the stop order. "add" - to add order, "cancel" - to cancel, "edit" - to change order,
    ///   "trigger:stop" - to trigger stop order, "trigger:order" - to spawn limit or market order as the result of the
    ///   triggering "trigger:stop"
    /// </summary>
    [JsonProperty("request")]
    public string Request { get; set; }

    /// <summary>
    ///   Id of the user stop-order used for the stop-order reference before triggering
    /// </summary>
    [JsonProperty("stop_id")]
    public string StopId { get; set; }

    /// <summary>
    ///   stop price (Only for future stop orders)
    /// </summary>
    [JsonProperty("stop_price")]
    public decimal StopPrice { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <inheritdoc cref="Timestamp" />
    [JsonIgnore]
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Trigger type (Only for stop orders). Allowed values: "index_price", "mark_price", "last_price".
    /// </summary>
    [JsonProperty("trigger")]
    public string Trigger { get; set; }
  }
}
