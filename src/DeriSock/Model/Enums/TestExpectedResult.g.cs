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
  /// <para>The value &quot;exception&quot; will trigger an error response. This may be useful for testing wrapper libraries.</para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class TestExpectedResult : DeriSock.Model.EnumValue
  {
    public static TestExpectedResult None = new TestExpectedResult("");
    public static TestExpectedResult Exception = new TestExpectedResult("exception");
    private TestExpectedResult(string value) : 
        base(value)
    {
    }
  }
}
