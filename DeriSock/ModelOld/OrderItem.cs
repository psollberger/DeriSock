namespace DeriSock.Model
{
  using System.Diagnostics;

  [DebuggerDisplay("{" + nameof(ToDebuggerDisplay) + "(),nq}")]
  public class OrderItem
  {
    /// <summary>
    ///   Amount in USD
    /// </summary>
    public long amount;

    /// <summary>
    ///   Direction of the order (buy/sell)
    /// </summary>
    public string direction;

    /// <summary>
    ///   Amount in USD that's already fullfilled
    /// </summary>
    public long filled_amount;

    /// <summary>
    ///   Is this in Open Orders?
    /// </summary>
    public bool is_open;

    /// <summary>
    ///   Is used in another order (e.g. buy order is used in a sell order)
    /// </summary>
    public bool is_used;

    /// <summary>
    ///   user defined label (up to 32 characters)
    /// </summary>
    public string label;

    /// <summary>
    ///   order_id if open ; can be null/empty
    /// </summary>
    public string order_id;

    /// <summary>
    ///   The price per currency (e.g. price for one BTC)
    /// </summary>
    public double price;

    private string ToDebuggerDisplay()
    {
      return $"Open: {is_open} ; Direction: {direction} ; Label: {label}";
    }
  }
}
