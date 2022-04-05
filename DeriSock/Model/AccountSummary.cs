namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

/// <summary>
///   User account summary
/// </summary>
public class AccountSummary
{
  /// <summary>
  ///   Options summary gamma
  /// </summary>
  [JsonProperty("options_gamma")]
  public decimal OptionsGamma { get; set; }

  /// <summary>
  ///   Projected maintenance margin (for portfolio margining users)
  /// </summary>
  [JsonProperty("projected_maintenance_margin")]
  public decimal ProjectedMaintenanceMargin { get; set; }

  /// <summary>
  ///   System generated user nickname (available when parameter <c>extended</c> is true)
  /// </summary>
  [JsonProperty("system_name")]
  public string SystemName { get; set; }

  /// <summary>
  ///   The account's margin balance
  /// </summary>
  [JsonProperty("margin_balance")]
  public decimal MarginBalance { get; set; }

  /// <summary>
  ///   Whether two factor authentication is enabled (available when parameter <c>extended</c> is true)
  /// </summary>
  [JsonProperty("tfa_enabled")]
  public bool TfaEnabled { get; set; }

  /// <summary>
  ///   Options value
  /// </summary>
  [JsonProperty("options_value")]
  public decimal OptionsValue { get; set; }

  /// <summary>
  ///   Account name (given by user) (available when parameter <c>extended</c> is true)
  /// </summary>
  [JsonProperty("username")]
  public string Username { get; set; }

  /// <summary>
  ///   Information about rate limits enforced on the account
  /// </summary>
  [JsonProperty("limits")]
  public LimitsItem Limits { get; set; }

  /// <summary>
  ///   The account's current equity
  /// </summary>
  [JsonProperty("equity")]
  public decimal Equity { get; set; }

  /// <summary>
  ///   Futures profit and loss
  /// </summary>
  [JsonProperty("futures_pl")]
  public decimal FuturesPl { get; set; }

  /// <summary>
  ///   User fees in case of any discounts (available when parameter <c>extended</c> is true and user has any discounts)
  /// </summary>
  [JsonProperty("fees")]
  public FeesItem[] Fees { get; set; }

  /// <summary>
  ///   Options session unrealized profit and loss
  /// </summary>
  [JsonProperty("options_session_upl")]
  public decimal OptionsSessionUpl { get; set; }

  /// <summary>
  ///   Account id (available when parameter <c>extended</c> is true)
  /// </summary>
  [JsonProperty("id")]
  public int Id { get; set; }

  /// <summary>
  ///   Options summary vega
  /// </summary>
  [JsonProperty("options_vega")]
  public decimal OptionsVega { get; set; }

  /// <summary>
  ///   Optional identifier of the referrer (of the affiliation program, and available when parameter <c>extended</c> is
  ///   true), which link was used by this account at registration. It coincides with suffix of the affiliation link path
  ///   after <c>/reg-</c>
  /// </summary>
  [JsonProperty("referrer_id")]
  public string ReferrerId { get; set; }

  /// <summary>
  ///   Session funding
  /// </summary>
  [JsonProperty("session_funding")]
  public decimal SessionFunding { get; set; }

  /// <summary>
  ///   The selected currency
  /// </summary>
  [JsonProperty("currency")]
  public string Currency { get; set; }

  /// <summary>
  ///   Account type (available when parameter <c>extended</c> is true)
  /// </summary>
  [JsonProperty("type")]
  public string Type { get; set; }

  /// <summary>
  ///   Futures session realized profit and loss
  /// </summary>
  [JsonProperty("futures_session_rpl")]
  public decimal FuturesSessionRpl { get; set; }

  /// <summary>
  ///   Options summary theta
  /// </summary>
  [JsonProperty("options_theta")]
  public decimal OptionsTheta { get; set; }

  /// <summary>
  ///   <c>true</c> when portfolio margining is enabled for user
  /// </summary>
  [JsonProperty("portfolio_margining_enabled")]
  public bool PortfolioMarginingEnabled { get; set; }

  /// <summary>
  ///   Session realized profit and loss
  /// </summary>
  [JsonProperty("session_rpl")]
  public decimal SessionRpl { get; set; }

  /// <summary>
  ///   The sum of all position deltas, EXCLUDING your collateral.
  /// </summary>
  [JsonProperty("delta_total")]
  public decimal DeltaTotal { get; set; }

  /// <summary>
  ///   The sum of position deltas + INCLUDING your collateral.
  /// </summary>
  [JsonIgnore]
  public decimal DeltaTotalWithCollateral => DeltaTotal + Equity;

  /// <summary>
  ///   Options profit and loss
  /// </summary>
  [JsonProperty("options_pl")]
  public decimal OptionsPl { get; set; }

  /// <summary>
  ///   The account's available to withdrawal funds
  /// </summary>
  [JsonProperty("available_withdrawal_funds")]
  public decimal AvailableWithdrawalFunds { get; set; }

  /// <summary>
  ///   The maintenance margin
  /// </summary>
  [JsonProperty("maintenance_margin")]
  public decimal MaintenanceMargin { get; set; }

