namespace DeriSock.WebSocket;

using System;

/// <inheritdoc />
/// <summary>
/// Contains information about a received message
/// </summary>
public sealed class WebSocketTextMessageReceivedEventArgs : EventArgs
{
  /// <summary>
  /// The message that was received
  /// </summary>
  public string Message { get; }

  /// <inheritdoc />
  public WebSocketTextMessageReceivedEventArgs(string message)
  {
    Message = message;
  }
}
