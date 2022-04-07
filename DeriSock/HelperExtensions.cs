namespace DeriSock;

using System;
using System.Collections.Generic;
using DeriSock.Constants;

/// <summary>
/// Provides some Extension Methods for various types
/// </summary>
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
    return (long)dateTime.Subtract(DateTimeConsts.UnixEpoch).TotalMilliseconds;
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
