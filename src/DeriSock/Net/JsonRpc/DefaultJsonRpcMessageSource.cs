namespace DeriSock.Net.JsonRpc;

using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Serilog;
using Serilog.Events;

/// <inheritdoc />
/// <summary>
///   The default JSON-RPC message source.
/// </summary>
public class DefaultJsonRpcMessageSource : IJsonRpcMessageSource
{
  /// <inheritdoc />
  public event EventHandler? Connected;

  private readonly ILogger? _logger;
  private Uri? _webSocketEndpoint;
  private ClientWebSocket? _webSocket;

  private readonly SemaphoreSlim _reconnectSemaphore = new(1, 1);

  /// <inheritdoc />
  public WebSocketState State => _webSocket?.State ?? WebSocketState.Closed;

  /// <inheritdoc />
  public WebSocketCloseStatus? CloseStatus { get; private set; }

  /// <inheritdoc />
  public string? CloseStatusDescription { get; private set; }

  /// <inheritdoc />
  public Exception? Exception { get; private set; }

  /// <summary>
  ///   Creates an instance of the <see cref="DefaultJsonRpcMessageSource" /> class.
  /// </summary>
  /// <param name="logger">Optional implementation of the <see cref="ILogger" /> interface to enable logging capabilities.</param>
  public DefaultJsonRpcMessageSource(ILogger? logger)
  {
    _logger = logger;
  }

  /// <inheritdoc />
  public async Task Connect(Uri endpoint, CancellationToken cancellationToken = default)
  {
    _webSocketEndpoint = endpoint;

    if (_webSocket?.State is not WebSocketState.None) {
      _webSocket?.Dispose();
      _webSocket = new ClientWebSocket();
    }

    if (_webSocket is not { State: WebSocketState.Closed } and not { State: WebSocketState.None }) {
      _logger?.Warning("DefaultJsonRpcMessageSource::Connect encountered an invalid WebSocketState: {State}", _webSocket.State);
      return;
    }

    _logger?.Information("Connecting to {Endpoint}", _webSocketEndpoint);

    try {
      await _webSocket.ConnectAsync(_webSocketEndpoint, cancellationToken).ConfigureAwait(false);
      Connected?.Invoke(this, EventArgs.Empty);
    }
    catch (Exception ex) {
      _logger?.Error(ex, "DefaultJsonRpcMessageSource::Connect :: Exception while attempting to connect the ClientWebSocket to the endpoint.");
      throw;
    }
  }

  private async Task Reconnect(CancellationToken cancellationToken)
  {
    if (_reconnectSemaphore.CurrentCount < 1)
      return;

    var lockAquired = await _reconnectSemaphore.WaitAsync(1, cancellationToken).ConfigureAwait(false);

    if (!lockAquired)
      return;

    try {
      await Connect(_webSocketEndpoint!, cancellationToken).ConfigureAwait(false);
    }
    finally {
      _reconnectSemaphore.Release();
    }
  }

