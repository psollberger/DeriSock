namespace DeriSock.Model;

using Newtonsoft.Json;

public class PerpetualUserTrades
{
  /// <summary>
  ///   Current interest
  /// </summary>
  [JsonProperty("current_interest")]
  public decimal CurrentInterest { get; set; }

  [JsonProperty("data")]
  public PerpetualUserTradesDataItem[] Data { get; set; }

  /// <summary>
  ///   Current interest 8h
  /// </summary>
  [JsonProperty("interest_8h")]
  public decimal Interest8H { get; set; }
}
