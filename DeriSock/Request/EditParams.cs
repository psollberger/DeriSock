namespace DeriSock.Request;

public class EditParams
{
  /// <summary>
  ///   The order id
  /// </summary>
  public string OrderId { get; set; }

  /// <summary>
  ///   It represents the requested order size. For perpetual and futures the amount is in USD units, for options it is
  ///   amount of corresponding cryptocurrency contracts, e.g., BTC or ETH
  /// </summary>
  public decimal Amount { get; set; }

  /// <summary>
  ///   <para>The order price in base currency</para>
  ///   <para>When editing an option order with advanced=usd, the field price should be the option price value in USD.</para>
  ///   <para>
  ///     When editing an option with advanced=implv, the field price should be a value of implied volatility in percentages.
  ///     For example, price=100, means implied volatility of 100%
  ///   </para>
  /// </summary>
  public decimal Price { get; set; }

  /// <summary>
  ///   <para>
  ///     If true, the order is considered post-only. If the new price would cause the order to be filled immediately (as
  ///     taker), the price will be changed to be just below or above the spread (accordingly to the original order type).
  ///   </para>
  ///   <para>Only valid in combination with time_in_force="good_til_cancelled"</para>
  /// </summary>
  public bool? PostOnly { get; set; }

  /// <summary>
  ///   If true, the order is considered reduce-only which is intended to only reduce a current position
  /// </summary>
  public bool? ReduceOnly { get; set; }

  /// <summary>
  ///   <para>
  ///     If order is considered post-only and this field is set to true than order is put to order book unmodified or
  ///     request is rejected.
  ///   </para>
  ///   <para>Only valid in combination with "post_only" set to true</para>
  /// </summary>
  public bool? RejectPostOnly { get; set; }

  /// <summary>
  ///   <para>
  ///     Advanced option order type. If you have posted an advanced option order, it is necessary to re-supply this
  ///     parameter when editing it (Only for options)
  ///   </para>
  ///   <para>Enum: <c>usd</c>, <c>implv</c></para>
  /// </summary>
  public string Advanced { get; set; }

  /// <summary>
  ///   Stop price, required for stop limit orders (Only for stop orders)
  /// </summary>
  public decimal? StopPrice { get; set; }
}
