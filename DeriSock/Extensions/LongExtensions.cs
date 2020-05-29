namespace DeriSock.Extensions
{
  using System;

  public static class LongExtensions
  {
    private static readonly long _ticksPerMicrosecond = TimeSpan.TicksPerMillisecond / 1000;

    /// <summary>
    /// Converts Deribit Timestamps from microseconds to an <see cref="DateTime"/>
    /// </summary>
    /// <param name="timestamp">timestamp in microseconds since the Unix epoch</param>
    /// <returns>The <see cref="DateTime"/> that represents the timestamp in local time</returns>
    public static DateTime AsDateTime(this long timestamp)
    {
      var dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
      return dateTime.AddTicks(_ticksPerMicrosecond * timestamp).ToLocalTime();
    }
  }
}
