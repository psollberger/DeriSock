namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class MarketSettlement
  {
    /// <summary>
    ///   funded amount (bankruptcy only)
    /// </summary>
    [JsonProperty("funded")]
    public decimal Funded { get; set; }

    /// <summary>
    ///   funding (in base currency ; settlement for perpetual product only)
    /// </summary>
    [JsonProperty("funding")]
    public decimal Funding { get; set; }

    /// <summary>
    ///   underlying index price at time of event (in quote currency; settlement and delivery only)
    /// </summary>
    [JsonProperty("index_price")]
    public decimal IndexPrice { get; set; }

    /// <summary>
    ///   instrument name (settlement and delivery only)
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   mark price for at the settlement time (in quote currency; settlement and delivery only)
    /// </summary>
    [JsonProperty("mark_price")]
    public decimal MarkPrice { get; set; }

    /// <summary>
    ///   position size (in quote currency; settlement and delivery only)
    /// </summary>
    [JsonProperty("position")]
    public decimal Position { get; set; }

    /// <summary>
    ///   profit and loss (in base currency; settlement and delivery only)
    /// </summary>
    [JsonProperty("profit_loss")]
    public decimal ProfitLoss { get; set; }

    /// <summary>
    ///   value of session bankruptcy (in base currency; bankruptcy only)
    /// </summary>
    // ReSharper disable once StringLiteralTypo
    [JsonProperty("session_bankrupcy")]
    public decimal SessionBankruptcy { get; set; }

    /// <summary>
    ///   total value of session profit and losses (in base currency)
    /// </summary>
    [JsonProperty("session_profit_loss")]
    public decimal SessionProfitLoss { get; set; }

    /// <summary>
    ///   total amount of paid taxes/fees (in base currency; bankruptcy only)
    /// </summary>
    [JsonProperty("session_tax")]
    public decimal SessionTax { get; set; }

    /// <summary>
    ///   rate of paid texes/fees (in base currency; bankruptcy only)
    /// </summary>
    [JsonProperty("session_tax_rate")]
    public decimal SessionTaxRate { get; set; }

    /// <summary>
    ///   the amount of the socialized losses (in base currency; bankruptcy only)
    /// </summary>
    [JsonProperty("socialized")]
    public decimal Socialized { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <inheritdoc cref="Timestamp" />
    [JsonIgnore]
    public DateTime TimestamDateTime => Timestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   The type of settlement. settlement, delivery or bankruptcy.
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }
  }
}
