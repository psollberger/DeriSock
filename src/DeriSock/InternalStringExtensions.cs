namespace DeriSock;

using System;
using DeriSock.Model;

internal static class InternalStringExtensions
{
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

  public static InstrumentType GetInstrumentType(this string? instrumentName)
  {
    if (instrumentName == null)
      return InstrumentType.Undefined;

    return instrumentName switch
    {
      { } i when i.EndsWith("-C") || i.EndsWith("-P") => InstrumentType.Option,
      { } i when i.EndsWith("-PERPETUAL") => InstrumentType.Perpetual,
      { } i when i.Split('-')[1].Equals("FS", StringComparison.CurrentCultureIgnoreCase) => InstrumentType.FutureCombo,
      { } i when i.Split('-')[1].Equals("CS", StringComparison.CurrentCultureIgnoreCase) => InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("CSR12", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("CSR13", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("CSR23", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("PS", StringComparison.CurrentCultureIgnoreCase) => InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("PSR12", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("PSR13", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("PSR23", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("STRD", StringComparison.CurrentCultureIgnoreCase) =>
        InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("STRG", StringComparison.CurrentCultureIgnoreCase) =>
        InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("GUTS", StringComparison.CurrentCultureIgnoreCase) =>
        InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("RR", StringComparison.CurrentCultureIgnoreCase) => InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("RRITM", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("CCAL", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("PCAL", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("CDIAG", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("PDIAG", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("STDC", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("DSTDC", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("REV", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("CBUT", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("PBUT", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("IBUT", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("CBUT111", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("PBUT111", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("CLAD", StringComparison.CurrentCultureIgnoreCase) =>
        InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("PLAD", StringComparison.CurrentCultureIgnoreCase) =>
        InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("CCOND", StringComparison.CurrentCultureIgnoreCase) =>
        InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("PCOND", StringComparison.CurrentCultureIgnoreCase) => InstrumentType
        .OptionCombo,
      { } i when i.Split('-')[1].Equals("ICOND", StringComparison.CurrentCultureIgnoreCase) =>
        InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("BOX", StringComparison.CurrentCultureIgnoreCase) => InstrumentType.OptionCombo,
      { } i when i.Split('-')[1].Equals("JR", StringComparison.CurrentCultureIgnoreCase) => InstrumentType.OptionCombo,
      {Length: >= 1} i when i.IsLastCharDigit() => InstrumentType.Future,
      _ => InstrumentType.Undefined
    };
  }

  public static ComboType GetComboType(this string? instrumentName)
  {
    if (instrumentName == null)
      return ComboType.Undefined;

    return instrumentName switch
    {
      { } i when i.Split('-')[1].Equals("FS", StringComparison.CurrentCultureIgnoreCase) => ComboType.FutureSpread,
      { } i when i.Split('-')[1].Equals("CS", StringComparison.CurrentCultureIgnoreCase) => ComboType.CallSpread,
      { } i when i.Split('-')[1].Equals("CSR12", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .CallRatioSpread1x2,
      { } i when i.Split('-')[1].Equals("CSR13", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .CallRatioSpread1x3,
      { } i when i.Split('-')[1].Equals("CSR23", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .CallRatioSpread2x3,
      { } i when i.Split('-')[1].Equals("PS", StringComparison.CurrentCultureIgnoreCase) => ComboType.PutSpread,
      { } i when i.Split('-')[1].Equals("PSR12", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .PutRatioSpread1x2,
      { } i when i.Split('-')[1].Equals("PSR13", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .PutRatioSpread1x3,
      { } i when i.Split('-')[1].Equals("PSR23", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .PutRatioSpread2x3,
      { } i when i.Split('-')[1].Equals("STRD", StringComparison.CurrentCultureIgnoreCase) => ComboType.Straddle,
      { } i when i.Split('-')[1].Equals("STRG", StringComparison.CurrentCultureIgnoreCase) => ComboType.Strangle,
      { } i when i.Split('-')[1].Equals("GUTS", StringComparison.CurrentCultureIgnoreCase) => ComboType.StrangleItm,
      { } i when i.Split('-')[1].Equals("RR", StringComparison.CurrentCultureIgnoreCase) => ComboType.RiskReversal,
      { } i when i.Split('-')[1].Equals("RRITM", StringComparison.CurrentCultureIgnoreCase) =>
        ComboType.RiskReversalItm,
      { } i when i.Split('-')[1].Equals("CCAL", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .CallCalendarSpread,
      { } i when i.Split('-')[1].Equals("PCAL", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .PutCalendarSpread,
      { } i when i.Split('-')[1].Equals("CDIAG", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .CallDiagonalCalendar,
      { } i when i.Split('-')[1].Equals("PDIAG", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .PutDiagonalCalendar,
      { } i when i.Split('-')[1].Equals("STDC", StringComparison.CurrentCultureIgnoreCase) =>
        ComboType.StraddleCalendar,
      { } i when i.Split('-')[1].Equals("DSTDC", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .StraddleCalendarDiagonal,
      { } i when i.Split('-')[1].Equals("REV", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .ReversalConversion,
      { } i when i.Split('-')[1].Equals("CBUT", StringComparison.CurrentCultureIgnoreCase) => ComboType.CallButterfly,
      { } i when i.Split('-')[1].Equals("PBUT", StringComparison.CurrentCultureIgnoreCase) => ComboType.PutButterfly,
      { } i when i.Split('-')[1].Equals("IBUT", StringComparison.CurrentCultureIgnoreCase) => ComboType.IronButterfly,
      { } i when i.Split('-')[1].Equals("CBUT111", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .SkinnyCallButterfly,
      { } i when i.Split('-')[1].Equals("PBUT111", StringComparison.CurrentCultureIgnoreCase) => ComboType
        .SkinnyPutButterfly,
      { } i when i.Split('-')[1].Equals("CLAD", StringComparison.CurrentCultureIgnoreCase) => ComboType.CallLadder,
      { } i when i.Split('-')[1].Equals("PLAD", StringComparison.CurrentCultureIgnoreCase) => ComboType.PutLadder,
      { } i when i.Split('-')[1].Equals("CCOND", StringComparison.CurrentCultureIgnoreCase) => ComboType.CallCondor,
      { } i when i.Split('-')[1].Equals("PCOND", StringComparison.CurrentCultureIgnoreCase) => ComboType.PutCondor,
      { } i when i.Split('-')[1].Equals("ICOND", StringComparison.CurrentCultureIgnoreCase) => ComboType.IronCondor,
      { } i when i.Split('-')[1].Equals("BOX", StringComparison.CurrentCultureIgnoreCase) => ComboType.Box,
      { } i when i.Split('-')[1].Equals("JR", StringComparison.CurrentCultureIgnoreCase) => ComboType.JellyRoll,
      _ => ComboType.Undefined
    };
  }

  public static OptionType GetOptionType(this string? instrumentName)
  {
    if (instrumentName == null)
      return OptionType.Undefined;

    return instrumentName switch
    {
      { } i when i.EndsWith("-C") => OptionType.Call,
      { } i when i.EndsWith("-P") => OptionType.Put,
      _ => OptionType.Undefined
    };
  }

  public static string GetUnderlyingCurrency(this string? instrumentName)
  {
    return instrumentName == null ? "" : instrumentName.Split('-')[0];
  }

  /// <summary>
  ///   Get expiry from name
  /// </summary>
  /// <param name="instrumentName"></param>
  /// <returns></returns>
  public static DateTime ToInstrumentExpiration(this string? instrumentName)
  {
    var split = instrumentName?.Split('-');

    if (split == null || split.Length < 2)
      return default(DateTime);

    if (!DateTime.TryParse(split[1], out var dt))
      return default(DateTime);

    return new DateTime(dt.Year, dt.Month, dt.Day, 8, 0, 0, DateTimeKind.Utc).ToLocalTime();
  }

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
