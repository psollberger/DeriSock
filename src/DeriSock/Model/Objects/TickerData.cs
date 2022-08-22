using System;
using Newtonsoft.Json;

namespace DeriSock.Model;

public partial class TickerData
{
  /// <summary>
  ///   The option type (only for options)
  /// </summary>
  public DateTime ExpiryDate => InstrumentName.ToInstrumentExpiration();

  /// <summary>
  ///   The instrument type (options,future,perpetual)
  /// </summary>
  public InstrumentType InstrumentType => InstrumentName.GetInstrumentType();

  /// <summary>
  ///   The option type (only for options)
  /// </summary>
  [JsonIgnore]
  public OptionType OptionTypeEnum => InstrumentName.GetOptionType();

  /// <summary>
  ///   Days to expiry of contract
  /// </summary>
  [JsonIgnore]
  public double DaysToExpiry => ExpiryDate.ToTotalDaysFromNow();

  /// <summary>
  ///   Strike price (only for options)
  /// </summary>
  public decimal Strike => InstrumentName.GetStrikePrice();
}
