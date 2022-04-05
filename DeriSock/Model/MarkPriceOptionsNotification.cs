namespace DeriSock.Model;

using Newtonsoft.Json;

public class MarkPriceOptionsNotification
{
  /// <summary>
  ///   Unique instrument identifier
  /// </summary>
  [JsonProperty("instrument_name")]
  public string InstrumentName { get; set; }

  /// <summary>
  ///   Value of the volatility of the underlying instrument
  /// </summary>
  [JsonProperty("iv")]
  public decimal Iv { get; set; }

  /// <summary>
  ///   The mark price for the instrument
  /// </summary>
  [JsonProperty("mark_price")]
  public decimal MarkPrice { get; set; }
}
