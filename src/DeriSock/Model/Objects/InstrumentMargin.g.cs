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
  public partial class InstrumentMargin
  {
    /// <summary>
    /// <para>Margin when buying</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("buy")]
    public decimal Buy { get; set; }
    /// <summary>
    /// <para>The maximum price for the future. Any buy orders you submit higher than this price, will be clamped to this maximum.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("max_price")]
    public decimal MaxPrice { get; set; }
    /// <summary>
    /// <para>The minimum price for the future. Any sell orders you submit lower than this price will be clamped to this minimum.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("min_price")]
    public decimal MinPrice { get; set; }
    /// <summary>
    /// <para>Margin when selling</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("sell")]
    public decimal Sell { get; set; }
  }
}
