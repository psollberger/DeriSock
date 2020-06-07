namespace DeriSock.JsonRpc
{
  using System;
  using System.Net.WebSockets;
  using System.Threading;
  using System.Threading.Tasks;

  public class DefaultWebSocket : IWebSocket
  {
    private readonly ClientWebSocket _socket;

    public DefaultWebSocket()
    {
      _socket = new ClientWebSocket();
    }

    public WebSocketState State => _socket.State;
    public WebSocketCloseStatus? CloseStatus => _socket.CloseStatus;
    public string CloseStatusDescription => _socket.CloseStatusDescription;

    public void Dispose()
    {
      _socket.Dispose();
    }

    public Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
    {
      return _socket.ConnectAsync(uri, cancellationToken);
    }

    public Task SendAsync(ArraySegment<byte> buffer, WebSocketMessageType messageType, bool endOfMessage,
      CancellationToken cancellationToken)
    {
      return _socket.SendAsync(buffer, messageType, endOfMessage, cancellationToken);
    }

    public Task<WebSocketReceiveResult> ReceiveAsync(ArraySegment<byte> buffer, CancellationToken cancellationToken)
    {
      return _socket.ReceiveAsync(buffer, cancellationToken);
    }

    public Task CloseAsync(WebSocketCloseStatus closeStatus, string statusDescription, CancellationToken cancellationToken)
    {
      return _socket.CloseAsync(closeStatus, statusDescription, cancellationToken);
    }
  }
}
