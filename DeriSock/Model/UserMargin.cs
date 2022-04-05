namespace DeriSock.Model;

using Newtonsoft.Json;

public class UserMargin
{
  /// <summary>
  ///   Margin when buying
  /// </summary>
  [JsonProperty("buy")]
  public decimal Buy { get; set; }

  /// <summary>
  ///   The maximum price for the future. Any buy orders you submit higher than this price, will be clamped to this maximum.
  /// </summary>
  [JsonProperty("max_price")]
  public decimal MaxPrice { get; set; }

  /// <summary>
  ///   The minimum price for the future. Any sell orders you submit lower than this price will be clamped to this minimum.
  /// </summary>
  [JsonProperty("min_price")]
  public decimal MinPrice { get; set; }

  /// <summary>
  ///   Margin when selling
  /// </summary>
  [JsonProperty("sell")]
  public decimal Sell { get; set; }
}