  /// <inheritdoc />
  public async Task Disconnect(WebSocketCloseStatus? closeStatus, string? closeStatusDescription, CancellationToken cancellationToken)
  {
    if (_webSocket is null)
      return;

    _logger?.Information("Closing connection with the endpoint.");

    if (_webSocket.State is not (WebSocketState.Open or WebSocketState.Connecting or WebSocketState.CloseReceived)) {
      _logger?.Debug("DefaultJsonRpcMessageSource::Disconnect encountered an invalid WebSocketState: {State}", _webSocket.State);
      _webSocket.Dispose();
      _webSocket = null;
      return;
    }

    var realCloseStatus = closeStatus ?? WebSocketCloseStatus.NormalClosure;
    var realCloseStatusDescription = closeStatusDescription ?? string.Empty;

    if (_webSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
      await _webSocket.CloseOutputAsync(realCloseStatus, realCloseStatusDescription, cancellationToken).ConfigureAwait(false);

    _webSocket?.Dispose();
    _webSocket = null;
  }


  /// <inheritdoc />
  public async ValueTask DisposeAsync()
  {
    await Disconnect(null, null, CancellationToken.None).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task Send(string message, CancellationToken cancellationToken = default)
  {
    if (_webSocket is null)
      throw new InvalidOperationException("Can not send a message when there is no open connection");

    while (_webSocket.State != WebSocketState.Open)
      await Task.Delay(1, cancellationToken).ConfigureAwait(false);

  retry:

    try {
      await _webSocket.SendAsync(
        new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
        WebSocketMessageType.Text,
        true,
        cancellationToken).ConfigureAwait(false);
    }
    catch (OperationCanceledException opEx) when (opEx.InnerException?.InnerException is SocketException { ErrorCode: 10054 }) {
      _logger?.Debug("DefaultJsonRpcMessageSource::Send: Socket error during send, try reconnect and send again");
      await Reconnect(cancellationToken).ConfigureAwait(false);
      goto retry;
    }
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<string> GetMessageStream([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    var msgBuffer = WebSocket.CreateClientBuffer(4096, 4096);
    var msgBuilder = new StringBuilder();

    var msgRecvStart = DateTime.MinValue;

    while (!cancellationToken.IsCancellationRequested) {
      var message = default(string?);

      try {
        if (cancellationToken.IsCancellationRequested)
          break;

        if (_webSocket is null) {
          _logger?.Debug("DefaultJsonRpcMessageSource::GetMessageStream: Socket null");
          continue;
        }

        if (_webSocket.State == WebSocketState.Aborted) {
          _logger?.Debug("DefaultJsonRpcMessageSource::GetMessageStream: Socket aborted, try reconnect");
          await Reconnect(cancellationToken).ConfigureAwait(false);
          continue;
        }

        if (_webSocket is not { State: WebSocketState.Open }) {
          _logger?.Debug("DefaultJsonRpcMessageSource::GetMessageStream: Socket not connected yet");
          continue;
        }

        WebSocketReceiveResult receiveResult;

        try {
          receiveResult = await _webSocket.ReceiveAsync(msgBuffer, cancellationToken).ConfigureAwait(false);
        }
        catch (WebSocketException wsEx) when (wsEx.InnerException?.InnerException is SocketException { ErrorCode: 10053 }) {
          _logger?.Debug("DefaultJsonRpcMessageSource::GetMessageStream: Socket error during receive, try reconnect");
          await Reconnect(cancellationToken).ConfigureAwait(false);
          continue;
        }
        catch (Exception ex) {
          _logger?.Debug(ex, "DefaultJsonRpcMessageSource::GetMessageStream: Exception during receive");
          continue;
        }

        if (msgRecvStart == DateTime.MinValue)
          msgRecvStart = DateTime.Now;

        if (receiveResult.MessageType == WebSocketMessageType.Close) {
          _logger?.Debug(
            "DefaultJsonRpcMessageSource::GetMessageStream: The endpoint initiated connection close ({Status}: {StatusDescription})",
            receiveResult.CloseStatus,
            receiveResult.CloseStatusDescription);

          await Disconnect(receiveResult.CloseStatus, receiveResult.CloseStatusDescription, CancellationToken.None).ConfigureAwait(false);
          break;
        }

        if (cancellationToken.IsCancellationRequested)
          break;

        msgBuilder.Append(Encoding.UTF8.GetString(msgBuffer.Array!, 0, receiveResult.Count));

        if (!receiveResult.EndOfMessage)
          continue;

        message = msgBuilder.ToString();

        if (_logger?.IsEnabled(LogEventLevel.Verbose) ?? false) {
          var msgRecvDiff = DateTime.Now.Subtract(msgRecvStart);
          msgRecvStart = DateTime.MinValue;

          _logger?.Verbose(
            "DefaultJsonRpcMessageSource::GetMessageStream: Received Message ({Size} ; {Duration:N4}ms): {@Message}",
            Encoding.UTF8.GetByteCount(message),
            msgRecvDiff.TotalMilliseconds,
            message);
        }
      }
      catch (TaskCanceledException) {
        break;
      }
      catch (Exception ex) {
        _logger?.Error(ex, "DefaultJsonRpcMessageSource::GetMessageStream: Connection closed by unknown error.");
        CloseStatus = WebSocketCloseStatus.Empty;
        CloseStatusDescription = null;
        Exception = ex;
      }

      if (!string.IsNullOrEmpty(message))
        yield return message!;

      msgBuilder.Clear();
    }

    _logger?.Debug("DefaultJsonRpcMessageSource::GetMessageStream: Leaving.");
  }
}
