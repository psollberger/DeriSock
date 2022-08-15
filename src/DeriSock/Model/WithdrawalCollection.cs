namespace DeriSock.Model;

using Newtonsoft.Json;

public class WithdrawalCollection
{
  /// <summary>
  ///   Total number of results available
  /// </summary>
  [JsonProperty("count")]
  public int Count { get; set; }

  [JsonProperty("data")]
  public WithdrawalEntry[] Data { get; set; }
}
