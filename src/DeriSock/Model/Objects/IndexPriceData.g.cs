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
  public partial class IndexPriceData
  {
    /// <summary>
    /// <para>Estimated delivery price for the market. For more details, see Documentation &gt; General &gt; Expiration Price</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("estimated_delivery_price")]
    public decimal EstimatedDeliveryPrice { get; set; }
    /// <summary>
    /// <para>Value of requested index</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("index_price")]
    public decimal IndexPrice { get; set; }
  }
}
