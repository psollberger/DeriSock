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
  /// <para>Describes if user wants to be maker or taker of trades</para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial class TradeRole : DeriSock.Model.EnumValue
  {
    public static TradeRole Maker = new TradeRole("maker");
    public static TradeRole Taker = new TradeRole("taker");
    private TradeRole(string value) : 
        base(value)
    {
    }
  }
}
