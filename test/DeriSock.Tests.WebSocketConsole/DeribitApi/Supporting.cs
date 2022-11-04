namespace DeriSock.Tests.WebSocketConsole.DeribitApi;

using Newtonsoft.Json;

internal sealed class Supporting
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
  public static object PublicTest(string expected_result = "", bool dummy = false)
    => new { version = "4.3.2.1" };
}
