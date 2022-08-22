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
  public partial class Deposit
  {
    /// <summary>
    /// <para>Address in proper format for currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("address")]
    public string Address { get; set; } = string.Empty;
    /// <summary>
    /// <para>Amount of funds in given currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("amount")]
    public decimal Amount { get; set; }
    /// <summary>
    /// <para>Currency, i.e <c>&quot;BTC&quot;</c>, <c>&quot;ETH&quot;</c>, <c>&quot;USDC&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("currency")]
    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("received_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime ReceivedTimestamp { get; set; }
    /// <summary>
    /// <para>Deposit state, allowed values : <c>pending</c>, <c>completed</c>, <c>rejected</c>, <c>replaced</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("state")]
    public string State { get; set; } = string.Empty;
    /// <summary>
    /// <para>Transaction id in proper format for currency, <c>null</c> if id is not available</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("transaction_id")]
    public string TransactionId { get; set; } = string.Empty;
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("updated_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime UpdatedTimestamp { get; set; }
  }
}