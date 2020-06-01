namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class TradeCollection
  {
    [JsonProperty("has_more")]
    public bool HasMore { get; set; }

    [JsonProperty("trades")]
    public Trade[] Trades { get; set; }
  }
}
