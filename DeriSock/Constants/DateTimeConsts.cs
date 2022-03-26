namespace DeriSock.Constants;

using System;

internal static class DateTimeConsts
{
#if NETFRAMEWORK
  public static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#else
  public static readonly DateTime UnixEpoch = DateTime.UnixEpoch;
#endif
}
