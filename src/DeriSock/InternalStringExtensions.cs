namespace DeriSock;

using System;
using System.Collections.Generic;
using System.Linq;

using DeriSock.Model;

internal static class InternalStringExtensions
{
  private static readonly Dictionary<string, ComboType> ComboTypeMap = new()
  {
    { "-FS-", ComboType.FutureSpread },
    { "-CS-", ComboType.CallSpread },
    { "-CSR12-", ComboType.CallRatioSpread1x2 },
    { "-CSR13-", ComboType.CallRatioSpread1x3 },
    { "-CSR23-", ComboType.CallRatioSpread2x3 },
    { "-PS-", ComboType.PutSpread },
    { "-PSR12-", ComboType.PutRatioSpread1x2 },
    { "-PSR13-", ComboType.PutRatioSpread1x3 },
    { "-PSR23-", ComboType.PutRatioSpread2x3 },
    { "-STRD-", ComboType.Straddle },
    { "-STRG-", ComboType.Strangle },
    { "-GUTS-", ComboType.StrangleItm },
    { "-RR-", ComboType.RiskReversal },
    { "-RRITM-", ComboType.RiskReversalItm },
    { "-CCAL-", ComboType.CallCalendarSpread },
    { "-PCAL-", ComboType.PutCalendarSpread },
    { "-CDIAG-", ComboType.CallDiagonalCalendar },
    { "-PDIAG-", ComboType.PutDiagonalCalendar },
    { "-STDC-", ComboType.StraddleCalendar },
    { "-DSTDC-", ComboType.StraddleCalendarDiagonal },
    { "-REV-", ComboType.ReversalConversion },
    { "-CBUT-", ComboType.CallButterfly },
    { "-PBUT-", ComboType.PutButterfly },
    { "-IBUT-", ComboType.IronButterfly },
    { "-CBUT111-", ComboType.SkinnyCallButterfly },
    { "-PBUT111-", ComboType.SkinnyPutButterfly },
    { "-CLAD-", ComboType.CallLadder },
    { "-PLAD-", ComboType.PutLadder },
    { "-CCOND-", ComboType.CallCondor },
    { "-PCOND-", ComboType.PutCondor },
    { "-ICOND-", ComboType.IronCondor },
    { "-BOX-", ComboType.Box },
    { "-JR-", ComboType.JellyRoll }
  };

  internal static bool IsLastCharDigit(this string? value)
  {
    if (value == null)
      return false;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    return char.IsDigit(value[^1]);
#else
    return char.IsDigit(value[value.Length - 1]);
#endif
  }

  /// <summary>
  ///   Gets the instrument type from instrument.
  /// </summary>
  /// <param name="instrumentName">The instrument name.</param>
  /// <returns>The <see cref="InstrumentType" /> for the instrument.</returns>
  public static InstrumentType GetInstrumentType(this string? instrumentName)
  {
    if (instrumentName == null)
      return InstrumentType.Undefined;

    var comboType = instrumentName.GetComboType();

    if (comboType != ComboType.Undefined)
      return comboType switch
      {
        ComboType.FutureSpread => InstrumentType.FutureCombo,
        _                      => InstrumentType.OptionCombo
      };

    return instrumentName switch
    {
      { } i when i.EndsWith("-C") || i.EndsWith("-P") => InstrumentType.Option,
      { } i when i.EndsWith("-PERPETUAL")             => InstrumentType.Perpetual,
      { Length: >= 1 } i when i.IsLastCharDigit()     => InstrumentType.Future,
      _                                               => InstrumentType.Undefined
    };
  }

  /// <summary>
  ///   Gets the combo type from instrument.
  /// </summary>
  /// <param name="instrumentName">The instrument name.</param>
  /// <returns>The <see cref="ComboType" /> for the instrument.</returns>
  public static ComboType GetComboType(this string? instrumentName)
  {
    if (instrumentName == null)
      return ComboType.Undefined;

    var firstSepIndex = instrumentName.IndexOf('-');

    if (firstSepIndex < 0)
      return ComboType.Undefined;

    // The default value for KeyValuePair<string, ComboType> will contain a ComboType.Undefined value.
    // So there is no need to check the return value of FirstOrDefault.
    return ComboTypeMap.FirstOrDefault(x => instrumentName.IndexOf(x.Key, StringComparison.OrdinalIgnoreCase) == firstSepIndex).Value;
  }

  /// <summary>
  ///   Gets the option type for an instrument.
  /// </summary>
  /// <param name="instrumentName">The instrument name.</param>
  /// <returns>The <see cref="OptionType" /> for the instrument.</returns>
  public static OptionType GetOptionType(this string? instrumentName)
  {
    if (instrumentName == null)
      return OptionType.Undefined;

    return instrumentName switch
    {
      { } i when i.EndsWith("-C") => OptionType.Call,
      { } i when i.EndsWith("-P") => OptionType.Put,
      _                           => OptionType.Undefined
    };
  }

  /// <summary>
  ///   Gets the underlying currency from instrument.
  /// </summary>
  /// <param name="instrumentName">The instrument name.</param>
  /// <returns>The underlying currency or an empty string.</returns>
  public static string GetUnderlyingCurrency(this string? instrumentName)
  {
    if (instrumentName is null)
      return string.Empty;

    var separatorIndex = instrumentName.IndexOf('-');

    if (separatorIndex < 0)
      return string.Empty;

    return instrumentName.Substring(0, separatorIndex);
  }

  /// <summary>
  ///   Gets the expiration date for an instrument.
  /// </summary>
  /// <param name="instrumentName">The instrument name.</param>
  /// <returns>The expiration date or <c>default(DateTime)</c></returns>
  public static DateTime ToInstrumentExpiration(this string? instrumentName)
  {
    var split = instrumentName?.Split('-');

    if (split == null || split.Length < 2)
      return default(DateTime);

    if (!DateTime.TryParse(split[1], out var dt))
      return default(DateTime);

    return new DateTime(dt.Year, dt.Month, dt.Day, 8, 0, 0, DateTimeKind.Utc).ToLocalTime();
  }

  /// <summary>
  ///   Gets the strike price for an option instrument.
  /// </summary>
  /// <param name="instrumentName">The instrument name.</param>
  /// <returns>The strike price or 0.</returns>
  public static decimal GetStrikePrice(this string? instrumentName)
  {
    var splitString = instrumentName?.Split('-');

    if (splitString is null || splitString.Length < 3)
      return 0m;

    if (!decimal.TryParse(splitString[2], out var strikePrice))
      return 0m;

    return strikePrice;
  }
}
