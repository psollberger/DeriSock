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
  /// <para>Method of authentication</para>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class GrantType : DeriSock.Model.EnumValue
  {
    public static GrantType ClientCredentials = new GrantType("client_credentials");
    public static GrantType ClientSignature = new GrantType("client_signature");
    public static GrantType RefreshToken = new GrantType("refresh_token");
    private GrantType(string value) : 
        base(value)
    {
    }
  }
}
