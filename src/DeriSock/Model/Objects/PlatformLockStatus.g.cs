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
  public partial class PlatformLockStatus
  {
    /// <summary>
    /// <para><c>true</c> when platform is locked in all currencies, <c>partial</c> when some currencies are locked, <c>false</c> - when there are not currencies locked</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("locked")]
    public string Locked { get; set; } = string.Empty;
    /// <summary>
    /// <para>List of currencies in which platform is locked</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("locked_currencies")]
    public string[] LockedCurrencies { get; set; } = System.Array.Empty<System.String>();
  }
}
