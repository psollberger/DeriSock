namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class SubAccountPortfolio
  {
    [JsonProperty("available_funds")]
    public decimal AvailableFunds { get; set; }

    [JsonProperty("available_withdrawal_funds")]
    public decimal AvailableWithdrawalFunds { get; set; }

    [JsonProperty("balance")]
    public decimal Balance { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("equity")]
    public decimal Equity { get; set; }

    [JsonProperty("initial_margin")]
    public decimal InitialMargin { get; set; }

    [JsonProperty("maintenance_margin")]
    public decimal MaintenanceMargin { get; set; }

    [JsonProperty("margin_balance")]
    public decimal MarginBalance { get; set; }
  }
}
