namespace DeriSock.Constants;

using System;

internal static class DateTimeConstants
{
  public const long UnixEpochTicks = 621_355_968_000_000_000;
  public const long TicksPerMillisecond = TimeSpan.TicksPerMillisecond;
  public const long TicksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

  public const long UnixEpochMicroseconds = UnixEpochTicks / TicksPerMicrosecond;

#if NET5_0_OR_GREATER || NETSTANDARD2_1_OR_GREATER
  public static readonly DateTime UnixEpoch = DateTime.UnixEpoch;
#else
  public static readonly DateTime UnixEpoch = new(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
#endif
}