  /// <summary>
  ///   The account's initial margin
  /// </summary>
  [JsonProperty("initial_margin")]
  public decimal InitialMargin { get; set; }

  /// <summary>
  ///   The account's estimated liquidation ratio.
  /// </summary>
  [JsonProperty("estimated_liquidation_ratio")]
  public decimal EstimatedLiquidationRatio { get; set; }

  /// <summary>
  ///   <c>true</c> when the inter-user transfers are enabled for user (available when parameter <c>extended</c> is true)
  /// </summary>
  [JsonProperty("interuser_transfers_enabled")]
  public bool InterUserTransfersEnabled { get; set; }

  /// <summary>
  ///   Futures session unrealized profit and loss
  /// </summary>
  [JsonProperty("futures_session_upl")]
  public decimal FuturesSessionUpl { get; set; }

  /// <summary>
  ///   Options session realized profit and loss
  /// </summary>
  [JsonProperty("options_session_rpl")]
  public decimal OptionsSessionRpl { get; set; }

  /// <summary>
  ///   The account's available funds
  /// </summary>
  [JsonProperty("available_funds")]
  public decimal AvailableFunds { get; set; }

  /// <summary>
  ///   User email (available when parameter <c>extended</c> is true)
  /// </summary>
  [JsonProperty("email")]
  public string Email { get; set; }

  /// <summary>
  ///   Time at which the account was created (milliseconds since the Unix epoch; available when parameter <c>extended</c> is
  ///   true)
  /// </summary>
  [JsonProperty("creation_timestamp")]
  public long CreationTimestamp { get; set; }

  /// <inheritdoc cref="CreationTimestamp" />
  [JsonIgnore]
  public DateTime CreationDateTime => CreationTimestamp.AsDateTimeFromMilliseconds();

  /// <summary>
  ///   Session unrealized profit and loss
  /// </summary>
  [JsonProperty("session_upl")]
  public decimal SessionUpl { get; set; }

  /// <summary>
  ///   Profit and loss
  /// </summary>
  [JsonProperty("total_pl")]
  public decimal TotalPl { get; set; }

  /// <summary>
  ///   Options summary delta
  /// </summary>
  [JsonProperty("options_delta")]
  public decimal OptionsDelta { get; set; }

  /// <summary>
  ///   Projected Delta total
  /// </summary>
  [JsonProperty("projected_delta_total")]
  public decimal ProjectedDeltaTotal { get; set; }

  /// <summary>
  ///   The account's balance
  /// </summary>
  [JsonProperty("balance")]
  public decimal Balance { get; set; }

  /// <summary>
  ///   Projected initial margin (for portfolio margining users)
  /// </summary>
  [JsonProperty("projected_initial_margin")]
  public decimal ProjectedInitialMargin { get; set; }

  /// <summary>
  ///   The deposit address for the account (if available)
  /// </summary>
  [JsonProperty("deposit_address")]
  public string DepositAddress { get; set; }

  /// <summary>
  ///   Various rate limits enforced on the account
  /// </summary>
  public class LimitsItem
  {
    /// <summary>
    ///   Field not included if limits for futures are not set
    /// </summary>
    [JsonProperty("futures")]
    public EngineMatchItem Futures { get; set; }

    /// <summary>
    ///   Number of matching engine requests per second allowed for user
    /// </summary>
    [JsonProperty("matching_engine")]
    public EngineMatchItem MatchingEngine { get; set; }

    /// <summary>
    ///   Number of non matching engine requests per second allowed for user
    /// </summary>
    [JsonProperty("non_matching_engine")]
    public EngineMatchItem NonMatchingEngine { get; set; }
  }

  /// <summary>
  ///   Matching engine limits holder
  /// </summary>
  public class EngineMatchItem
  {
    /// <summary>
    ///   Number of non matching engine requests per second allowed for user
    /// </summary>
    [JsonProperty("rate")]
    public int Rate { get; set; }

    /// <summary>
    ///   Number of non matching engine burst requests per second allowed for user
    /// </summary>
    [JsonProperty("burst")]
    public int Burst { get; set; }
  }

  /// <summary>
  ///   Information about a fee entry
  /// </summary>
  public class FeesItem
  {
    /// <summary>
    ///   The currency the fee applies to
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }

    /// <summary>
    ///   Fee type - <c>relative</c> if fee is calculated as a fraction of base instrument fee, <c>fixed</c> if fee is
    ///   calculated solely using user fee
    /// </summary>
    [JsonProperty("fee_type")]
    public string FeeType { get; set; }

    /// <summary>
    ///   Type of the instruments the fee applies to - <c>future</c> for future instruments (excluding perpetual),
    ///   <c>perpetual</c> for future perpetual instruments, <c>option</c> for options
    /// </summary>
    [JsonProperty("instrument_type")]
    public string InstrumentType { get; set; }

    /// <summary>
    ///   User fee as a maker
    /// </summary>
    [JsonProperty("maker_fee")]
    public decimal MakerFee { get; set; }

    /// <summary>
    ///   User fee as a taker
    /// </summary>
    [JsonProperty("taker_fee")]
    public decimal TakerFee { get; set; }
  }
}
