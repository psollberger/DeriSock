namespace DeriSock.JsonRpc
{
  public class WebSocketFactory
  {
    private static IWebSocketFactory _factory;

    public static void Register(IWebSocketFactory factory)
    {
      _factory = factory;
    }

    public static IWebSocket Create()
    {
      return _factory != null ? _factory.Create() : new DefaultWebSocket();
    }
  }
}
