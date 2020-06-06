namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class UserPortfolioNotification
  {
    /// <summary>
    ///   The account's available funds
    /// </summary>
    [JsonProperty("available_funds")]
    public decimal AvailableFunds { get; set; }

    /// <summary>
    ///   The account's available to withdrawal funds
    /// </summary>
    [JsonProperty("available_withdrawal_funds")]
    public decimal AvailableWithdrawalFunds { get; set; }

    /// <summary>
    ///   The account's balance
    /// </summary>
    [JsonProperty("balance")]
    public decimal Balance { get; set; }

    /// <summary>
    ///   The selected currency
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }

    /// <summary>
    ///   The sum of position deltas
    /// </summary>
    [JsonProperty("delta_total")]
    public decimal DeltaTotal { get; set; }

    /// <summary>
    ///   The account's current equity
    /// </summary>
    [JsonProperty("equity")]
    public decimal Equity { get; set; }

    /// <summary>
    ///   Futures profit and Loss
    /// </summary>
    [JsonProperty("futures_pl")]
    public decimal FuturesPl { get; set; }

    /// <summary>
    ///   Futures session realized profit and Loss
    /// </summary>
    [JsonProperty("futures_session_rpl")]
    public decimal FuturesSessionRpl { get; set; }

    /// <summary>
    ///   Futures session unrealized profit and Loss
    /// </summary>
    [JsonProperty("futures_session_upl")]
    public decimal FuturesSessionUpl { get; set; }

    /// <summary>
    ///   The account's initial margin
    /// </summary>
    [JsonProperty("initial_margin")]
    public decimal InitialMargin { get; set; }

    /// <summary>
    ///   The maintenance margin.
    /// </summary>
    [JsonProperty("maintenance_margin")]
    public decimal MaintenanceMargin { get; set; }

    /// <summary>
    ///   The account's margin balance
    /// </summary>
    [JsonProperty("margin_balance")]
    public decimal MarginBalance { get; set; }

    /// <summary>
    ///   Options summary delta
    /// </summary>
    [JsonProperty("options_delta")]
    public decimal OptionsDelta { get; set; }

    /// <summary>
    ///   Options summary gamma
    /// </summary>
    [JsonProperty("options_gamma")]
    public decimal OptionsGamma { get; set; }

    /// <summary>
    ///   Options profit and Loss
    /// </summary>
    [JsonProperty("options_pl")]
    public decimal OptionsPl { get; set; }

    /// <summary>
    ///   Options session realized profit and Loss
    /// </summary>
    [JsonProperty("options_session_rpl")]
    public decimal OptionsSessionRpl { get; set; }

    /// <summary>
    ///   Options session unrealized profit and Loss
    /// </summary>
    [JsonProperty("options_session_upl")]
    public decimal OptionsSessionUpl { get; set; }

    /// <summary>
    ///   Options summary theta
    /// </summary>
    [JsonProperty("options_theta")]
    public decimal OptionsTheta { get; set; }

    /// <summary>
    ///   Options value
    /// </summary>
    [JsonProperty("options_value")]
    public decimal OptionsValue { get; set; }

    /// <summary>
    ///   Options summary vega
    /// </summary>
    [JsonProperty("options_vega")]
    public decimal OptionsVega { get; set; }

    /// <summary>
    ///   When true portfolio margining is enabled for user
    /// </summary>
    [JsonProperty("portfolio_margining_enabled")]
    public bool PortfolioMarginingEnabled { get; set; }

    /// <summary>
    ///   Projected initial margin (for portfolio margining users)
    /// </summary>
    [JsonProperty("projected_initial_margin")]
    public decimal ProjectedInitialMargin { get; set; }

    /// <summary>
    ///   Projected maintenance margin (for portfolio margining users)
    /// </summary>
    [JsonProperty("projected_maintenance_margin")]
    public decimal ProjectedMaintenanceMargin { get; set; }

    /// <summary>
    ///   Session funding
    /// </summary>
    [JsonProperty("session_funding")]
    public decimal SessionFunding { get; set; }

    /// <summary>
    ///   Session realized profit and loss
    /// </summary>
    [JsonProperty("session_rpl")]
    public decimal SessionRpl { get; set; }

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
  }
}
