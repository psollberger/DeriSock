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
  using System;
  using DeriSock.Converter;
  
  /// <summary>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial class AccountSummaryData
  {
    /// <summary>
    /// <para>Options summary gamma</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_gamma")]
    public decimal OptionsGamma { get; set; }
    /// <summary>
    /// <para>Projected maintenance margin</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("projected_maintenance_margin")]
    public decimal ProjectedMaintenanceMargin { get; set; }
    /// <summary>
    /// <para>System generated user nickname (available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("system_name")]
    public string? SystemName { get; set; }
    /// <summary>
    /// <para>The account&apos;s margin balance</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("margin_balance")]
    public decimal MarginBalance { get; set; }
    /// <summary>
    /// <para>Options value</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_value")]
    public decimal OptionsValue { get; set; }
    /// <summary>
    /// <para>Account name (given by user) (available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("username")]
    public string? Username { get; set; }
    /// <summary>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("limits")]
    public MatchingEngineLimits Limits { get; set; } = null!;
    /// <summary>
    /// <para>The account&apos;s current equity</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("equity")]
    public decimal Equity { get; set; }
    /// <summary>
    /// <para>Futures profit and Loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("futures_pl")]
    public decimal FuturesPl { get; set; }
    /// <summary>
    /// <para>Whether Security Key authentication is enabled (available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("security_keys_enabled")]
    public bool? SecurityKeysEnabled { get; set; }
    /// <summary>
    /// <para>User fees in case of any discounts (available when parameter <c>extended</c> = <c>true</c> and user has any discounts)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("fees")]
    public UserFee[]? Fees { get; set; }
    /// <summary>
    /// <para>Options session unrealized profit and Loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_session_upl")]
    public decimal OptionsSessionUpl { get; set; }
    /// <summary>
    /// <para>Account id (available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("id")]
    public int? Id { get; set; }
    /// <summary>
    /// <para>Options summary vega</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_vega")]
    public decimal OptionsVega { get; set; }
    /// <summary>
    /// <para>Optional identifier of the referrer (of the affiliation program, and available when parameter <c>extended</c> = <c>true</c>), which link was used by this account at registration. It coincides with suffix of the affiliation link path after <c>/reg-</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("referrer_id")]
    public string? ReferrerId { get; set; }
    /// <summary>
    /// <para>The selected currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("currency")]
    public string Currency { get; set; } = string.Empty;
    /// <summary>
    /// <para>Whether account is loginable using email and password (available when parameter <c>extended</c> = <c>true</c> and account is a subaccount)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("login_enabled")]
    public bool? LoginEnabled { get; set; }
    /// <summary>
    /// <para>Account type (available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("type")]
    public string? Type { get; set; }
    /// <summary>
    /// <para>Futures session realized profit and Loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("futures_session_rpl")]
    public decimal FuturesSessionRpl { get; set; }
    /// <summary>
    /// <para>Options summary theta</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_theta")]
    public decimal OptionsTheta { get; set; }
    /// <summary>
    /// <para><c>true</c> when portfolio margining is enabled for user</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("portfolio_margining_enabled")]
    public bool PortfolioMarginingEnabled { get; set; }
    /// <summary>
    /// <para>The sum of position deltas without positions that will expire during closest expiration</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("projected_delta_total")]
    public decimal ProjectedDeltaTotal { get; set; }
    /// <summary>
    /// <para>Session realized profit and loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("session_rpl")]
    public decimal SessionRpl { get; set; }
    /// <summary>
    /// <para>The sum of position deltas</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("delta_total")]
    public decimal DeltaTotal { get; set; }
    /// <summary>
    /// <para>Options profit and Loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_pl")]
    public decimal OptionsPl { get; set; }
    /// <summary>
    /// <para>The account&apos;s available to withdrawal funds</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("available_withdrawal_funds")]
    public decimal AvailableWithdrawalFunds { get; set; }
    /// <summary>
    /// <para>The maintenance margin.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("maintenance_margin")]
    public decimal MaintenanceMargin { get; set; }
    /// <summary>
    /// <para>The account&apos;s initial margin</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("initial_margin")]
    public decimal InitialMargin { get; set; }
    /// <summary>
    /// <para>The account&apos;s fee balance (it can be used to pay for fees)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("fee_balance")]
    public decimal FeeBalance { get; set; }
    /// <summary>
    /// <para><c>true</c> when the inter-user transfers are enabled for user (available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("interuser_transfers_enabled")]
    public bool? InteruserTransfersEnabled { get; set; }
    /// <summary>
    /// <para>Futures session unrealized profit and Loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("futures_session_upl")]
    public decimal FuturesSessionUpl { get; set; }
    /// <summary>
    /// <para>Options session realized profit and Loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_session_rpl")]
    public decimal OptionsSessionRpl { get; set; }
    /// <summary>
    /// <para>The account&apos;s available funds</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("available_funds")]
    public decimal AvailableFunds { get; set; }
    /// <summary>
    /// <para>User email (available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("email")]
    public string? Email { get; set; }
    /// <summary>
    /// <para>Time at which the account was created (milliseconds since the Unix epoch; available when parameter <c>extended</c> = <c>true</c>)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("creation_timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime CreationTimestamp { get; set; }
    /// <summary>
    /// <para>Session unrealized profit and loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("session_upl")]
    public decimal SessionUpl { get; set; }
    /// <summary>
    /// <para>Profit and loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("total_pl")]
    public decimal TotalPl { get; set; }
    /// <summary>
    /// <para>Options summary delta</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("options_delta")]
    public decimal OptionsDelta { get; set; }
    /// <summary>
    /// <para>The account&apos;s balance</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("balance")]
    public decimal Balance { get; set; }
    /// <summary>
    /// <para>Projected initial margin</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("projected_initial_margin")]
    public decimal ProjectedInitialMargin { get; set; }
    /// <summary>
    /// <para>The deposit address for the account (if available)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("deposit_address")]
    public string DepositAddress { get; set; } = string.Empty;
  }
}