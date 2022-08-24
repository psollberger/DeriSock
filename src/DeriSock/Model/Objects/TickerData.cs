using System;
using Newtonsoft.Json;

namespace DeriSock.Model;

public partial class TickerData
{
  /// <summary>
  ///   The option type (only for options)
  /// </summary>
  [JsonIgnore]
  public DateTime ExpiryDate => InstrumentName.ToInstrumentExpiration();

  /// <summary>
  ///   The instrument type (options,future,perpetual)
  /// </summary>
  [JsonIgnore]
  public InstrumentType InstrumentType => InstrumentName.GetInstrumentType();

  /// <summary>
  ///   The option type (only for options)
  /// </summary>
  [JsonIgnore]
  public OptionType OptionType => InstrumentName.GetOptionType();
  
  /// <summary>
  ///   Get combo type
  /// </summary>
  [JsonIgnore]
  public ComboType ComboType => InstrumentName.GetComboType();

  /// <summary>
  ///   Days to expiry of contract
  /// </summary>
  [JsonIgnore]
  public double DaysToExpiry => ExpiryDate.ToTotalDaysFromNow();

  /// <summary>
  ///   Strike price (only for options)
  /// </summary>
  [JsonIgnore]
  public decimal Strike => InstrumentName.GetStrikePrice();
  
  /// <summary>
  ///   Underlying currency
  /// </summary>
  [JsonIgnore]
  public string UnderlyingCurrency => InstrumentName.GetUnderlyingCurrency();
}
