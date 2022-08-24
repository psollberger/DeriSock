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
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial class Instrument
  {
    /// <summary>
    /// <para>The underlying currency being traded.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("base_currency")]
    public string BaseCurrency { get; set; } = string.Empty;
    /// <summary>
    /// <para>Block Trade commission for instrument</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("block_trade_commission")]
    public decimal? BlockTradeCommission { get; set; }
    /// <summary>
    /// <para>Contract size for instrument</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("contract_size")]
    public long? ContractSize { get; set; }
    /// <summary>
    /// <para>Counter currency for the instrument.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("counter_currency")]
    public string CounterCurrency { get; set; } = string.Empty;
    /// <summary>
    /// <para>The time when the instrument was first created (milliseconds since the UNIX epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("creation_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime CreationTimestamp { get; set; }
    /// <summary>
    /// <para>The time when the instrument will expire (milliseconds since the UNIX epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("expiration_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime ExpirationTimestamp { get; set; }
    /// <summary>
    /// <para>Future type (only for futures)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("future_type")]
    public string? FutureType { get; set; }
    /// <summary>
    /// <para>Instrument ID</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_id")]
    public long InstrumentId { get; set; }
    /// <summary>
    /// <para>Unique instrument identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_name")]
    public string InstrumentName { get; set; } = string.Empty;
    /// <summary>
    /// <para>Indicates if the instrument can currently be traded.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("is_active")]
    public bool IsActive { get; set; }
    /// <summary>
    /// <para>Instrument kind: <c>&quot;future&quot;</c>, <c>&quot;option&quot;</c>, <c>&quot;future_combo&quot;</c>, <c>&quot;option_combo&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("kind")]
    public string Kind { get; set; } = string.Empty;
    /// <summary>
    /// <para>Maximal leverage for instrument, for futures only</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("max_leverage")]
    public long? MaxLeverage { get; set; }
    /// <summary>
    /// <para>Maker commission for instrument</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("maker_commission")]
    public decimal? MakerCommission { get; set; }
    /// <summary>
    /// <para>Minimum amount for trading. For perpetual and futures - in USD units, for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("min_trade_amount")]
    public decimal? MinTradeAmount { get; set; }
    /// <summary>
    /// <para>The option type (only for options)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("option_type")]
    public string? OptionType { get; set; }
    /// <summary>
    /// <para>Name of price index that is used for this instrument</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("price_index")]
    public string PriceIndex { get; set; } = string.Empty;
    /// <summary>
    /// <para>The currency in which the instrument prices are quoted.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("quote_currency")]
    public string QuoteCurrency { get; set; } = string.Empty;
    /// <summary>
    /// <para>Settlement currency for the instrument.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("settlement_currency")]
    public string SettlementCurrency { get; set; } = string.Empty;
    /// <summary>
    /// <para>The settlement period.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("settlement_period")]
    public string SettlementPeriod { get; set; } = string.Empty;
    /// <summary>
    /// <para>The strike value. (only for options)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("strike")]
    public decimal? Strike { get; set; }
    /// <summary>
    /// <para>Taker commission for instrument</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("taker_commission")]
    public decimal? TakerCommission { get; set; }
    /// <summary>
    /// <para>specifies minimal price change and, as follows, the number of decimal places for instrument prices</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("tick_size")]
    public decimal? TickSize { get; set; }
  }
}
