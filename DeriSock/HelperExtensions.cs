namespace DeriSock
{
  using System;

  public static class HelperExtensions
  {
    private static readonly long _ticksPerMillisecond = TimeSpan.TicksPerMillisecond;
    private static readonly long _ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

    /// <summary>
    ///   Converts Deribit Timestamps from microseconds to an <see cref="DateTime" />
    /// </summary>
    /// <param name="timestamp">timestamp in microseconds since the Unix epoch</param>
    /// <returns>The <see cref="DateTime" /> that represents the timestamp in local time</returns>
    public static DateTime AsDateTimeFromMicroseconds(this long timestamp)
    {
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      return dateTime.AddTicks(_ticksPerMicrosecond * timestamp).ToLocalTime();
    }

    /// <summary>
    ///   Converts Deribit Timestamps from milliseconds to an <see cref="DateTime" />
    /// </summary>
    /// <param name="timestamp">timestamp in milliseconds since the Unix epoch</param>
    /// <returns>The <see cref="DateTime" /> that represents the timestamp in local time</returns>
    public static DateTime AsDateTimeFromMilliseconds(this long timestamp)
    {
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      return dateTime.AddTicks(_ticksPerMillisecond * timestamp).ToLocalTime();
    }

    /// <summary>
    ///   Converts a <see cref="DateTime" /> into milliseconds since Unix epoch
    /// </summary>
    /// <param name="dateTime"><see cref="DateTime" /> as UTC</param>
    /// <returns></returns>
    public static long AsMilliseconds(this DateTime dateTime)
    {
      dateTime = dateTime.ToUniversalTime();
      return (long)dateTime.Subtract(DateTime.UnixEpoch).TotalMilliseconds;
    }
  }
}
