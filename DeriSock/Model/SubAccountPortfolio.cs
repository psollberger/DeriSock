namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class SubAccountPortfolio
  {
    [JsonProperty("available_funds")]
    public double AvailableFunds { get; set; }

    [JsonProperty("available_withdrawal_funds")]
    public double AvailableWithdrawalFunds { get; set; }

    [JsonProperty("balance")]
    public double Balance { get; set; }

    [JsonProperty("currency")]
    public string Currency { get; set; }

    [JsonProperty("equity")]
    public double Equity { get; set; }

    [JsonProperty("initial_margin")]
    public double InitialMargin { get; set; }

    [JsonProperty("maintenance_margin")]
    public double MaintenanceMargin { get; set; }

    [JsonProperty("margin_balance")]
    public double MarginBalance { get; set; }
  }
}
