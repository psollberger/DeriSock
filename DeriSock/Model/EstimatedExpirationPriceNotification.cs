namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class EstimatedExpirationPriceNotification
  {
    /// <summary>
    ///   When true then prize is given as an estimated value, otherwise it's current index price
    /// </summary>
    [JsonProperty("is_estimated")]
    public bool IsEstimated { get; set; }

    /// <summary>
    ///   Index current or estimated price
    /// </summary>
    [JsonProperty("price")]
    public decimal Price { get; set; }

    /// <summary>
    ///   Number of seconds till finalizing the nearest instrument
    /// </summary>
    [JsonProperty("seconds")]
    public int Seconds { get; set; }
  }
}
