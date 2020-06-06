namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class ChartTradesNotification
  {
    /// <summary>
    ///   The close price for the candle
    /// </summary>
    [JsonProperty("close")]
    public decimal Close { get; set; }

    /// <summary>
    ///   Cost data for the candle
    /// </summary>
    [JsonProperty("cost")]
    public decimal Cost { get; set; }

    /// <summary>
    ///   The highest price level for the candle
    /// </summary>
    [JsonProperty("high")]
    public decimal High { get; set; }

    /// <summary>
    ///   The lowest price level for the candle
    /// </summary>
    [JsonProperty("low")]
    public decimal Low { get; set; }

    /// <summary>
    ///   The open price for the candle'
    /// </summary>
    [JsonProperty("open")]
    public decimal Open { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("tick")]
    public long Tick { get; set; }

    /// <inheritdoc cref="Tick" />
    [JsonIgnore]
    public DateTime TickDateTime => Tick.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Volume data for the candle
    /// </summary>
    [JsonProperty("volume")]
    public decimal Volume { get; set; }
  }
}
