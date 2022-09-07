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
  
  /// <summary>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class PrivateEditRequest
  {
    /// <summary>
    /// <para>The order id</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("order_id")]
    public string OrderId { get; set; } = string.Empty;
    /// <summary>
    /// <para>It represents the requested order size. For perpetual and futures the amount is in USD units, for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("amount")]
    public decimal Amount { get; set; }
    /// <summary>
    /// <para>The order price in base currency.<br/>
    ///When editing an option order with advanced=usd, the field price should be the option price value in USD.<br/>
    ///When editing an option order with advanced=implv, the field price should be a value of implied volatility in percentages. For example,  price=100, means implied volatility of 100%</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("price")]
    public decimal? Price { get; set; }
    /// <summary>
    /// <para>If true, the order is considered post-only. If the new price would cause the order to be filled immediately (as taker), the price will be changed to be just below or above the spread (accordingly to the original order type).<br/>
    ///Only valid in combination with time_in_force=<c>&quot;good_til_cancelled&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("post_only")]
    public bool? PostOnly { get; set; }
    /// <summary>
    /// <para>If <c>true</c>, the order is considered reduce-only which is intended to only reduce a current position</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("reduce_only")]
    public bool? ReduceOnly { get; set; }
    /// <summary>
    /// <para>If an order is considered post-only and this field is set to true then the order is put to order book unmodified or request is rejected.<br/>
    ///Only valid in combination with <c>&quot;post_only&quot;</c> set to true</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("reject_post_only")]
    public bool? RejectPostOnly { get; set; }
    /// <summary>
    /// <para>Advanced option order type. If you have posted an advanced option order, it is necessary to re-supply this parameter when editing it (Only for options)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("advanced")]
    public AdvancedOptionOrderType? Advanced { get; set; }
    /// <summary>
    /// <para>Trigger price, required for trigger orders only (Stop-loss or Take-profit orders)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trigger_price")]
    public decimal? TriggerPrice { get; set; }
    /// <summary>
    /// <para>The maximum deviation from the price peak beyond which the order will be triggered</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trigger_offset")]
    public decimal? TriggerOffset { get; set; }
    /// <summary>
    /// <para>Order MMP flag, only for order_type &apos;limit&apos;</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("mmp")]
    public bool? Mmp { get; set; }
    /// <summary>
    /// <para>Timestamp, when provided server will start processing request in Matching Engine only before given timestamp, in other cases <c>timed_out</c> error will be responded. Remember that the given timestamp should be consistent with the server&apos;s time, use <a href="https://docs.deribit.com/#public-get_time">/public/time</a> method to obtain current server time.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("valid_until")]
    public long? ValidUntil { get; set; }
  }
}
