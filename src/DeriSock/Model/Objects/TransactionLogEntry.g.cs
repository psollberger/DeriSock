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
  using Newtonsoft.Json.Linq;
  
  /// <summary>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial class TransactionLogEntry
  {
    /// <summary>
    /// <para>The amount of traded contracts</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("amount")]
    public decimal Amount { get; set; }
    /// <summary>
    /// <para>Cash balance after the transaction</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("balance")]
    public decimal Balance { get; set; }
    /// <summary>
    /// <para>For futures and perpetual contracts: Realized session PNL (since last settlement). For options: the amount paid or received for the options traded.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("cashflow")]
    public decimal Cashflow { get; set; }
    /// <summary>
    /// <para>Change in cash balance. For trades: fees and options premium paid/received. For settlement: Futures session PNL and perpetual session funding.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("change")]
    public decimal Change { get; set; }
    /// <summary>
    /// <para>Commission paid so far (in base currency)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("commission")]
    public decimal Commission { get; set; }
    /// <summary>
    /// <para>Currency, i.e <c>&quot;BTC&quot;</c>, <c>&quot;ETH&quot;</c>, <c>&quot;USDC&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("currency")]
    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// <para>Updated equity value after the transaction</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("equity")]
    public decimal Equity { get; set; }
    /// <summary>
    /// <para>Unique identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("id")]
    public int Id { get; set; }
    /// <summary>
    /// <para>Additional information regarding transaction. Strongly dependent on the log entry type</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("info")]
    public JObject Info { get; set; } = null!;
    /// <summary>
    /// <para>Unique instrument identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_name")]
    public string InstrumentName { get; set; } = string.Empty;
    /// <summary>
    /// <para>Actual funding rate of trades and settlements on perpetual instruments</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("interest_pl")]
    public decimal InterestPl { get; set; }
    /// <summary>
    /// <para>Market price during the trade</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("mark_price")]
    public decimal MarkPrice { get; set; }
    /// <summary>
    /// <para>Unique order identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("order_id")]
    public string OrderId { get; set; } = string.Empty;
    /// <summary>
    /// <para>Updated position size after the transaction</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("position")]
    public decimal Position { get; set; }
    /// <summary>
    /// <para>Settlement/delivery price or the price level of the traded contracts</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("price")]
    public decimal Price { get; set; }
    /// <summary>
    /// <para>Currency symbol associated with the <c>price</c> field value</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("price_currency")]
    public string PriceCurrency { get; set; } = string.Empty;
    /// <summary>
    /// <para>Indicator informing whether the cashflow is waiting for settlement or not</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("profit_as_cashflow")]
    public bool ProfitAsCashflow { get; set; }
    /// <summary>
    /// <para>Session realized profit and loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("session_rpl")]
    public decimal SessionRpl { get; set; }
    /// <summary>
    /// <para>Session unrealized profit and loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("session_upl")]
    public decimal SessionUpl { get; set; }
    /// <summary>
    /// <para>One of: <c>short</c> or <c>long</c> in case of settlements, <c>close sell</c> or <c>close buy</c> in case of deliveries, <c>open sell</c>, <c>open buy</c>, <c>close sell</c>, <c>close buy</c> in case of trades</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("side")]
    public string Side { get; set; } = string.Empty;
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime Timestamp { get; set; }
    /// <summary>
    /// <para>Total session funding rate</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("total_interest_pl")]
    public decimal TotalInterestPl { get; set; }
    /// <summary>
    /// <para>Unique (per currency) trade identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("trade_id")]
    public string TradeId { get; set; } = string.Empty;
    /// <summary>
    /// <para>Transaction category/type. The most common are: <c>trade</c>, <c>deposit</c>, <c>withdrawal</c>, <c>settlement</c>, <c>delivery</c>, <c>transfer</c>, <c>swap</c>, <c>correction</c>. New types can be added any time in the future</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("type")]
    public string Type { get; set; } = string.Empty;
    /// <summary>
    /// <para>Unique user identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("user_id")]
    public int UserId { get; set; }
    /// <summary>
    /// <para>Trade role of the user: maker or taker</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("user_role")]
    public string UserRole { get; set; } = string.Empty;
    /// <summary>
    /// <para>Sequential identifier of user transaction</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("user_seq")]
    public int UserSeq { get; set; }
    /// <summary>
    /// <para>System name or user defined subaccount alias</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("username")]
    public string Username { get; set; } = string.Empty;
  }
}