namespace DeriSock.WebSocket;

using System;
using System.Net.WebSockets;

/// <summary>
/// Contains information about a WebSocket close event
/// </summary>
public sealed class WebSocketCloseEventArgs : EventArgs
{
  /// <summary>
  ///   Can contain information about the reason of a disconnect
  /// </summary>
  public WebSocketCloseStatus? CloseStatus { get; }

  /// <summary>
  ///   Can contain a description about the reason of a disconnect
  /// </summary>
  public string? CloseStatusDescription { get; }

  /// <summary>
  /// Can contain an error that occurred while processing messages
  /// </summary>
  public Exception? Error { get; }

  /// <inheritdoc />
  public WebSocketCloseEventArgs(WebSocketCloseStatus? closeStatus, string? closeStatusDescription, Exception? error)
  {
    CloseStatus = closeStatus;
    CloseStatusDescription = closeStatusDescription;
    Error = error;
  }
}
