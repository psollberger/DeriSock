namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class DepositCollection
  {
    /// <summary>
    ///   Total number of results available
    /// </summary>
    [JsonProperty("count")]
    public int Count { get; set; }

    [JsonProperty("data")]
    public DepositInfo[] Data { get; set; }
  }
}
