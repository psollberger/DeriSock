namespace DeriSock.WebSocket;

/// <summary>
/// The ITextMessageWebSocketFactory creates a new instance of an <see cref="ITextMessageWebSocket"/> implementation
/// </summary>
public interface ITextMessageWebSocketFactory
{
  /// <summary>
  /// Creates a new instance of an ITextMessageWebSocket implementation
  /// </summary>
  /// <returns></returns>
  ITextMessageWebSocket Create();
}
