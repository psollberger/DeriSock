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
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial class BookInstrumentNameGroupDepthIntervalRequest
  {
    /// <summary>
    /// <para>Instrument name</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_name")]
    public string InstrumentName { get; set; } = string.Empty;
    /// <summary>
    /// <para>Group prices (by rounding). Use <c>none</c> for no grouping.</para>
    /// <para>For ETH cryptocurrency, real <c>group</c> is divided by 100.0, e.g. given value <c>5</c> means using <c>0.05</c></para>
    /// <para>Allowed values for BTC - <c>none</c>, <c>1</c>, <c>2</c>, <c>5</c>, <c>10</c></para>
    /// <para>Allowed values for ETH - <c>none</c>, <c>5</c>, <c>10</c>, <c>25</c>, <c>100</c>, <c>250</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("group")]
    public PriceGrouping Group { get; set; } = null!;
    /// <summary>
    /// <para>Number of price levels to be included.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("depth")]
    public PriceLevelDepth Depth { get; set; } = null!;
    /// <summary>
    /// <para>Frequency of notifications. Events will be aggregated over this interval.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("interval")]
    public NotificationInterval1 Interval { get; set; } = null!;
  }
}