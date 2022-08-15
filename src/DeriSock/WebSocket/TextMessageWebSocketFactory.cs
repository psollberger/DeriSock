namespace DeriSock.WebSocket;

using Serilog;

/// <summary>
/// Uses the default or a custom implementation of an <see cref="ITextMessageWebSocketFactory"/> implementation to create new <see cref="ITextMessageWebSocket"/> instances
/// </summary>
public class TextMessageWebSocketFactory
{
  private static ITextMessageWebSocketFactory? _factory;

  /// <summary>
  /// Registers a custom <see cref="ITextMessageWebSocketFactory"/> that will be used to create <see cref="ITextMessageWebSocket"/> instances
  /// </summary>
  /// <param name="factory">The custom factory</param>
  public static void Register(ITextMessageWebSocketFactory factory)
  {
    _factory = factory;
  }

  /// <summary>
  /// Creates a new instance of an <see cref="ITextMessageWebSocket"/> implementation
  /// </summary>
  /// <returns>The instance of the <see cref="ITextMessageWebSocket"/> implementation</returns>
  public static ITextMessageWebSocket Create(ILogger? logger)
  {
    return _factory?.Create() ?? new TextMessageWebSocket(logger);
  }
}
