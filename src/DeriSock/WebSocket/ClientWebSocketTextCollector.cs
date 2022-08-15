namespace DeriSock.WebSocket;

using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Serilog;
using Serilog.Events;

/// <inheritdoc />
/// <summary>
///   Collects text messages on a given <see cref="T:System.Net.WebSockets.ClientWebSocket" />
/// </summary>
public sealed class ClientWebSocketTextCollector : IDisposable
{
  private readonly ILogger? _logger;

  private Thread? _collectThread;

  public ClientWebSocket WebSocket { get; set; }

  /// <summary>
  ///   The <see cref="MessageReady" /> callback is executed when a message was collected from the <see cref="ClientWebSocket" />
  /// </summary>
  public Action<string>? MessageReady;

  /// <summary>
  /// The <see cref="CloseInitiated"/> callback is executed when the WebSocket sends a <see cref="WebSocketMessageType.Close"/> message
  /// </summary>
  public Action<WebSocketCloseStatus?, string?, Exception?>? CloseInitiated;

  /// <summary>
  ///   The <see cref="ErrorOccurred" /> callback is executed when an error occurred
  /// </summary>
  public Action<WebSocketCloseStatus?, string?, Exception?>? ErrorOccurred;

  /// <summary>
  ///   Creates an instance of the <see cref="ClientWebSocketTextCollector" /> class
  /// </summary>
  /// <param name="webSocket">The <see cref="ClientWebSocket" /> to collect text messages from</param>
  /// <param name="logger">An optional <see cref="ILogger" /> that will be used for logging</param>
  public ClientWebSocketTextCollector(ClientWebSocket webSocket, ILogger? logger)
  {
    WebSocket = webSocket;
    _logger = logger;
  }

  /// <inheritdoc />
  public void Dispose()
  {
    StopCollect();
  }

  /// <summary>
  ///   Starts collecting text from the <see cref="ClientWebSocket" />
  /// </summary>
  public void StartCollect()
  {
    if (_collectThread != null) {
      if (_logger?.IsEnabled(LogEventLevel.Warning) ?? false)
        _logger?.Warning("{Method} was called, but text collection is already running", nameof(StartCollect));

      return;
    }

    if (_logger?.IsEnabled(LogEventLevel.Debug) ?? false)
      _logger?.Debug("Starting to collect text");
    
    _collectThread = new Thread(DoCollect)
    {
      Name = "ClientWebSocketTextCollector"
    };

    _collectThread.Start();
  }

  /// <summary>
  ///   Stops collecting text from the <see cref="ClientWebSocket" />
  /// </summary>
  public void StopCollect()
  {
    if (_collectThread == null) {
      if (_logger?.IsEnabled(LogEventLevel.Warning) ?? false)
        _logger?.Warning("{Method} was called, but text collection isn't running", nameof(StopCollect));

      return;
    }

    if (_logger?.IsEnabled(LogEventLevel.Debug) ?? false)
      _logger?.Debug("Stopping to collect text");

    WebSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
    _collectThread?.Join();
  }

  private void DoCollect()
  {
    var msgBuffer = System.Net.WebSockets.WebSocket.CreateClientBuffer(4096, 4096);
    var msgBuilder = new StringBuilder();

    var verboseDebugEnabled = _logger?.IsEnabled(LogEventLevel.Verbose) ?? false;
    var msgRecvStart = DateTime.MinValue;

    try {
      while (true) {
        if (WebSocket is not { State: WebSocketState.Open }) {
          if (_logger?.IsEnabled(LogEventLevel.Debug) ?? false)
            _logger?.Debug("ProcessReceive: Socket null or not connected yet");

          continue;
        }

        try {
          var receiveResult = WebSocket.ReceiveAsync(msgBuffer, CancellationToken.None).GetAwaiter().GetResult();

          if (verboseDebugEnabled && msgRecvStart == DateTime.MinValue)
            msgRecvStart = DateTime.Now;

          if (receiveResult.MessageType == WebSocketMessageType.Close) {
            if (_logger?.IsEnabled(LogEventLevel.Debug) ?? false)
              _logger?.Debug("ProcessReceive: The host closed the connection ({Status}: {StatusDescription})", receiveResult.CloseStatus, receiveResult.CloseStatusDescription);

            if (CloseInitiated is not null)
              Task.Run(() => CloseInitiated.Invoke(receiveResult.CloseStatus, receiveResult.CloseStatusDescription, null)).ConfigureAwait(false);
            break;
          }

          msgBuilder.Append(Encoding.UTF8.GetString(msgBuffer.Array!, 0, receiveResult.Count));

          if (!receiveResult.EndOfMessage)
            continue;

          var message = msgBuilder.ToString();

          if (verboseDebugEnabled) {
            var msgRecvDiff = DateTime.Now.Subtract(msgRecvStart);
            msgRecvStart = DateTime.MinValue;

            if (_logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
              _logger?.Verbose("ProcessReceive: Received Message ({Size} ; {Duration:N4}ms): {@Message}", Encoding.UTF8.GetByteCount(message), msgRecvDiff.TotalMilliseconds, message);
          }

          if (!string.IsNullOrEmpty(message))
            if (MessageReady is not null)
              Task.Run(() => MessageReady.Invoke(message)).ConfigureAwait(false);

          //if (MessageReceived is not null)
          //  Task.Run(() => MessageReceived.Invoke(message)).ConfigureAwait(false);

          msgBuilder.Clear();
        }
        catch (OperationCanceledException) {
          if (_logger?.IsEnabled(LogEventLevel.Debug) ?? false)
            _logger?.Debug("ProcessReceive: Valid manual cancellation");
          break;
        }
        catch (Exception ex) {
          if (_logger?.IsEnabled(LogEventLevel.Error) ?? false)
            _logger?.Error(ex, "ProcessReceive: Connection closed by unknown error");

          if (ErrorOccurred is not null)
            Task.Run(() => ErrorOccurred.Invoke(WebSocketCloseStatus.Empty, null, ex)).ConfigureAwait(false);
          break;
        }
      }
    }
    finally {
      _logger?.Debug("ProcessReceive: Leaving");
    }
  }
}
