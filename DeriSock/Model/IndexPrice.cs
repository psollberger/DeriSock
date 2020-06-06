namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class IndexPrice
  {
    /// <summary>
    ///   The current index price for BTC-USD (only for selected currency == BTC)
    /// </summary>
    [JsonProperty("BTC")]
    public decimal Btc { get; set; }

    /// <summary>
    ///   The current index price for ETH-USD (only for selected currency == ETH)
    /// </summary>
    [JsonProperty("ETH")]
    public decimal Eth { get; set; }

    /// <summary>
    ///   Estimated delivery price for the currency. For more details, see Documentation > General > Expiration Price
    /// </summary>
    [JsonProperty("edp")]
    public decimal Edp { get; set; }
  }
}
