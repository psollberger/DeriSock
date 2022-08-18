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
      { } i when i.EndsWith("-PERPETUAL")             => InstrumentType.Perpetual,
      { Length: >= 1 } i when i.IsLastCharDigit()     => InstrumentType.Future,
      _                                               => InstrumentType.Undefined
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
      _                           => OptionType.Undefined
    };
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
