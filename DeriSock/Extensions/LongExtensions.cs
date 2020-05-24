namespace DeriSock.Extensions
{
  using System;

  public static class LongExtensions
  {
    //Not an extension but something like that
    public static DateTime TimeStampToDateTime(long timestamp)
    {
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      return dateTime.AddSeconds(timestamp / 1000.0).ToLocalTime();
    }

    public static DateTime AsDateTime(this long timestamp)
    {
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      return dateTime.AddSeconds(timestamp / 1000.0).ToLocalTime();
    }


  }
}
