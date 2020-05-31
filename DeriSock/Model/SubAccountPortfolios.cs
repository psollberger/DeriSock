namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class SubAccountPortfolios
  {
    [JsonProperty("btc")]
    public SubAccountPortfolio Btc { get; set; }

    [JsonProperty("eth")]
    public SubAccountPortfolio Eth { get; set; }
  }
}
