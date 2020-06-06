namespace DeriSock.Request
{
  using Newtonsoft.Json;

  public class UserPortfolioSubscriptionParams : ISubscriptionChannel
  {
    /// <summary>
    ///   The currency symbol
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }

    public string ToChannelName()
    {
      return $"user.portfolio.{Currency}".ToLower();
    }
  }
}
