namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class Instrument
  {
    /// <summary>
    ///   The underlying currency being traded.
    /// </summary>
    [JsonProperty("base_currency")]
    public string BaseCurrency { get; set; }

    /// <summary>
    ///   Contract size for instrument
    /// </summary>
    [JsonProperty("contract_size")]
    public int ContractSize { get; set; }

    /// <summary>
    ///   The time when the instrument was first created (milliseconds)
    /// </summary>
    [JsonProperty("creation_timestamp")]
    public long CreationTimestamp { get; set; }

    /// <inheritdoc cref="CreationTimestamp" />
    [JsonIgnore]
    public DateTime CreationDateTime => CreationTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   The time when the instrument will expire (milliseconds)
    /// </summary>
    [JsonProperty("expiration_timestamp")]
    public long ExpirationTimestamp { get; set; }

    /// <inheritdoc cref="ExpirationTimestamp" />
    [JsonIgnore]
    public DateTime ExpirationDateTime => ExpirationTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   Indicates if the instrument can currently be traded.
    /// </summary>
    [JsonProperty("is_active")]
    public bool IsActive { get; set; }

    /// <summary>
    ///   Instrument kind, "future" or "option"
    /// </summary>
    [JsonProperty("kind")]
    public string Kind { get; set; }

    /// <summary>
    ///   Maximal leverage for instrument, for futures only
    /// </summary>
    [JsonProperty("leverage")]
    public int Leverage { get; set; }

    /// <summary>
    ///   Maker commission for instrument
    /// </summary>
    [JsonProperty("maker_commission")]
    public decimal MakerCommission { get; set; }

    /// <summary>
    ///   Minimum amount for trading. For perpetual and futures - in USD units, for options it is amount of corresponding
    ///   cryptocurrency contracts, e.g., BTC or ETH.
    /// </summary>
    [JsonProperty("min_trade_amount")]
    public decimal MinTradeAmount { get; set; }

    /// <summary>
    ///   The option type (only for options)
    /// </summary>
    [JsonProperty("option_type")]
    public string OptionType { get; set; }

    /// <summary>
    ///   The currency in which the instrument prices are quoted.
    /// </summary>
    [JsonProperty("quote_currency")]
    public string QuoteCurrency { get; set; }

    /// <summary>
    ///   The settlement period.
    /// </summary>
    [JsonProperty("settlement_period")]
    public string SettlementPeriod { get; set; }

    /// <summary>
    ///   The strike value. (only for options)
    /// </summary>
    [JsonProperty("strike")]
    public decimal Strike { get; set; }

    /// <summary>
    ///   Taker commission for instrument
    /// </summary>
    [JsonProperty("taker_commission")]
    public decimal TakerCommission { get; set; }

    /// <summary>
    ///   specifies minimal price change and, as follows, the number of decimal places for instrument prices
    /// </summary>
    [JsonProperty("tick_size")]
    public decimal TickSize { get; set; }
  }
}
