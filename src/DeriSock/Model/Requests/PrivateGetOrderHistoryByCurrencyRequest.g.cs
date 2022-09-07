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
  public partial class PrivateGetOrderHistoryByCurrencyRequest
  {
    /// <summary>
    /// <para>The currency symbol</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("currency")]
    public CurrencySymbol Currency { get; set; } = null!;
    /// <summary>
    /// <para>Instrument kind, <c>&quot;combo&quot;</c> for any combo or <c>&quot;any&quot;</c> for all. If not provided instruments of all kinds are considered</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("kind")]
    public InstrumentKind? Kind { get; set; }
    /// <summary>
    /// <para>Number of requested items, default - <c>20</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("count")]
    public long? Count { get; set; }
    /// <summary>
    /// <para>The offset for pagination, default - <c>0</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("offset")]
    public long? Offset { get; set; }
    /// <summary>
    /// <para>Include in result orders older than 2 days, default - <c>false</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("include_old")]
    public bool? IncludeOld { get; set; }
    /// <summary>
    /// <para>Include in result fully unfilled closed orders, default - <c>false</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("include_unfilled")]
    public bool? IncludeUnfilled { get; set; }
  }
}
