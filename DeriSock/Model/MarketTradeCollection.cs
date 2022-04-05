namespace DeriSock.Model;

using Newtonsoft.Json;

public class MarketTradeCollection
{
  [JsonProperty("has_more")]
  public bool HasMore { get; set; }

  [JsonProperty("trades")]
  public MarketTrade[] Trades { get; set; }
}
