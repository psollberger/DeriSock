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
  public partial class Withdrawal
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
    /// <para>The timestamp (milliseconds since the Unix epoch) of withdrawal confirmation, <c>null</c> when not confirmed</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("confirmed_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime ConfirmedTimestamp { get; set; }
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("created_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime CreatedTimestamp { get; set; }
    /// <summary>
    /// <para>Currency, i.e <c>&quot;BTC&quot;</c>, <c>&quot;ETH&quot;</c>, <c>&quot;USDC&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("currency")]
    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// <para>Fee in currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("fee")]
    public decimal Fee { get; set; }
    /// <summary>
    /// <para>Withdrawal id in Deribit system</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("id")]
    public int Id { get; set; }
    /// <summary>
    /// <para>Id of priority level</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("priority")]
    public decimal Priority { get; set; }
    /// <summary>
    /// <para>Withdrawal state, allowed values : <c>unconfirmed</c>, <c>confirmed</c>, <c>cancelled</c>, <c>completed</c>, <c>interrupted</c>, <c>rejected</c></para>
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