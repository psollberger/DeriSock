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
  /// <para>Direction of results sorting (<c>default</c> value means no sorting, results will be returned in order in which they left the database)</para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class UserTradeSortDirection : DeriSock.Model.EnumValue
  {
    public static UserTradeSortDirection Asc = new UserTradeSortDirection("asc");
    public static UserTradeSortDirection Desc = new UserTradeSortDirection("desc");
    public static UserTradeSortDirection Default = new UserTradeSortDirection("default");
    private UserTradeSortDirection(string value) : 
        base(value)
    {
    }
  }
}
