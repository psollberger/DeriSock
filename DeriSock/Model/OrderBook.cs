namespace DeriSock.Model;

using System;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class OrderBook
{
  /// <summary>
  ///   (Only for option) implied volatility for best ask
  /// </summary>
  [JsonProperty("ask_iv")]
  public decimal AskIv { get; set; }

  /// <summary>
  ///   of [price, amount]	List of asks
  /// </summary>
  [JsonProperty("asks")]
  public PriceItem[] Asks { get; set; }

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
  ///   of [price, amount]	List of bids
  /// </summary>
  [JsonProperty("bids")]
  public PriceItem[] Bids { get; set; }

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
  ///   Only for options
  /// </summary>
  [JsonProperty("greeks")]
  public Greeks? Greeks { get; set; }

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

  public InstrumentType InstrumentType => GetInstrumentType();

  public OptionType OptionType => GetOptionType();

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
  ///   The state of the order book. Possible values are <c>open</c> and <c>closed</c>
  /// </summary>
  [JsonProperty("state")]
  public string State { get; set; }

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

  private InstrumentType GetInstrumentType()
  {
    return InstrumentName switch
    {
      { } i when i.EndsWith("-C") || i.EndsWith("-P") => InstrumentType.Option,
      { } i when i.EndsWith("-PERPETUAL") => InstrumentType.Perpetual,
      { } i when i.Length >= 1 && char.IsDigit(i[i.Length - 1]) => InstrumentType.Future,
      _ => InstrumentType.Undefined
    };
  }

  private OptionType GetOptionType()
  {
    return InstrumentName switch
    {
      { } i when i.EndsWith("-C") => OptionType.Call,
      { } i when i.EndsWith("-P") => OptionType.Put,
      _ => OptionType.Undefined
    };
  }

  [JsonConverter(typeof(PriceItemConverter))]
  public class PriceItem
  {
    public decimal Price { get; set; }
    public decimal Amount { get; set; }
  }

  public class PriceItemConverter : JsonConverter<PriceItem>
  {
    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, PriceItem value, JsonSerializer serializer)
    {
      throw new NotSupportedException();
    }

    public override PriceItem ReadJson(
      JsonReader reader, Type objectType,
      PriceItem existingValue, bool hasExistingValue,
      JsonSerializer serializer)
    {
      var arr = JArray.Load(reader);

      return new PriceItem
      {
        Price = arr[0].Value<decimal>(), Amount = arr[1].Value<decimal>()
      };
    }
  }
}
