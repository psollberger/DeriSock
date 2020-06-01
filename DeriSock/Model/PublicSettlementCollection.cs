namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class PublicSettlementCollection
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
    public PublicSettlement[] Settlements { get; set; }
  }
}
