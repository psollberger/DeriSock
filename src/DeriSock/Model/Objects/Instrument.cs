using System;
using Newtonsoft.Json;

namespace DeriSock.Model;

using Newtonsoft.Json;

public partial class Instrument
{
  /// <summary>
  ///   The instrument type (options,future,perpetual)
  /// </summary>
  [JsonIgnore]
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
  public double DaysToExpiry => ExpirationTimestamp.ToTotalDaysFromNow();
}
