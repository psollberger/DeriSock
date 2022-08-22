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
  public partial class CurrencyData
  {
    /// <summary>
    /// <para>The type of the currency.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("coin_type")]
    public string CoinType { get; set; } = string.Empty;
    /// <summary>
    /// <para>The abbreviation of the currency. This abbreviation is used elsewhere in the API to identify the currency.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("currency")]
    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// <para>The full name for the currency.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("currency_long")]
    public string CurrencyLong { get; set; } = string.Empty;
    /// <summary>
    /// <para>fee precision</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("fee_precision")]
    public int FeePrecision { get; set; }
    /// <summary>
    /// <para>Minimum number of block chain confirmations before deposit is accepted.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("min_confirmations")]
    public int MinConfirmations { get; set; }
    /// <summary>
    /// <para>The minimum transaction fee paid for withdrawals</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("min_withdrawal_fee")]
    public decimal MinWithdrawalFee { get; set; }
    /// <summary>
    /// <para>The total transaction fee paid for withdrawals</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("withdrawal_fee")]
    public decimal WithdrawalFee { get; set; }
    /// <summary>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("withdrawal_priorities")]
    public WithdrawalPriority[] WithdrawalPriorities { get; set; } = System.Array.Empty<WithdrawalPriority>();
  }
}
