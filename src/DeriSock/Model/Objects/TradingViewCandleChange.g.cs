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
  public partial class TradingViewCandleChange
  {
    /// <summary>
    /// <para>The close price for the candle</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("close")]
    public decimal Close { get; set; }
    /// <summary>
    /// <para>Cost data for the candle</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("cost")]
    public decimal Cost { get; set; }
    /// <summary>
    /// <para>The highest price level for the candle</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("high")]
    public decimal High { get; set; }
    /// <summary>
    /// <para>The lowest price level for the candle</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("low")]
    public decimal Low { get; set; }
    /// <summary>
    /// <para>The open price for the candle&apos;</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("open")]
    public decimal Open { get; set; }
    /// <summary>
    /// <para>The timestamp (milliseconds since the Unix epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("tick")]
    public long Tick { get; set; }
    /// <summary>
    /// <para>Volume data for the candle</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("volume")]
    public decimal Volume { get; set; }
  }
}
