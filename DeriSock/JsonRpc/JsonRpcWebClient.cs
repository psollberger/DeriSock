namespace DeriSock.JsonRpc
{
  using System;
  using System.Collections.Concurrent;
  using System.Net.WebSockets;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Serilog;
  using Serilog.Events;

  public sealed class JsonRpcWebClient
  {
    private readonly ILogger _logger = Log.Logger;
    private readonly Action<JsonRpcRequest> _serverRequestHandler;

    private readonly ConcurrentDictionary<int, TaskCompletionSource<JsonRpcResponse>> _openRequests =
      new ConcurrentDictionary<int, TaskCompletionSource<JsonRpcResponse>>();

    private volatile int _lastUsedRequestId;
    private Thread _processReceiveThread;
    private CancellationTokenSource _receiveCancellationTokenSource;

    private ClientWebSocket _socket;

    public bool SocketAvailable => _socket != null;

    public bool ClosedByError { get; private set; }
    public bool ClosedByClient { get; private set; }
    public bool ClosedByHost { get; private set; }

    public Uri ServerUri { get; }

    public JsonRpcWebClient(Uri serverUri, Action<JsonRpcRequest> serverRequestHandler)
    {
      _serverRequestHandler = serverRequestHandler;
      ServerUri = serverUri;
    }

    /// <summary>
    ///   Connects to the server using the <see cref="ServerUri" /> and starts processing received messages
    /// </summary>
    public async Task ConnectAsync()
    {
      if (_socket != null)
      {
        throw new JsonRpcAlreadyConnectedException();
      }

      ClosedByClient = false;
      ClosedByError = false;
      ClosedByHost = false;

      _receiveCancellationTokenSource = new CancellationTokenSource();

      if (_logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        _logger.Information("Connecting to {Host}", ServerUri);
      }

      _socket = new ClientWebSocket();
      try
      {
        await _socket.ConnectAsync(ServerUri, CancellationToken.None).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        _logger?.Error(ex, "Exception during ConnectAsync");
        _socket.Dispose();
        _socket = null;
        throw;
      }

      //Start processing Threads
      _processReceiveThread = new Thread(ProcessReceive) { Name = "ProcessReceive" };
      _processReceiveThread.Start();
    }

    /// <summary>
    ///   Stops processing of received messages and disconnects from the server
    /// </summary>
    public async Task DisconnectAsync()
    {
      if (_socket == null || _socket.State != WebSocketState.Open)
      {
        throw new JsonRpcNotConnectedException();
      }

      if (_logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        _logger.Information("Disconnecting from {Host}", ServerUri);
      }

      //Shutdown processing Threads
      _receiveCancellationTokenSource.Cancel();
      _processReceiveThread.Join();

      if (_socket.State == WebSocketState.Open)
      {
        await _socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing Connection", CancellationToken.None);
      }

      _socket.Dispose();
      _socket = null;
    }

    /// <summary>
    ///   Sends a message to the server
    /// </summary>
    /// <param name="method">A string containing the name of the method to be invoked</param>
    /// <param name="parameters">
    ///   A structured value that hold the parameter values to be used during the invocation of the
    ///   method
    /// </param>
    /// <returns>A Task object</returns>
    public Task<JsonRpcResponse> SendAsync(string method, object parameters)
    {
      var request = new JsonRpcRequest { Id = GetNextRequestId(), Method = method, Params = parameters };

      if (_logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      {
        _logger.Verbose("SendAsync: {@Request}", request);
      }

      var taskSource = new TaskCompletionSource<JsonRpcResponse>();
      _openRequests[request.Id] = taskSource;

      _ = _socket.SendAsync(
        new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request, Formatting.None))),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None);

      return taskSource.Task;
    }

    private void ProcessReceive()
    {
      var msgBuffer = new byte[0x1000];
      var msgSegment = new ArraySegment<byte>(msgBuffer);
      var msgBuilder = new StringBuilder();
      var msgRecvStart = DateTime.MinValue;

      try
      {
        while (!_receiveCancellationTokenSource.IsCancellationRequested)
        {
          if (_socket == null || _socket.State != WebSocketState.Open)
          {
            continue;
          }

          try
          {
            var receiveResult = _socket.ReceiveAsync(msgSegment, _receiveCancellationTokenSource.Token).GetAwaiter().GetResult();

            if (msgRecvStart == DateTime.MinValue)
            {
              msgRecvStart = DateTime.Now;
            }

            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
              ClosedByHost = true;
              if (_logger?.IsEnabled(LogEventLevel.Debug) ?? false)
              {
                _logger.Debug("ProcessReceive: The host closed the connection");
              }

              break;
            }

            msgBuilder.Append(Encoding.UTF8.GetString(msgBuffer, 0, receiveResult.Count));
            if (!receiveResult.EndOfMessage)
            {
              continue;
            }

            var msgRecvEnd = DateTime.Now;
            var msgRecvDiff = msgRecvEnd.Subtract(msgRecvStart);
            var message = msgBuilder.ToString();

            if (_logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              _logger.Verbose(
                "ProcessReceive: Received Message ({Size} ; {Duration}): {@Message}",
                Encoding.UTF8.GetByteCount(message), msgRecvDiff, message);
            }

            if (!string.IsNullOrEmpty(message))
            {
              var jObject = (JObject)JsonConvert.DeserializeObject(message);
              if (jObject == null)
              {
                continue;
              }

              if (jObject.TryGetValue("method", out var method))
              {
                Task.Factory.StartNew(req =>
                {
                  _serverRequestHandler?.Invoke(req as JsonRpcRequest);
                }, jObject.ToObject<JsonRpcRequest>());
              }
              else
              {
                Task.Factory.StartNew(res =>
                {
                  var response = (JsonRpcResponse)res;
                  if (response.Id <= 0) return;
                  if (_openRequests.TryRemove(response.Id, out var task))
                  {
                    task.SetResult(response);
                  }
                }, jObject.ToObject<JsonRpcResponse>());
              }
            }

            msgBuilder.Clear();
            msgRecvStart = DateTime.MinValue;
          }
          catch (OperationCanceledException) when (_receiveCancellationTokenSource.IsCancellationRequested)
          {
            //user cancelled
            ClosedByClient = true;
            _logger?.Verbose("ProcessReceive: Valid manual cancellation");

            break;
          }
          catch (Exception ex)
          {
            ClosedByError = true;
            _logger?.Error(ex, "ProcessReceive: Connection closed by unknown error");
            break;
          }
        }
      }
      finally
      {
        _logger?.Verbose("ProcessReceive: Leaving");
      }
    }

    private int GetNextRequestId()
    {
      Thread.BeginCriticalRegion();
      try
      {
        Interlocked.CompareExchange(ref _lastUsedRequestId, 0, int.MaxValue);
        return ++_lastUsedRequestId;
      }
      finally
      {
        Thread.EndCriticalRegion();
      }
    }
  }
}
