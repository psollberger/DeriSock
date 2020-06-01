namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class MarketSettlementCollection
  {
    /// <summary>
    ///   Continuation token for pagination.
    /// </summary>
    [JsonProperty("continuation")]
    public string Continuation { get; set; }

    /// <summary>
    ///   of object
    /// </summary>
    [JsonProperty("settlements")]
    public MarketSettlement[] Settlements { get; set; }
  }
}
