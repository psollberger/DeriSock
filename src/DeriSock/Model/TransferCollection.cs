namespace DeriSock.Model;

using Newtonsoft.Json;

public class TransferCollection
{
  /// <summary>
  ///   Total number of results available
  /// </summary>
  [JsonProperty("count")]
  public int Count { get; set; }

  [JsonProperty("data")]
  public TransferInfo[] Data { get; set; }
}
