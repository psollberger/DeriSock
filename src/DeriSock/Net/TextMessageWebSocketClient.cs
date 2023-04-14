#nullable enable
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeriSock.Net;
using Serilog;
using Serilog.Events;

public sealed class TextMessageWebSocketClient : ITextMessageClient, IDisposable
{
  private readonly ILogger? _logger;
  private Uri? _websocketEndpoint;
  private ClientWebSocket _websocket = null!;
  private readonly CancellationTokenSource _cts = new CancellationTokenSource();

  public bool IsConnected { get; private set; }

  public TextMessageWebSocketClient(ILogger? logger)
  {
    _logger = logger;
  }

  public async Task Connect(Uri endpoint, CancellationToken cancellationToken = default)
  {
    if (endpoint == null)
      throw new ArgumentNullException(nameof(endpoint));

    if (IsConnected)
      throw new InvalidOperationException();

    _websocketEndpoint = endpoint;
    _websocket = new ClientWebSocket();
    _logger?.Information("Connecting to {Endpoint}", _websocketEndpoint);

    try
    {
      await _websocket.ConnectAsync(_websocketEndpoint, cancellationToken).ConfigureAwait(false);
      IsConnected = true;
    }
    catch (Exception ex)
    {
      _logger?.Error(ex,
        "TextMessageWebSocketClient::Connect::Exception while attempting to connect the ClientWebSocket to the endpoint");
      throw;
    }
  }

  public async Task Reconnect(CancellationToken cancellationToken = default)
  {
    try
    {
      if (IsConnected)
      {
        await Disconnect(cancellationToken).ConfigureAwait(false);
      }

      if (_websocketEndpoint == null)
      {
        throw new InvalidOperationException("Endpoint is not set.");
      }

      await Connect(_websocketEndpoint, cancellationToken).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      _logger?.Error(ex, "TextMessageWebSocketClient::Reconnect::Exception while attempting to reconnect");
    }
  }

  public async Task<bool> ReconnectWithRetry(int maxRetries, CancellationToken cancellationToken = default)
  {
    int retries = 0;
    while (retries < maxRetries)
    {
      try
      {
        await Reconnect(cancellationToken).ConfigureAwait(false);
        return true;
      }
      catch (Exception ex)
      {
        _logger?.Error(ex, $"Reconnect attempt {retries + 1} of {maxRetries} failed");
        retries++;
      }

      await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retries)), cancellationToken).ConfigureAwait(false);
    }

    return false;
  }

  public async Task Disconnect(CancellationToken cancellationToken)
  {
    if (!IsConnected)
      throw new InvalidOperationException();

    if (_websocket == null)
      return;

    _logger?.Information("Closing connection to the endpoint");

    if (_websocket.State is not (WebSocketState.Open or WebSocketState.Connecting or WebSocketState.CloseReceived))
    {
      _logger?.Debug("TextMessageWebSocketClient::Disconnect encountered an invalid WebSocketState: {State}",
        _websocket.State);
      _websocket.Dispose();
      _websocket = null!;
      IsConnected = false;
      return;
    }

    if (_websocket.State is WebSocketState.Open or WebSocketState.CloseReceived)
    {
      await _websocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, cancellationToken)
        .ConfigureAwait(false);
    }

    _websocket.Dispose();
    _websocket = null!;
    IsConnected = false;
  }

  public async ValueTask DisposeAsync()
  {
    await Disconnect(CancellationToken.None).ConfigureAwait(false);
    _cts.Dispose();
  }

  public void Dispose()
  {
    DisposeAsync().AsTask().GetAwaiter().GetResult();
    GC.SuppressFinalize(this);
  }

  public async Task Send(string message, CancellationToken cancellationToken = default)
  {
    if (!IsConnected)
      throw new InvalidOperationException("Cannot send a message when there is no open connection");

    try
    {
      await _websocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(message)), WebSocketMessageType.Text,
        true, cancellationToken).ConfigureAwait(false);
    }
    catch (WebSocketException wsex) when (wsex.InnerException?.InnerException is SocketException sockex)
    {
      _logger?.Debug(sockex, "TextMessageWebSocketClient::GetMessageStream: SocketException during receive");
      await ReconnectWithRetry(maxRetries: 3, cancellationToken: cancellationToken).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
      _logger?.Debug(ex, "TextMessageWebSocketClient::Send: Error during send");
      await Disconnect(CancellationToken.None).ConfigureAwait(false);
      throw;
    }
  }

  public async IAsyncEnumerable<string> GetMessageStream(
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
  {
    const int bufferSize = 4096;
    var msgBufferBytes = ArrayPool<byte>.Shared.Rent(bufferSize);

    try
    {
      var msgBuffer = new ArraySegment<byte>(msgBufferBytes);
      var msgBuilder = new StringBuilder(bufferSize);

      while (!cancellationToken.IsCancellationRequested)
      {
        var message = string.Empty;

        if (cancellationToken.IsCancellationRequested)
          break;

        if (_websocket is null)
        {
          _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: Socket null");
          continue;
        }

        if (_websocket.State == WebSocketState.Aborted)
        {
          _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: WebSocket suddenly in aborted state");
          await Disconnect(CancellationToken.None).ConfigureAwait(false);
          throw new SocketException((int) SocketError.ConnectionAborted);
        }

        if (_websocket is {State: WebSocketState.Connecting})
        {
          _logger?.Debug("TextMessageWebSocketClient::GetMessageStream: Socket not fully connected yet");
          continue;
        }

        WebSocketReceiveResult receiveResult;

        try
        {
          receiveResult = await _websocket.ReceiveAsync(msgBuffer, cancellationToken).ConfigureAwait(false);
        }
        catch (WebSocketException wsex) when (wsex.InnerException?.InnerException is SocketException sockex)
        {
          _logger?.Debug(sockex, "TextMessageWebSocketClient::GetMessageStream: SocketException during receive");
          await Disconnect(CancellationToken.None).ConfigureAwait(false);
          throw sockex;
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

        msgBuilder.Append(Encoding.UTF8.GetString(msgBuffer.Array, msgBuffer.Offset, receiveResult.Count));

        if (!receiveResult.EndOfMessage)
          continue;

        message = msgBuilder.ToString();

        if (_logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          _logger?.Verbose("TextMessageWebSocketClient::GetMessageStream: Received message ({Size}): {@Message}",
            Encoding.UTF8.GetByteCount(message), message);

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
