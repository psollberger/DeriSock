namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class TickerNotification
{
  /// <summary>
  ///   (Only for option) implied volatility for best ask
  /// </summary>
  [JsonProperty("ask_iv")]
  public decimal AskIv { get; set; }

  /// <summary>
  ///   It represents the requested order size of all best asks
  /// </summary>
  [JsonProperty("best_ask_amount")]
  public decimal BestAskAmount { get; set; }

  /// <summary>
  ///   The current best ask price, null if there aren't any asks
  /// </summary>
  [JsonProperty("best_ask_price")]
  public decimal BestAskPrice { get; set; }

  /// <summary>
  ///   It represents the requested order size of all best bids
  /// </summary>
  [JsonProperty("best_bid_amount")]
  public decimal BestBidAmount { get; set; }

  /// <summary>
  ///   The current best bid price, null if there aren't any bids
  /// </summary>
  [JsonProperty("best_bid_price")]
  public decimal BestBidPrice { get; set; }

  /// <summary>
  ///   (Only for option) implied volatility for best bid
  /// </summary>
  [JsonProperty("bid_iv")]
  public decimal BidIv { get; set; }

  /// <summary>
  ///   Current funding (perpetual only)
  /// </summary>
  [JsonProperty("current_funding")]
  public decimal CurrentFunding { get; set; }

  /// <summary>
  ///   The settlement price for the instrument. Only when state = closed
  /// </summary>
  [JsonProperty("delivery_price")]
  public decimal DeliveryPrice { get; set; }

  /// <summary>
  ///   Funding 8h (perpetual only)
  /// </summary>
  [JsonProperty("funding_8h")]
  public decimal Funding8H { get; set; }

  /// <summary>
  ///   greeks
  /// </summary>
  [JsonProperty("greeks")]
  public Greeks Greeks { get; set; }

  /// <summary>
  ///   Current index price
  /// </summary>
  [JsonProperty("index_price")]
  public decimal IndexPrice { get; set; }

  /// <summary>
  ///   Unique instrument identifier
  /// </summary>
  [JsonProperty("instrument_name")]
  public string InstrumentName { get; set; }

  /// <summary>
  ///   Interest rate used in implied volatility calculations (options only)
  /// </summary>
  [JsonProperty("interest_rate")]
  public decimal InterestRate { get; set; }

  /// <summary>
  ///   The price for the last trade
  /// </summary>
  [JsonProperty("last_price")]
  public decimal? LastPrice { get; set; }

  /// <summary>
  ///   (Only for option) implied volatility for mark price
  /// </summary>
  [JsonProperty("mark_iv")]
  public decimal MarkIv { get; set; }

  /// <summary>
  ///   The mark price for the instrument
  /// </summary>
  [JsonProperty("mark_price")]
  public decimal MarkPrice { get; set; }

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
  ///   The total amount of outstanding contracts in the corresponding amount units. For perpetual and futures the amount is
  ///   in USD units, for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.
  /// </summary>
  [JsonProperty("open_interest")]
  public decimal OpenInterest { get; set; }

  /// <summary>
  ///   The settlement price for the instrument. Only when state = open
  /// </summary>
  [JsonProperty("settlement_price")]
  public decimal SettlementPrice { get; set; }

  /// <summary>
  ///   The state of the order book. Possible values are open and closed.
  /// </summary>
  [JsonProperty("state")]
  public string State { get; set; }

  /// <summary>
  ///   stats
  /// </summary>
  [JsonProperty("stats")]
  public Statistics Stats { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("timestamp")]
  public long Timestamp { get; set; }

  /// <inheritdoc cref="Timestamp" />
  [JsonIgnore]
  public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();

  /// <summary>
  ///   Name of the underlying future, or index_price (options only)
  /// </summary>
  [JsonProperty("underlying_index")]
  public string UnderlyingIndex { get; set; }

  /// <summary>
  ///   Underlying price for implied volatility calculations (options only)
  /// </summary>
  [JsonProperty("underlying_price")]
  public decimal UnderlyingPrice { get; set; }
  
  public InstrumentType InstrumentType => GetInstrumentType();

  public OptionType OptionType => GetOptionType();
  
  private OptionType GetOptionType()
  {
    if (InstrumentName.EndsWith("-C"))
      return OptionType.Call;
    if (InstrumentName.EndsWith("-P"))
      return OptionType.Put;
    return OptionType.Undefined;
  }
  
  private InstrumentType GetInstrumentType()
  {
    if (InstrumentName.EndsWith("-C") || InstrumentName.EndsWith("-P"))
      return InstrumentType.Option;
    if (InstrumentName.EndsWith("-PERPETUAL"))
      return InstrumentType.Perpetual;
    if (char.IsDigit(InstrumentName[InstrumentName.Length-1]))
      return InstrumentType.Future;
    return InstrumentType.Undefined;
  }
}
