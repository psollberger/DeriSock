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
  public partial class PrivateSetEmailLanguageRequest
  {
    /// <summary>
    /// <para>The abbreviated language name. Valid values include <c>&quot;en&quot;</c>, <c>&quot;ko&quot;</c>, <c>&quot;zh&quot;</c>,  <c>&quot;ja&quot;</c>, <c>&quot;ru&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("language")]
    public string Language { get; set; } = string.Empty;
  }
}