namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class UserOrderTrades
  {
    [JsonProperty("order")]
    public UserOrder Order { get; set; }

    [JsonProperty("trades")]
    public UserTrade[] Trades { get; set; }
  }
}
