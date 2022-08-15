namespace DeriSock.Model;

using DeriSock.Converter;

using Newtonsoft.Json;

/// <summary>
///   Represents a combination of action, price and amount
/// </summary>
[JsonConverter(typeof(ActionPriceAmountArrayConverter))]
public class ActionPriceAmountItem
{
  /// <summary>
  ///   The action representation
  /// </summary>
  public string Action { get; set; } = string.Empty;

  /// <summary>
  ///   The price representation
  /// </summary>
  public decimal Price { get; set; }

  /// <summary>
  ///   The amount representation
  /// </summary>
  public decimal Amount { get; set; }
}
