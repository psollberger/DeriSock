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
  public partial class BookSummaryEntry
  {
    /// <summary>
    /// <para>The current best ask price, <c>null</c> if there aren&apos;t any asks</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("ask_price")]
    public decimal? AskPrice { get; set; }
    /// <summary>
    /// <para>Base currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("base_currency")]
    public string BaseCurrency { get; set; } = string.Empty;
    /// <summary>
    /// <para>The current best bid price, <c>null</c> if there aren&apos;t any bids</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("bid_price")]
    public decimal? BidPrice { get; set; }
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("creation_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime CreationTimestamp { get; set; }
    /// <summary>
    /// <para>Current funding (perpetual only)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("current_funding")]
    public decimal? CurrentFunding { get; set; }
    /// <summary>
    /// <para>Estimated delivery price for the market. For more details, see Contract Specification &gt; General Documentation &gt; Expiration Price</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("estimated_delivery_price")]
    public decimal EstimatedDeliveryPrice { get; set; }
    /// <summary>
    /// <para>Funding 8h (perpetual only)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("funding_8h")]
    public decimal? Funding8H { get; set; }
    /// <summary>
    /// <para>Price of the 24h highest trade</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("high")]
    public decimal High { get; set; }
    /// <summary>
    /// <para>Unique instrument identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_name")]
    public string InstrumentName { get; set; } = string.Empty;
    /// <summary>
    /// <para>Interest rate used in implied volatility calculations (options only)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("interest_rate")]
    public decimal? InterestRate { get; set; }
    /// <summary>
    /// <para>The price of the latest trade, <c>null</c> if there weren&apos;t any trades</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("last")]
    public decimal? Last { get; set; }
    /// <summary>
    /// <para>Price of the 24h lowest trade, <c>null</c> if there weren&apos;t any trades</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("low")]
    public decimal? Low { get; set; }
    /// <summary>
    /// <para>The current instrument market price</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("mark_price")]
    public decimal MarkPrice { get; set; }
    /// <summary>
    /// <para>The average of the best bid and ask, <c>null</c> if there aren&apos;t any asks or bids</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("mid_price")]
    public decimal? MidPrice { get; set; }
    /// <summary>
    /// <para>Volume in quote currency (futures only)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("notional_volume")]
    public decimal? NotionalVolume { get; set; }
    /// <summary>
    /// <para>The total amount of outstanding contracts in the corresponding amount units. For perpetual and futures the amount is in USD units, for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("open_interest")]
    public decimal OpenInterest { get; set; }
    /// <summary>
    /// <para>24-hour price change expressed as a percentage, <c>null</c> if there weren&apos;t any trades</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("price_change")]
    public decimal? PriceChange { get; set; }
    /// <summary>
    /// <para>Quote currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("quote_currency")]
    public string QuoteCurrency { get; set; } = string.Empty;
    /// <summary>
    /// <para>Name of the underlying future, or <c>&apos;index_price&apos;</c> (options only)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("underlying_index")]
    public string? UnderlyingIndex { get; set; }
    /// <summary>
    /// <para>underlying price for implied volatility calculations (options only)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("underlying_price")]
    public decimal? UnderlyingPrice { get; set; }
    /// <summary>
    /// <para>The total 24h traded volume (in base currency)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("volume")]
    public decimal Volume { get; set; }
    /// <summary>
    /// <para>[DEPRECATED] Volume in usd, use &apos;notional_volume&apos; instead, available only for instruments with USD as quote currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [System.ObsoleteAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("volume_usd")]
    public decimal? VolumeUsd { get; set; }
  }
}
