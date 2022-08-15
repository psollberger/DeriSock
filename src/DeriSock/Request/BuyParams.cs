namespace DeriSock.Request;

public class BuyParams
{
  /// <summary>
  ///   Instrument name
  /// </summary>
  public string InstrumentName { get; set; }

  /// <summary>
  ///   It represents the requested order size. For perpetual and futures the amount is in USD units, for options it is
  ///   amount of corresponding cryptocurrency contracts, e.g., BTC or ETH
  /// </summary>
  public decimal Amount { get; set; }

  /// <summary>
  ///   <para>The order type, default: "limit"</para>
  ///   <para>Enum: <c>limit</c>, <c>stop_limit</c>, <c>market</c>, <c>stop_market</c></para>
  /// </summary>
  public string Type { get; set; }

  /// <summary>
  ///   user defined label for the order (maximum 64 characters)
  /// </summary>
  public string Label { get; set; }

  /// <summary>
  ///   <para>The order price in base currency (Only for limit and stop_limit orders)</para>
  ///   <para>When adding order with advanced=usd, the field price should be the option price value in USD.</para>
  ///   <para>
  ///     When adding order with advanced=implv, the field price should be a value of implied volatility in percentages.
  ///     For example, price=100, means implied volatility of 100%
  ///   </para>
  /// </summary>
  public decimal? Price { get; set; }

  /// <summary>
  ///   <para>Specifies how long the order remains in effect. Default <c>good_til_cancelled</c></para>
  ///   <para><c>good_til_cancelled</c> - unfilled order remains in order book until cancelled</para>
  ///   <para><c>fill_or_kill</c> - execute a transaction immediately and completely or not at all</para>
  ///   <para>
  ///     <c>immediate_or_cancel</c> - execute a transaction immediately, and any portion of the order that cannot be
  ///     immediately filled is cancelled
  ///   </para>
  /// </summary>
  public string TimeInForce { get; set; }

  /// <summary>
  ///   Maximum amount within an order to be shown to other customers, 0 for invisible order
  /// </summary>
  public decimal? MaxShow { get; set; }

  /// <summary>
  ///   <para>
  ///     If true, the order is considered post-only. If the new price would cause the order to be filled immediately (as
  ///     taker), the price will be changed to be just below the spread.
  ///   </para>
  ///   <para>Only valid in combination with time_in_force="good_til_cancelled"</para>
  /// </summary>
  public bool? PostOnly { get; set; }

  /// <summary>
  ///   <para>
  ///     If order is considered post-only and this field is set to true than order is put to order book unmodified or
  ///     request is rejected.
  ///   </para>
  ///   <para>Only valid in combination with "post_only" set to true</para>
  /// </summary>
  public bool? RejectPostOnly { get; set; }

  /// <summary>
  ///   If true, the order is considered reduce-only which is intended to only reduce a current position
  /// </summary>
  public bool? ReduceOnly { get; set; }

  /// <summary>
  ///   Stop price, required for stop limit orders (Only for stop orders)
  /// </summary>
  public decimal? StopPrice { get; set; }

  /// <summary>
  ///   <para>Defines trigger type, required for "stop_limit" order type</para>
  ///   <para>Enum: <c>index_price</c>, <c>mark_price</c>, <c>last_price</c></para>
  /// </summary>
  public string Trigger { get; set; }

  /// <summary>
  ///   <para>Advanced option order type. (Only for options)</para>
  ///   <para>Enum: <c>usd</c>, <c>implv</c></para>
  /// </summary>
  public string Advanced { get; set; }
}
