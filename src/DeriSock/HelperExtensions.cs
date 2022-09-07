namespace DeriSock;

using System;
using System.Collections.Generic;

using DeriSock.Constants;

/// <summary>
///   Provides some Extension Methods for various types.
/// </summary>
public static class HelperExtensions
{
  /// <summary>
  ///   Converts microseconds since Unix epoch to a <see cref="DateTime" />.
  /// </summary>
  /// <param name="timestamp">timestamp in microseconds since the Unix epoch.</param>
  /// <returns>The <see cref="DateTime" /> that represents the timestamp in local time.</returns>
  public static DateTime ToDateTimeFromUnixTimeMicroseconds(this long timestamp)
  {
    var unixepochticks = DateTimeConstants.UnixEpoch.Ticks;
    return DateTimeConstants.UnixEpoch.AddTicks(DateTimeConstants.TicksPerMicrosecond * timestamp).ToLocalTime();
  }

  /// <summary>
  ///   Converts a <see cref="DateTime" /> into microseconds since Unix epoch.
  /// </summary>
  /// <param name="dateTime">The <see cref="DateTime" /> object to convert to microseconds.</param>
  /// <returns>The amount of microsends since Unix epoch.</returns>
  public static long ToUnixTimeMicroseconds(this DateTime dateTime)
  {
    var dateTimeOffset = new DateTimeOffset(dateTime);
    var microseconds = dateTimeOffset.UtcTicks / DateTimeConstants.TicksPerMicrosecond;
    return microseconds - DateTimeConstants.UnixEpochMicroseconds;
  }

  /// <summary>
  ///   Converts milliseconds since Unix epoch to an <see cref="DateTime" />.
  /// </summary>
  /// <param name="timestamp">timestamp in milliseconds since the Unix epoch.</param>
  /// <returns>The <see cref="DateTime" /> object to convert to milliseconds.</returns>
  public static DateTime ToDateTimeFromUnixTimeMilliseconds(this long timestamp)
    => new DateTime(DateTimeOffset.FromUnixTimeMilliseconds(timestamp).Ticks, DateTimeKind.Utc).ToLocalTime();

  /// <summary>
  ///   Converts a <see cref="DateTime" /> into milliseconds since Unix epoch.
  /// </summary>
  /// <param name="dateTime">The <see cref="DateTime" /> object to convert to milliseconds.</param>
  /// <returns>The amount of milliseconds since Unix epoch.</returns>
  public static long ToUnixTimeMilliseconds(this DateTime dateTime)
    => new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();

  /// <summary>
  ///   Calculates the amount of days from now until this point in time is reached.
  /// </summary>
  /// <param name="value">A <see cref="DateTime" /> object for which the remaining days should be calculated.</param>
  /// <returns>Total days as a floating point value.</returns>
  public static double ToTotalDaysFromNow(this DateTime value)
  {
    var now = DateTimeOffset.UtcNow;
    var target = new DateTimeOffset(value);

    if (now > target)
      return 0d;

    return (target - now).TotalDays;
  }
}
