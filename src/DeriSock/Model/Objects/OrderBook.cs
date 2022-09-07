namespace DeriSock.Model;

using System;

using Newtonsoft.Json;

public partial class OrderBook
{
  /// <summary>
  ///   The type of the instrument.
  /// </summary>
  [JsonIgnore]
  public InstrumentType InstrumentType => InstrumentName.GetInstrumentType();

  /// <summary>
  ///   The option type of the instrument.
  /// </summary>
  [JsonIgnore]
  public OptionType OptionType => InstrumentName.GetOptionType();

  /// <summary>
  ///   Expiry date of option contract.
  /// </summary>
  [JsonIgnore]
  public DateTime ExpiryDate => InstrumentName.ToInstrumentExpiration();

  /// <summary>
  ///   Days until expiry of option contract.
  /// </summary>
  [JsonIgnore]
  public double DaysToExpiry => ExpiryDate.ToTotalDaysFromNow();

  /// <summary>
  ///   Strike price of option contract.
  /// </summary>
  [JsonIgnore]
  public decimal Strike => GetStrikePrice();

  private decimal GetStrikePrice()
    => InstrumentType == InstrumentType.Option ? InstrumentName.GetStrikePrice() : 0;
}
