namespace DeriSock;

using DeriSock.Model;

internal static class InternalStringExtensions
{
  internal static bool IsLastCharDigit(this string value)
  {
#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    return char.IsDigit(value[^1]);
#else
    return char.IsDigit(value[value.Length - 1]);
#endif
  }

  public static InstrumentType GetInstrumentType(this string instrumentName)
  {
    return instrumentName switch
    {
      { } i when i.EndsWith("-C") || i.EndsWith("-P") => InstrumentType.Option,
      { } i when i.EndsWith("-PERPETUAL") => InstrumentType.Perpetual,
      { Length: >= 1 } i when i.IsLastCharDigit() => InstrumentType.Future,
      _ => InstrumentType.Undefined
    };
  }

  public static OptionType GetOptionType(this string instrumentName)
  {
    return instrumentName switch
    {
      { } i when i.EndsWith("-C") => OptionType.Call,
      { } i when i.EndsWith("-P") => OptionType.Put,
      _ => OptionType.Undefined
    };
  }
}
