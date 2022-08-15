namespace DeriSock.Model;

using Newtonsoft.Json;

public class SettlementCollection
{
  /// <summary>
  ///   Continuation token for pagination
  /// </summary>
  [JsonProperty("continuation")]
  public string Continuation { get; set; }

  [JsonProperty("settlements")]
  public SettlementEntry[] Settlements { get; set; }
}
