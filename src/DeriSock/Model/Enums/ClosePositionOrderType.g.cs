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
  /// <para>The order type</para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class ClosePositionOrderType : DeriSock.Model.EnumValue
  {
    public static ClosePositionOrderType Limit = new ClosePositionOrderType("limit");
    public static ClosePositionOrderType Market = new ClosePositionOrderType("market");
    private ClosePositionOrderType(string value) : 
        base(value)
    {
    }
  }
}
