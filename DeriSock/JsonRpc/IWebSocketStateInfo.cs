namespace DeriSock.JsonRpc;

using System;
using System.Net.WebSockets;

/// <summary>
/// Provides information about a WebSockets state
/// </summary>
public interface IWebSocketStateInfo
{
  /// <summary>
  ///   The current state of the WebSocket
  /// </summary>
  WebSocketState State { get; }

  /// <summary>
  ///   Can contain information about the reason of a disconnect
  /// </summary>
  WebSocketCloseStatus? CloseStatus { get; }

  /// <summary>
  ///   Can contain a description about the reason of a disconnect
  /// </summary>
  string CloseStatusDescription { get; }

  /// <summary>
  ///   Can contain a Exception that occurred when the WebSocket disconnected
  /// </summary>
  Exception Error { get; }
}
