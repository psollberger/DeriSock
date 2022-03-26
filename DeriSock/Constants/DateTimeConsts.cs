namespace DeriSock.Constants;

using System;

internal static class DateTimeConsts
{
#if NETFRAMEWORK
  private static readonly DateTime NetFxUnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
#endif

  public static DateTime UnixEpoch
  {
    get
    {
#if NETFRAMEWORK
      return NetFxUnixEpoch;
#else
      return DateTime.UnixEpoch;
#endif
    }
  }
}
