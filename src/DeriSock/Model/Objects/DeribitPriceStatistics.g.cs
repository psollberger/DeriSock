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
  public partial class DeribitPriceStatistics
  {
    /// <summary>
    /// <para>The price index change calculated between the first and last point within most recent 24 hours window</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("change24h")]
    public decimal Change24h { get; set; }
    /// <summary>
    /// <para>The highest recorded price within the last 24 hours</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("high24h")]
    public decimal High24h { get; set; }
    /// <summary>
    /// <para>Indicates the high volatility periods on the market. The value <c>true</c> is set when the price index value drastically changed within the last 5 minutes</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("high_volatility")]
    public bool HighVolatility { get; set; }
    /// <summary>
    /// <para>Index identifier, matches (base) cryptocurrency with quote currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("index_name")]
    public string IndexName { get; set; } = string.Empty;
    /// <summary>
    /// <para>The lowest recorded price within the last 24 hours</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("low24h")]
    public decimal Low24h { get; set; }
  }
}
