namespace DeriSock;

using System;
using System.Net.WebSockets;

/// <summary>
///   Provides data for the Disconnected event.
/// </summary>
public class DeribitClientDisconnectedEventArgs : EventArgs
{
  /// <summary>
  ///   The WebSocket close status.
  /// </summary>
  public WebSocketCloseStatus? CloseStatus { get; }

  /// <summary>
  ///   The close status description.
  /// </summary>
  public string? CloseStatusDescription { get; }

  /// <summary>
  ///   In case of an error, the exception that occured.
  /// </summary>
  public Exception? Exception { get; }

  /// <summary>
  ///   Initializes a new instance of the <see cref="DeribitClientDisconnectedEventArgs" /> class.
  /// </summary>
  /// <param name="closeStatus">The WebSocket close status.</param>
  /// <param name="closeStatusDescription">The close status description.</param>
  /// <param name="exception">In case of an error. the exception that occured.</param>
  public DeribitClientDisconnectedEventArgs(WebSocketCloseStatus? closeStatus, string? closeStatusDescription, Exception? exception)
  {
    CloseStatus = closeStatus;

    // When using close status code 'Empty' the description must be null
    CloseStatusDescription = closeStatus == WebSocketCloseStatus.Empty ? null : closeStatusDescription;
    Exception = exception;
  }
}
