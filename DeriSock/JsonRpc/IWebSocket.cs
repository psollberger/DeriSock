// ReSharper disable InheritdocConsiderUsage
namespace DeriSock.JsonRpc;

using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
/// Used to provide a custom WebSocket implementation using <see cref="IWebSocketFactory"/>
/// </summary>
public interface IWebSocket : IDisposable
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
  /// Connects to the WebSocket server
  /// </summary>
  /// <param name="uri">The target <see cref="Uri"/> to connect to</param>
  /// <param name="cancellationToken">Used to cancel the async call</param>
  Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

  /// <summary>
  /// Sends data to the WebSocket server
  /// </summary>
  /// <param name="buffer">The bytes to be sent</param>
  /// <param name="messageType">The <see cref="WebSocketMessageType"/> of the message</param>
  /// <param name="endOfMessage">Indicates if this is the end of the message</param>
  /// <param name="cancellationToken">Used to cancel the async call</param>
  Task SendAsync(
    ArraySegment<byte> buffer,
    WebSocketMessageType messageType, bool endOfMessage,
    CancellationToken cancellationToken);

  /// <summary>
  /// Used to fetch data from the WebSocket server
  /// </summary>
  /// <param name="buffer">The buffer into which the received data should be written</param>
  /// <param name="cancellationToken">Used to cancel the async call</param>
  /// <returns>A <see cref="WebSocketReceiveResult"/> object describing the result of the operation</returns>
  Task<WebSocketReceiveResult> ReceiveAsync(
    ArraySegment<byte> buffer,
    CancellationToken cancellationToken);

  /// <summary>
  /// Closes the connection to the WebSocket server
  /// </summary>
  /// <param name="closeStatus">The kind in which the connection should be closed</param>
  /// <param name="statusDescription">A closing description</param>
  /// <param name="cancellationToken">Used to cancel the async call</param>
  /// <returns></returns>
  Task CloseAsync(
    WebSocketCloseStatus closeStatus,
    string statusDescription,
    CancellationToken cancellationToken);
}
