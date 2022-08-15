namespace DeriSock.Model;

using DeriSock.Converter;

using Newtonsoft.Json;

/// <summary>
///   Represents a combination of price and amount
/// </summary>
[JsonConverter(typeof(PriceAmountArrayConverter))]
public class PriceAmountItem
{
  /// <summary>
  ///   The price representation
  /// </summary>
  public decimal Price { get; set; }

  /// <summary>
  ///   The amount representation
  /// </summary>
  public decimal Amount { get; set; }
}
