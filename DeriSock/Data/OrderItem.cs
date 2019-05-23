namespace DeriSock.Data
{
  using System.Diagnostics;

  [DebuggerDisplay("{" + nameof(ToDebuggerDisplay) + "(),nq}")]
  public class OrderItem
  {
    /// <summary>
    /// order_id if open ; can be null/empty
    /// </summary>
    public string order_id;
    /// <summary>
    /// user defined label (up to 32 characters)
    /// </summary>
    public string label;
    /// <summary>
    /// The price per currency (e.g. price for one BTC)
    /// </summary>
    public double price;
    /// <summary>
    /// Amount in USD
    /// </summary>
    public long amount;
    /// <summary>
    /// Amount in USD that's already fullfilled
    /// </summary>
    public long filled_amount;
    /// <summary>
    /// Direction of the order (buy/sell)
    /// </summary>
    public string direction;
    /// <summary>
    /// Is this in Open Orders?
    /// </summary>
    public bool is_open;
    /// <summary>
    /// Is used in another order (e.g. buy order is used in a sell order)
    /// </summary>
    public bool is_used;

    private string ToDebuggerDisplay()
    {
      return $"Open: {is_open} ; Direction: {direction} ; Label: {label}";
    }
  }
}
