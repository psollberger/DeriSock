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
  /// <para>Order type - <c>limit</c>, <c>stop</c>, <c>take</c>, <c>trigger_all</c> or <c>all</c>, default - <c>all</c></para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class CancelOrderType : DeriSock.Model.EnumValue
  {
    public static CancelOrderType All = new CancelOrderType("all");
    public static CancelOrderType Limit = new CancelOrderType("limit");
    public static CancelOrderType TriggerAll = new CancelOrderType("trigger_all");
    public static CancelOrderType Stop = new CancelOrderType("stop");
    public static CancelOrderType Take = new CancelOrderType("take");
    public static CancelOrderType TrailingStop = new CancelOrderType("trailing_stop");
    private CancelOrderType(string value) : 
        base(value)
    {
    }
  }
}
