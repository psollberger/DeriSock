namespace DeriSock.Model;

using Newtonsoft.Json;

public class UserSettlementCollection
{
  /// <summary>
  ///   Continuation token for pagination
  /// </summary>
  [JsonProperty("continuation")]
  public string Continuation { get; set; }

  [JsonProperty("settlements")]
  public UserSettlement[] Settlements { get; set; }
}
