namespace DeriSock.WebSocket;

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Serilog;
using Serilog.Events;

/// <inheritdoc />
public sealed class TextMessageWebSocket : ITextMessageWebSocket
{
  private readonly ILogger? _logger;
  private ClientWebSocket _webSocket;
  private readonly ClientWebSocketTextCollector _textCollector;
  private Uri? _webSocketUri;

  /// <summary>
  ///   Creates an instance of the <see cref="TextMessageWebSocket" /> class
  /// </summary>
  public TextMessageWebSocket(ILogger? logger)
  {
    _logger = logger;
    _webSocket = new ClientWebSocket();
    _textCollector = new ClientWebSocketTextCollector(_webSocket, logger);

    _textCollector.MessageReady = (message) =>
    {
      MessageReceived?.Invoke(this, new WebSocketTextMessageReceivedEventArgs(message));
    };

    _textCollector.CloseInitiated = (_, _, _) =>
    {
      _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    };

    _textCollector.ErrorOccurred = (socketCloseStatus, closeStatusDescription, ex) =>
    {
      if (ConnectionClosed is null)
        return;

      var closeStatus = socketCloseStatus ?? WebSocketCloseStatus.Empty;
      var ea = new WebSocketCloseEventArgs(closeStatus, closeStatus == WebSocketCloseStatus.Empty ? null : closeStatusDescription, ex);
      Task.Run(() => ConnectionClosed.Invoke(this, ea), CancellationToken.None).ConfigureAwait(false);
    };
  }

  /// <inheritdoc />
  public event EventHandler<WebSocketTextMessageReceivedEventArgs>? MessageReceived;

  /// <inheritdoc />
  public event EventHandler<WebSocketCloseEventArgs>? ConnectionClosed;

  /// <inheritdoc />
  public WebSocketState State => _webSocket.State;

  /// <inheritdoc />
  public async Task ConnectAsync(Uri uri, CancellationToken cancellationToken)
  {
    if (_webSocket.State is not WebSocketState.None) {
      _webSocket.Dispose();
      _webSocket = new ClientWebSocket();
      _textCollector.WebSocket = _webSocket;
    }

    if (_webSocket.State is not (WebSocketState.Closed or WebSocketState.None))
      return;

    _webSocketUri = uri;

    if (_logger?.IsEnabled(LogEventLevel.Information) ?? false)
      _logger.Information("Connecting to {Host}", _webSocketUri);

    try {
      await _webSocket.ConnectAsync(_webSocketUri, cancellationToken).ConfigureAwait(false);
    }
    catch (Exception ex) {
      if (_logger?.IsEnabled(LogEventLevel.Error) ?? false)
        _logger.Error(ex, "Exception during {methodName}", nameof(ConnectAsync));

      throw;
    }

    if (_logger?.IsEnabled(LogEventLevel.Information) ?? false)
      _logger?.Information("Connected. Start collecting messages");

    _textCollector.StartCollect();
  }

  /// <inheritdoc />
  public async Task CloseAsync(WebSocketCloseStatus? closeStatus, string? closeStatusDescription, CancellationToken cancellationToken)
  {
    if (_webSocket.State is not (WebSocketState.Open or WebSocketState.Connecting or WebSocketState.CloseReceived))
      return;

    if (_logger?.IsEnabled(LogEventLevel.Information) ?? false)
      _logger?.Information("Closing connection with {Host}", _webSocketUri);

    _textCollector.StopCollect();

    var usedCloseStatus = closeStatus ?? WebSocketCloseStatus.NormalClosure;
    var usedCloseStatusDescription = closeStatusDescription ?? string.Empty;

    if (_webSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
      await _webSocket.CloseOutputAsync(usedCloseStatus, usedCloseStatusDescription, cancellationToken).ConfigureAwait(false);

    if (ConnectionClosed is not null)
      await Task.Run(() => ConnectionClosed.Invoke(this, new WebSocketCloseEventArgs(usedCloseStatus, usedCloseStatusDescription, null)), cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task SendMessageAsync(string message, CancellationToken cancellationToken)
  {
    await _webSocket.SendAsync(
      new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text,
      true, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public void Dispose()
  {
    _textCollector.Dispose();
    _webSocket.Dispose();
  }
}
