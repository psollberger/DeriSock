namespace DeriSock.Model;

using Newtonsoft.Json;

public class UserOrderMargin
{
  /// <summary>
  ///   Initial margin of order, in base currency
  /// </summary>
  [JsonProperty("initial_margin")]
  public decimal? InitialMargin { get; set; }

  /// <summary>
  ///   Unique order identifier
  /// </summary>
  [JsonProperty("order_id")]
  public string OrderId { get; set; }
}
