namespace DeriSock.Net;

using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Serilog;
using Serilog.Events;

// TODO: Take a look at https://github.com/dimaaan/WebSocketTextClient/tree/master/WebSocketTextClient to maybe improve WebSocket handling

/// <inheritdoc />
/// <summary>
///   An <see cref="ITextMessageClient" /> implementation using a <see cref="ClientWebSocket" />.
/// </summary>
public sealed class TextMessageWebSocketClient : ITextMessageClient
{
  private readonly ILogger? _logger;
  private Uri? _webSocketEndpoint;
  private ClientWebSocket _webSocket = null!;

  /// <inheritdoc />
  public bool IsConnected { get; private set; }

  /// <inheritdoc />
  public IWebProxy? Proxy { get; set; }

  /// <summary>
  ///   Creates an instance of the <see cref="TextMessageWebSocketClient" /> class.
  /// </summary>
  /// <param name="logger">Optional implementation of the <see cref="ILogger" /> interface to enable logging capabilities.</param>
  public TextMessageWebSocketClient(ILogger? logger)
  {
    _logger = logger;
  }

  /// <inheritdoc />
  public async Task Connect(Uri endpoint, CancellationToken cancellationToken = default)
  {
    if (IsConnected)
      throw new InvalidOperationException();

    _webSocketEndpoint = endpoint;

    _webSocket = new ClientWebSocket();

    if (Proxy is not null)
      _webSocket.Options.Proxy = Proxy;

    _logger?.Information("Connecting to {Endpoint}", _webSocketEndpoint);

    try
    {
      await _webSocket.ConnectAsync(_webSocketEndpoint, cancellationToken).ConfigureAwait(false);
      IsConnected = true;
    }
    catch (Exception ex)
    {
      _logger?.Error(ex, "TextMessageWebSocketClient::Connect :: Exception while attempting to connect the ClientWebSocket to the endpoint");
      throw;
    }
  }

  /// <inheritdoc />
  public async Task Disconnect(CancellationToken cancellationToken)
  {
    if (!IsConnected)
      throw new InvalidOperationException();

    _logger?.Information("Closing connection to the endpoint");

    if (_webSocket.State is not (WebSocketState.Open or WebSocketState.Connecting or WebSocketState.CloseReceived))
    {
      _logger?.Debug("TextMessageWebSocketClient::Disconnect encountered an invalid WebSocketState: {State}", _webSocket.State);
      _webSocket.Dispose();
      _webSocket = null!;
      IsConnected = false;
      return;
    }

    if (_webSocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
      await _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken).ConfigureAwait(false);

    _webSocket.Dispose();
    _webSocket = null!;
    IsConnected = false;
  }

  /// <inheritdoc />
  public async ValueTask DisposeAsync()
  {
    await Disconnect(CancellationToken.None).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public async Task Send(string message, CancellationToken cancellationToken = default)
  {
    if (!IsConnected)
      throw new InvalidOperationException("Can not send a message when there is no open connection");

    try
    {
      await _webSocket.SendAsync(
        new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)),
        WebSocketMessageType.Text,
        true,
        cancellationToken
      ).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      _logger?.Debug(ex, "TextMessageWebSocketClient::Send: Error during send");
      await Disconnect(CancellationToken.None).ConfigureAwait(false);
      throw;
    }
  }

  /// <inheritdoc />
  public async IAsyncEnumerable<string> GetMessageStream([EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    const int bufferSize = 4096;

    var msgBufferBytes = ArrayPool<byte>.Shared.Rent(bufferSize);

    try
    {
#if NETSTANDARD2_0
      var msgBuffer = new ArraySegment<byte>(msgBufferBytes);
#else
      var msgBuffer = new Memory<byte>(msgBufferBytes);
#endif

      var msgBuilder = new StringBuilder(bufferSize);

      while (!cancellationToken.IsCancellationRequested)
      {
        var message = string.Empty;

        try
        {
          if (cancellationToken.IsCancellationRequested)
            break;

          if (_webSocket is null)
          {
            _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: Socket null");
            continue;
          }

          if (_webSocket.State == WebSocketState.Aborted)
          {
            _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: WebSocket suddenly in aborted state");
            await Disconnect(CancellationToken.None).ConfigureAwait(false);
            throw new SocketException((int)SocketError.ConnectionAborted);
          }

          if (_webSocket is { State: WebSocketState.Connecting })
          {
            _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: Socket not fully connected yet");
            continue;
          }

#if NETSTANDARD2_0
          WebSocketReceiveResult receiveResult;
#else
          ValueWebSocketReceiveResult receiveResult;
#endif

          try
          {
            receiveResult = await _webSocket.ReceiveAsync(msgBuffer, cancellationToken).ConfigureAwait(false);
          }
          catch (WebSocketException wsEx) when (wsEx.InnerException?.InnerException is SocketException sockEx)
          {
            _logger?.Debug(sockEx, "TextMessageWebSocketClient::GetMessageStream: SocketException during receive");
            await Disconnect(CancellationToken.None).ConfigureAwait(false);
            throw sockEx;
          }
          catch (Exception ex)
          {
            _logger?.Debug(ex, "TextMessageWebSocketClient::GetMessageStream: Exception during receive");
            continue;
          }

          if (receiveResult.MessageType == WebSocketMessageType.Close)
          {
            _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: The endpoint initiated connection close");
            await Disconnect(CancellationToken.None).ConfigureAwait(false);
            break;
          }

          if (cancellationToken.IsCancellationRequested)
            break;

          msgBuilder.Append(Encoding.UTF8.GetString(msgBuffer, 0, receiveResult.Count));

          if (!receiveResult.EndOfMessage)
            continue;

          message = msgBuilder.ToString();

          if (_logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            _logger?.Verbose(
              "TextMessageWebSocketClient::GetMessageStream: Received Message ({Size}): {@Message}",
              Encoding.UTF8.GetByteCount(message),
              message
            );
        }
        catch (TaskCanceledException)
        {
          // Cancellation token was signaled.
          break;
        }
        catch (Exception ex)
        {
          _logger?.Error(ex, "TextMessageWebSocketClient::GetMessageStream: Connection closed by unknown error");
        }

        if (!string.IsNullOrEmpty(message))
          yield return message;

        msgBuilder.Clear();
      }

      _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: Leaving");
    }
    finally
    {
      ArrayPool<byte>.Shared.Return(msgBufferBytes);
    }
  }
}
