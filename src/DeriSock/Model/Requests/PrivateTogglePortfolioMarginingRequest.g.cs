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
  public partial class PrivateTogglePortfolioMarginingRequest
  {
    /// <summary>
    /// <para>Id of a (sub)account</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("user_id")]
    public int UserId { get; set; }
    /// <summary>
    /// <para>Whether PM or SM should be enabled - PM while <c>true</c>, SM otherwise</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("enabled")]
    public bool Enabled { get; set; }
    /// <summary>
    /// <para>If <c>true</c> request returns the result without switching the margining model. Default: <c>false</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("dry_run")]
    public bool? DryRun { get; set; }
  }
}
