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
  /// <para>Combo state, if not provided combos of all states are considered</para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial class ComboState : DeriSock.Model.EnumValue
  {
    public static ComboState Rfq = new ComboState("rfq");
    public static ComboState Active = new ComboState("active");
    public static ComboState Inactive = new ComboState("inactive");
    private ComboState(string value) : 
        base(value)
    {
    }
  }
}