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
  public partial class PublicGetLastSettlementsByInstrumentRequest
  {
    /// <summary>
    /// <para>Instrument name</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_name")]
    public string InstrumentName { get; set; } = string.Empty;
    /// <summary>
    /// <para>Settlement type</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("type")]
    public SettlementType? Type { get; set; }
    /// <summary>
    /// <para>Number of requested items, default - <c>20</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("count")]
    public int? Count { get; set; }
    /// <summary>
    /// <para>Continuation token for pagination</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("continuation")]
    public string? Continuation { get; set; }
    /// <summary>
    /// <para>The latest timestamp to return result for (milliseconds since the UNIX epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("search_start_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime? SearchStartTimestamp { get; set; }
  }
}
