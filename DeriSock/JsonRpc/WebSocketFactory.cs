namespace DeriSock.JsonRpc;

/// <summary>
///   Used to inject a custom <see cref="IWebSocketFactory" />
/// </summary>
public class WebSocketFactory
{
  private static IWebSocketFactory _factory;

  /// <summary>
  ///   Register a custom <see cref="IWebSocketFactory" />
  /// </summary>
  /// <param name="factory"></param>
  public static void Register(IWebSocketFactory factory)
  {
    _factory = factory;
  }

  /// <summary>
  ///   Creates the default <see cref="IWebSocket" /> implementation or uses the a injected <see cref="IWebSocketFactory" /> to create one
  /// </summary>
  /// <returns>An <see cref="IWebSocket" /> implementation</returns>
  public static IWebSocket Create()
  {
    return _factory != null ? _factory.Create() : new DefaultWebSocket();
  }
}
