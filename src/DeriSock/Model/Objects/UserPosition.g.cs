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
  public partial class UserPosition
  {
    /// <summary>
    /// <para>Average price of trades that built this position</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("average_price")]
    public decimal AveragePrice { get; set; }
    /// <summary>
    /// <para>Only for options, average price in USD</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("average_price_usd")]
    public decimal? AveragePriceUsd { get; set; }
    /// <summary>
    /// <para>Delta parameter</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("delta")]
    public decimal Delta { get; set; }
    /// <summary>
    /// <para>Direction: <c>buy</c>, <c>sell</c> or <c>zero</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("direction")]
    public string Direction { get; set; } = string.Empty;
    /// <summary>
    /// <para>Estimated liquidation price, added only for futures, for non portfolio margining users</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("estimated_liquidation_price")]
    public decimal? EstimatedLiquidationPrice { get; set; }
    /// <summary>
    /// <para>Floating profit or loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("floating_profit_loss")]
    public decimal FloatingProfitLoss { get; set; }
    /// <summary>
    /// <para>Only for options, floating profit or loss in USD</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("floating_profit_loss_usd")]
    public decimal? FloatingProfitLossUsd { get; set; }
    /// <summary>
    /// <para>Only for options, Gamma parameter</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("gamma")]
    public decimal? Gamma { get; set; }
    /// <summary>
    /// <para>Current index price</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("index_price")]
    public decimal IndexPrice { get; set; }
    /// <summary>
    /// <para>Initial margin</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("initial_margin")]
    public decimal InitialMargin { get; set; }
    /// <summary>
    /// <para>Unique instrument identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("instrument_name")]
    public string InstrumentName { get; set; } = string.Empty;
    /// <summary>
    /// <para>Value used to calculate <c>realized_funding</c> (perpetual only)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("interest_value")]
    public decimal? InterestValue { get; set; }
    /// <summary>
    /// <para>Instrument kind: <c>&quot;future&quot;</c>, <c>&quot;option&quot;</c>, <c>&quot;future_combo&quot;</c>, <c>&quot;option_combo&quot;</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("kind")]
    public string Kind { get; set; } = string.Empty;
    /// <summary>
    /// <para>Current available leverage for future position</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("leverage")]
    public long Leverage { get; set; }
    /// <summary>
    /// <para>Maintenance margin</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("maintenance_margin")]
    public decimal MaintenanceMargin { get; set; }
    /// <summary>
    /// <para>Current mark price for position&apos;s instrument</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("mark_price")]
    public decimal MarkPrice { get; set; }
    /// <summary>
    /// <para>Open orders margin</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("open_orders_margin")]
    public decimal OpenOrdersMargin { get; set; }
    /// <summary>
    /// <para>Realized Funding in current session included in session realized profit or loss, only for positions of perpetual instruments</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("realized_funding")]
    public decimal? RealizedFunding { get; set; }
    /// <summary>
    /// <para>Realized profit or loss</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("realized_profit_loss")]
    public decimal RealizedProfitLoss { get; set; }
    /// <summary>
    /// <para>Last settlement price for position&apos;s instrument 0 if instrument wasn&apos;t settled yet</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("settlement_price")]
    public decimal SettlementPrice { get; set; }
    /// <summary>
    /// <para>Position size for futures size in quote currency (e.g. USD), for options size is in base currency (e.g. BTC)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("size")]
    public decimal Size { get; set; }
    /// <summary>
    /// <para>Only for futures, position size in base currency</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("size_currency")]
    public decimal? SizeCurrency { get; set; }
    /// <summary>
    /// <para>Only for options, Theta parameter</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("theta")]
    public decimal? Theta { get; set; }
    /// <summary>
    /// <para>Profit or loss from position</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("total_profit_loss")]
    public decimal TotalProfitLoss { get; set; }
    /// <summary>
    /// <para>Only for options, Vega parameter</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("vega")]
    public decimal? Vega { get; set; }
  }
}
