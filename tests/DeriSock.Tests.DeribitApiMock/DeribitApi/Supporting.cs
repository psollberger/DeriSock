namespace DeriSock.Tests.WebSocketConsole.DeribitApi;

using System.Net.WebSockets;

internal sealed partial class DeribitApiMock
{
  [MyJsonRpcMethod("public/get_time")]
  public static long PublicGetTime()
    => DateTime.UtcNow.ToUnixTimeMilliseconds();

  [MyJsonRpcMethod("public/hello")]
  public static object PublicHello(string client_name, string client_version)
    => new { version = "4.3.2.1" };

  [MyJsonRpcMethod("public/status")]
  public static object PublicStatus()
    => new { locked = "false", locked_currencies = Array.Empty<string>() };

  [MyJsonRpcMethod("public/test")]
  public async Task<object> PublicTestAsync(string expected_result = "", bool dummy = false)
  {
    switch (expected_result)
    {
      case "disconnect":
        _rpc.Dispose();
        break;

      case "close":
        await _clientSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, null, CancellationToken.None);
        break;
    }

    return new { version = "4.3.2.1" };
  }
}
