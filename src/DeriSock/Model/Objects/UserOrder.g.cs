// --------------------------------------------------------------------------
// <auto-generated>
//      This code was generated by a tool.
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
// </auto-generated>
// --------------------------------------------------------------------------
#pragma warning disable CS1591
#nullable enable
namespace DeriSock.Model
{
  using System;
  using DeriSock.Converter;
  
  /// <summary>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class UserOrder
  {
    /// <summary>
    /// <para><c>true</c> if order was cancelled by mmp trigger (optional)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("mmp_cancelled")]
    public bool? MmpCancelled { get; set; }
    /// <summary>
    /// <para>Order state: <c>&quot;open&quot;</c>, <c>&quot;filled&quot;</c>, <c>&quot;rejected&quot;</c>, <c>&quot;cancelled&quot;</c>, <c>&quot;untriggered&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("order_state")]
    public string OrderState { get; set; } = string.Empty;
    /// <summary>
    /// <para>Maximum amount within an order to be shown to other traders, 0 for invisible order.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("max_show")]
    public decimal MaxShow { get; set; }
    /// <summary>
    /// <para><c>true</c> if order has <c>reject_post_only</c> flag (field is present only when <c>post_only</c> is <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("reject_post_only")]
    public bool RejectPostOnly { get; set; }
    /// <summary>
    /// <para><c>true</c> if created with API</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("api")]
    public bool Api { get; set; }
    /// <summary>
    /// <para>It represents the requested order size. For perpetual and futures the amount is in USD units, for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("amount")]
    public decimal Amount { get; set; }
    /// <summary>
    /// <para><c>true</c> if created via Deribit frontend (optional)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("web")]
    public bool? Web { get; set; }
    /// <summary>
    /// <para>Unique instrument identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_name")]
    public string InstrumentName { get; set; } = string.Empty;
    /// <summary>
    /// <para>advanced type: <c>&quot;usd&quot;</c> or <c>&quot;implv&quot;</c> (Only for options; field is omitted if not applicable).</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("advanced")]
    public string? Advanced { get; set; }
    /// <summary>
    /// <para>Whether the trigger order has been triggered</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("triggered")]
    public bool Triggered { get; set; }
    /// <summary>
    /// <para><c>true</c> if order made from block_trade trade, added only in that case.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("block_trade")]
    public bool BlockTrade { get; set; }
    /// <summary>
    /// <para>Original order type. Optional field</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("original_order_type")]
    public string? OriginalOrderType { get; set; }
    /// <summary>
    /// <para>The maximum deviation from the price peak beyond which the order will be triggered (Only for trailing trigger orders)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trigger_offset")]
    public decimal? TriggerOffset { get; set; }
    /// <summary>
    /// <para>Price in base currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("price")]
    public decimal Price { get; set; }
    /// <summary>
    /// <para><c>true</c> if the order is a MMP order, otherwise <c>false</c>.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("mmp")]
    public bool Mmp { get; set; }
    /// <summary>
    /// <para>Order time in force: <c>&quot;good_til_cancelled&quot;</c>, <c>&quot;good_til_day&quot;</c>, <c>&quot;fill_or_kill&quot;</c> or <c>&quot;immediate_or_cancel&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("time_in_force")]
    public string TimeInForce { get; set; } = string.Empty;
    /// <summary>
    /// <para>Options, advanced orders only - <c>true</c> if last modification of the order was performed by the pricing engine, otherwise <c>false</c>.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("auto_replaced")]
    public bool AutoReplaced { get; set; }
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("last_update_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime LastUpdateTimestamp { get; set; }
    /// <summary>
    /// <para><c>true</c> for post-only orders only</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("post_only")]
    public bool PostOnly { get; set; }
    /// <summary>
    /// <para><c>true</c> if the order was edited (by user or - in case of advanced options orders - by pricing engine), otherwise <c>false</c>.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("replaced")]
    public bool Replaced { get; set; }
    /// <summary>
    /// <para>Filled amount of the order. For perpetual and futures the filled_amount is in USD units, for options - in units or corresponding cryptocurrency contracts, e.g., BTC or ETH.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("filled_amount")]
    public decimal FilledAmount { get; set; }
    /// <summary>
    /// <para>Average fill price of the order</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("average_price")]
    public decimal AveragePrice { get; set; }
    /// <summary>
    /// <para>Unique order identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("order_id")]
    public string OrderId { get; set; } = string.Empty;
    /// <summary>
    /// <para><c>true</c> for reduce-only orders only</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("reduce_only")]
    public bool ReduceOnly { get; set; }
    /// <summary>
    /// <para>Commission paid so far (in base currency)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("commission")]
    public decimal Commission { get; set; }
    /// <summary>
    /// <para>The name of the application that placed the order on behalf of the user (optional).</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("app_name")]
    public string? AppName { get; set; }
    /// <summary>
    /// <para>User defined label (up to 64 characters)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("label")]
    public string Label { get; set; } = string.Empty;
    /// <summary>
    /// <para>Id of the trigger order that created the order (Only for orders that were created by triggered orders).</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trigger_order_id")]
    public string? TriggerOrderId { get; set; }
    /// <summary>
    /// <para>Trigger price (Only for future trigger orders)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trigger_price")]
    public decimal? TriggerPrice { get; set; }
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("creation_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime CreationTimestamp { get; set; }
    /// <summary>
    /// <para>Direction: <c>buy</c>, or <c>sell</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("direction")]
    public string Direction { get; set; } = string.Empty;
    /// <summary>
    /// <para><c>true</c> if order was automatically created during liquidation</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("is_liquidation")]
    public bool IsLiquidation { get; set; }
    /// <summary>
    /// <para>Order type: <c>&quot;limit&quot;</c>, <c>&quot;market&quot;</c>, <c>&quot;stop_limit&quot;</c>, <c>&quot;stop_market&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("order_type")]
    public string OrderType { get; set; } = string.Empty;
    /// <summary>
    /// <para>Option price in USD (Only if <c>advanced=&quot;usd&quot;</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("usd")]
    public decimal Usd { get; set; }
    /// <summary>
    /// <para>Profit and loss in base currency.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("profit_loss")]
    public decimal ProfitLoss { get; set; }
    /// <summary>
    /// <para>The price of the given trigger at the time when the order was placed (Only for trailing trigger orders)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trigger_reference_price")]
    public decimal? TriggerReferencePrice { get; set; }
    /// <summary>
    /// <para><c>true</c> if the order is marked by the platform as a risk reducing order (can apply only to orders placed by PM users), otherwise <c>false</c>.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("risk_reducing")]
    public bool RiskReducing { get; set; }
    /// <summary>
    /// <para>Implied volatility in percent. (Only if <c>advanced=&quot;implv&quot;</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("implv")]
    public decimal Implv { get; set; }
    /// <summary>
    /// <para>Trigger type (only for trigger orders). Allowed values: <c>&quot;index_price&quot;</c>, <c>&quot;mark_price&quot;</c>, <c>&quot;last_price&quot;</c>.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trigger")]
    public string? Trigger { get; set; }
  }
}
