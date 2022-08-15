namespace DeriSock.WebSocket;

using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

/// <summary>
///   The ITextMessageWebSocket handles the retrieval of text message blocks using a <see cref="WebSocket" />
/// </summary>
public interface ITextMessageWebSocket : IDisposable
{
  /// <summary>
  ///   The <see cref="MessageReceived"/> event is called whenever collecting a text message block finished
  /// </summary>
  event EventHandler<WebSocketTextMessageReceivedEventArgs>? MessageReceived;

  /// <summary>
  /// The <see cref="ConnectionClosed"/> event is called when the connection to the WebSocket is closed
  /// </summary>
  event EventHandler<WebSocketCloseEventArgs>? ConnectionClosed;

  /// <summary>
  ///   The current state of the WebSocket
  /// </summary>
  WebSocketState State { get; }

  /// <summary>
  ///   Connects to the WebSocket server
  /// </summary>
  /// <param name="uri">The target <see cref="Uri" /> to connect to</param>
  /// <param name="cancellationToken">Used to cancel the async call</param>
  Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

  /// <summary>
  ///   Closes the connection to the WebSocket server
  /// </summary>
  /// <param name="closeStatus">The kind in which the connection should be closed</param>
  /// <param name="closeStatusDescription">A closing description</param>
  /// <param name="cancellationToken">Used to cancel the async call</param>
  /// <returns></returns>
  Task CloseAsync(WebSocketCloseStatus? closeStatus, string? closeStatusDescription, CancellationToken cancellationToken);

  /// <summary>
  ///   Sends a text message to the WebSocket server
  /// </summary>
  /// <param name="message">The message to be sent</param>
  /// <param name="cancellationToken">Used to cancel the async call</param>
  Task SendMessageAsync(string message, CancellationToken cancellationToken);
}
