namespace DeriSock.JsonRpc
{
  using System;
  using System.Net.WebSockets;
  using System.Threading;
  using System.Threading.Tasks;

  public interface IWebSocket : IDisposable
  {
    WebSocketState State { get; }
    WebSocketCloseStatus? CloseStatus { get; }
    string CloseStatusDescription { get; }

    Task ConnectAsync(Uri uri, CancellationToken cancellationToken);

    Task SendAsync(
      ArraySegment<byte> buffer,
      WebSocketMessageType messageType, bool endOfMessage,
      CancellationToken cancellationToken);

    Task<WebSocketReceiveResult> ReceiveAsync(
      ArraySegment<byte> buffer,
      CancellationToken cancellationToken);

    Task CloseAsync(
      WebSocketCloseStatus closeStatus,
      string statusDescription,
      CancellationToken cancellationToken);
  }
}
