namespace DeriSock.Tests.WebSocketConsole.DeribitApi;

using System.Net.WebSockets;

using StreamJsonRpc;

internal sealed partial class DeribitApiMock
{
  private readonly WebSocket _clientSocket;
  private readonly JsonRpc _rpc;

  internal DeribitApiMock(WebSocket clientSocket, JsonRpc rpc)
  {
    _clientSocket = clientSocket;
    _rpc = rpc;
  }
}
