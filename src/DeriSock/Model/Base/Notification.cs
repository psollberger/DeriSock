namespace DeriSock.Model;

using System;
using System.Diagnostics.CodeAnalysis;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Represents a received notification for a subscribed channel.
/// </summary>
/// <typeparam name="T">The data type of the notification data.</typeparam>
[SuppressMessage("ReSharper", "UnusedTypeParameter")]
public interface INotification<out T> where T : class
{
  /// <summary>
  ///   The time when the notification was received.
  /// </summary>
  [JsonIgnore]
  DateTime Timestamp { get; set; }

  /// <summary>
  ///   The same channel as given when subscribing to notifications.
  /// </summary>
  string Channel { get; set; }

  /// <summary>
  ///   The same label as given when subscribing to notifications (only for private channels).
  /// </summary>
  string? Label { get; set; }

  /// <summary>
  ///   Data specific for the channel as a <see cref="JToken" />.
  /// </summary>
  JToken DataToken { get; set; }

  /// <summary>
  ///   Converts the notification to a notification with a different data type.
  /// </summary>
  /// <typeparam name="TNew">The new data type.</typeparam>
  /// <returns>A <see cref="Notification{T}" /> instance with the new data type applied.</returns>
  Notification<TNew> ConvertTo<TNew>() where TNew : class;
}

/// <inheritdoc cref="INotification{T}" />
public class Notification<T> : INotification<T> where T : class
{
  /// <inheritdoc cref="INotification{T}.Timestamp" />
  [JsonIgnore]
  public DateTime Timestamp { get; set; }

  /// <inheritdoc cref="INotification{T}.Channel" />
  [JsonProperty("channel")]
  public string Channel { get; set; } = null!;

  /// <inheritdoc cref="INotification{T}.Label" />
  [JsonProperty("label")]
  public string? Label { get; set; }

  /// <inheritdoc cref="INotification{T}.DataToken" />
  [JsonProperty("data")]
  public JToken DataToken { get; set; } = null!;

  /// <summary>
  ///   The typed data that is contained in this notification.
  /// </summary>
  [JsonIgnore]
  public T Data { get; set; } = null!;

  /// <inheritdoc cref="INotification{T}.ConvertTo{TNew}" />
  public Notification<TNew> ConvertTo<TNew>() where TNew : class
    => new()
    {
      Timestamp = Timestamp,
      Channel = Channel,
      DataToken = DataToken,
      Data = DataToken.ToObject<TNew>()!
    };
}
