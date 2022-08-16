namespace DeriSock;

using System;
using System.Collections.Generic;
using System.Globalization;

using DeriSock.Constants;

/// <summary>
/// Provides some Extension Methods for various types
/// </summary>
public static class HelperExtensions
{
  /// <summary>
  ///   Converts Deribit Timestamps from microseconds to an <see cref="DateTime" />
  /// </summary>
  /// <param name="timestamp">timestamp in microseconds since the Unix epoch</param>
  /// <returns>The <see cref="DateTime" /> that represents the timestamp in local time</returns>
  public static DateTime ToDateTimeFromUnixTimeMicroseconds(this long timestamp)
  {
    var unixepochticks = DateTimeConstants.UnixEpoch.Ticks;
    return DateTimeConstants.UnixEpoch.AddTicks(DateTimeConstants.TicksPerMicrosecond * timestamp).ToLocalTime();
  }

  /// <summary>
  ///   Converts a <see cref="DateTime" /> into microseconds since Unix epoch
  /// </summary>
  /// <param name="dateTime"><see cref="DateTime" /> as UTC</param>
  /// <returns></returns>
  public static long ToUnixTimeMicroseconds(this DateTime dateTime)
  {
    var dateTimeOffset = new DateTimeOffset(dateTime);
    var microseconds = dateTimeOffset.UtcTicks / DateTimeConstants.TicksPerMicrosecond;
    return microseconds - DateTimeConstants.UnixEpochMicroseconds;
  }

  /// <summary>
  ///   Converts Deribit Timestamps from milliseconds to an <see cref="DateTime" />
  /// </summary>
  /// <param name="timestamp">timestamp in milliseconds since the Unix epoch</param>
  /// <returns>The <see cref="DateTime" /> that represents the timestamp in local time</returns>
  public static DateTime ToDateTimeFromUnixTimeMilliseconds(this long timestamp)
  {
    return new DateTime(DateTimeOffset.FromUnixTimeMilliseconds(timestamp).Ticks, DateTimeKind.Utc).ToLocalTime();
  }

  /// <summary>
  ///   Converts a <see cref="DateTime" /> into milliseconds since Unix epoch
  /// </summary>
  /// <param name="dateTime"><see cref="DateTime" /> as UTC</param>
  /// <returns></returns>
  public static long ToUnixTimeMilliseconds(this DateTime dateTime)
  {
    return new DateTimeOffset(dateTime).ToUnixTimeMilliseconds();
  }
  
  /// <summary>
  /// Returns days to expiry of contract
  /// </summary>
  /// <param name="contractExpirationTime"></param>
  /// <param name="time"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentOutOfRangeException"></exception>
  public static double GetDaysToExpiry(DateTimeOffset contractExpirationTime, DateTimeOffset time)
  {
    if (time > contractExpirationTime)
      return 0;
    //throw new ArgumentOutOfRangeException(nameof(time));

    return (contractExpirationTime - time).TotalDays;
  }

  /// <summary>
  /// Tries to add a key/value pair
  /// </summary>
  /// <typeparam name="T">The type of the value</typeparam>
  /// <param name="dictionary">The dictionary in which the key/value pair should be added</param>
  /// <param name="key">The key as a string</param>
  /// <param name="value">The value as a type of T</param>
  /// <returns>true if the key was added. false if the key already existed</returns>
  /// <exception cref="ArgumentNullException"></exception>
  public static bool TryAdd<T>(this IDictionary<string, T> dictionary, string key, T value)
  {
    if (dictionary is null)
    {
      throw new ArgumentNullException(nameof(dictionary));
    }

    if (dictionary.ContainsKey(key))
    {
      return false;
    }

    dictionary.Add(key, value);
    return true;
  }
}
