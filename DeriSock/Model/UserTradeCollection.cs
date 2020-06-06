namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class UserTradeCollection
  {
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("trades")]
    public UserTrade[] Trades { get; set; }
  }
}
