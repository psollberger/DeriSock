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
  public partial class DeribitPriceRankingChannel : DeriSock.Model.ISubscriptionChannel
  {
    /// <summary>
    /// <para>Index identifier, matches (base) cryptocurrency with quote currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("index_name")]
    public IndexName IndexName { get; set; } = null!;
    public string ToChannelName()
    {
      return $"deribit_price_ranking.{IndexName}";
    }
  }
}
