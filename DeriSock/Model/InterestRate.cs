namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class InterestRate
  {
    /// <summary>
    ///   Price in base currency
    /// </summary>
    [JsonProperty("index_price")]
    public decimal IndexPrice { get; set; }

    /// <summary>
    ///   1hour interest rate
    /// </summary>
    [JsonProperty("interest_1h")]
    public decimal Interest1H { get; set; }

    /// <summary>
    ///   8hour interest rate
    /// </summary>
    [JsonProperty("interest_8h")]
    public decimal Interest8H { get; set; }

    /// <summary>
    ///   Price in base currency
    /// </summary>
    [JsonProperty("prev_index_price")]
    public decimal PrevIndexPrice { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <inheritdoc cref="Timestamp" />
    [JsonIgnore]
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();
  }
}
