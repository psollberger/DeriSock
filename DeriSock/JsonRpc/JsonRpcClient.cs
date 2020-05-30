namespace DeriSock.JsonRpc
{
  using System;
  using System.Net.WebSockets;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.Utils;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Serilog;
  using Serilog.Events;
  using RequestDictionary =
    System.Collections.Concurrent.ConcurrentDictionary<int, System.Threading.Tasks.TaskCompletionSource<JsonRpcResponse>>;

  //TODO: Create OnDisconnect Event or something to let consumers know the connection was closed. Maybe put ClosedBy* properties into DisconnectReason Enum or something
  public class JsonRpcClient : IJsonRpcClient
  {
    protected readonly ILogger Logger = Log.Logger;
    protected readonly RequestDictionary OpenRequests = new RequestDictionary();
    protected Thread ProcessReceiveThread;
    protected CancellationTokenSource ReceiveCancellationTokenSource;
    protected readonly RequestIdGenerator RequestIdGenerator = new RequestIdGenerator();

    protected IWebSocket Socket;

    public event EventHandler<JsonRpcRequest> Request;
    protected virtual void OnRequest(JsonRpcRequest request)
    {
      Request?.Invoke(this, request);
    }

    public bool SocketAvailable => Socket != null;
    public bool IsConnected => SocketAvailable && !(ClosedByHost || ClosedByClient || ClosedByError);
    public bool ClosedByError { get; private set; }
    public bool ClosedByClient { get; private set; }
    public bool ClosedByHost { get; private set; }

    public Uri ServerUri { get; }

    public JsonRpcClient(Uri serverUri)
    {
      ServerUri = serverUri;
    }

    /// <inheritdoc />
    public virtual async Task ConnectAsync()
    {
      if (IsConnected)
      {
        throw new JsonRpcAlreadyConnectedException();
      }

      Socket?.Dispose();
      OpenRequests.Clear();
      ClosedByClient = false;
      ClosedByError = false;
      ClosedByHost = false;

      ReceiveCancellationTokenSource = new CancellationTokenSource();

      if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        Logger.Information("Connecting to {Host}", ServerUri);
      }

      Socket = WebSocketFactory.Create();
      try
      {
        await Socket.ConnectAsync(ServerUri, CancellationToken.None).ConfigureAwait(false);
      }
      catch (Exception ex)
      {
        Logger?.Error(ex, "Exception during ConnectAsync");
        Socket.Dispose();
        Socket = null;
        throw;
      }

      if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        Logger.Information("Connected. Start collecting messages");
      }

      //Start processing Threads
      ProcessReceiveThread = new Thread(CollectMessages) { Name = "ProcessReceive" };
      ProcessReceiveThread.Start();
    }

    /// <inheritdoc />
    public virtual async Task DisconnectAsync()
    {
      if (!IsConnected || Socket.State != WebSocketState.Open)
      {
        throw new JsonRpcNotConnectedException();
      }

      if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        Logger.Information("Disconnecting from {Host}", ServerUri);
      }

      //Shutdown processing Threads
      ReceiveCancellationTokenSource.Cancel();
      ProcessReceiveThread.Join();

      if (Socket.State == WebSocketState.Open)
      {
        await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing Connection", CancellationToken.None);
      }

      Socket.Dispose();
      Socket = null;

      if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      {
        Logger.Information("Disconnected");
      }
    }

    /// <inheritdoc />
    public virtual void SendLogout(string method, object parameters)
    {
      var request = new JsonRpcRequest { Id = RequestIdGenerator.Next(), Method = method, Params = parameters };

      if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      {
        Logger.Verbose("Send: {@Request}", request);
      }

      Socket.SendAsync(
        new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request, Formatting.None))),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <inheritdoc />
    public virtual Task<JsonRpcResponse> SendAsync(string method, object parameters)
    {
      var request = new JsonRpcRequest { Id = RequestIdGenerator.Next(), Method = method, Params = parameters };

      if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      {
        Logger.Verbose("SendAsync: {@Request}", request);
      }

      var taskSource = new TaskCompletionSource<JsonRpcResponse>();
      OpenRequests[request.Id] = taskSource;

      _ = Socket.SendAsync(
        new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request, Formatting.None))),
        WebSocketMessageType.Text,
        true,
        CancellationToken.None);

      return taskSource.Task;
    }
    
    protected virtual void CollectMessages()
    {
      var msgBuffer = new byte[0x1000];
      var msgSegment = new ArraySegment<byte>(msgBuffer);
      var msgBuilder = new StringBuilder();

      var verboseDebugEnabled = Logger?.IsEnabled(LogEventLevel.Verbose) ?? false;
      var msgRecvStart = DateTime.MinValue;

      try
      {
        while (!ReceiveCancellationTokenSource.IsCancellationRequested)
        {
          if (Socket == null || Socket.State != WebSocketState.Open)
          {
            if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
            {
              Logger?.Debug("ProcessReceive: Socket null or not connected");
            }
            continue;
          }

          try
          {
            var receiveResult = Socket.ReceiveAsync(msgSegment, ReceiveCancellationTokenSource.Token).GetAwaiter().GetResult();

            if (verboseDebugEnabled && msgRecvStart == DateTime.MinValue)
            {
              msgRecvStart = DateTime.Now;
            }

            if (receiveResult.MessageType == WebSocketMessageType.Close)
            {
              if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
              {
                Logger.Debug("ProcessReceive: The host closed the connection ({StatusDescription})", receiveResult.CloseStatusDescription);
              }
              OnConnectionClosed(!string.Equals(receiveResult.CloseStatusDescription, "logout"));
              break;
            }

            msgBuilder.Append(Encoding.UTF8.GetString(msgBuffer, 0, receiveResult.Count));
            if (!receiveResult.EndOfMessage)
            {
              continue;
            }

            var message = msgBuilder.ToString();

            if (verboseDebugEnabled)
            {
              var msgRecvDiff = DateTime.Now.Subtract(msgRecvStart);
              msgRecvStart = DateTime.MinValue;

              Logger.Verbose(
                "ProcessReceive: Received Message ({Size} ; {Duration:N3}ms): {@Message}",
                Encoding.UTF8.GetByteCount(message), msgRecvDiff.TotalMilliseconds, message);
            }

            if (!string.IsNullOrEmpty(message))
            {
              OnMessageReceived(message);
            }

            msgBuilder.Clear();
          }
          catch (OperationCanceledException) when (ReceiveCancellationTokenSource.IsCancellationRequested)
          {
            if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
            {
              Logger?.Debug("ProcessReceive: Valid manual cancellation");
            }
            OnConnectionClosed(false);
            break;
          }
          catch (Exception ex)
          {
            Logger?.Error(ex, "ProcessReceive: Connection closed by unknown error");
            OnConnectionError(ex);
            break;
          }
        }
      }
      finally
      {
        Logger?.Debug("ProcessReceive: Leaving");
      }
    }

    protected virtual void OnMessageReceived(string message)
    {
      var jObject = (JObject)JsonConvert.DeserializeObject(message);
      if (jObject == null)
      {
        return;
      }

      if (jObject.TryGetValue("method", out _))
      {
        OnRequest(jObject);
      }
      else
      {
        OnResponse(jObject);
      }
    }

    protected virtual void OnRequest(JObject requestObject)
    {
      var request = requestObject.ToObject<JsonRpcRequest>();
      request.Original = requestObject;
      Task.Factory.StartNew(req =>
      {
        OnRequest((JsonRpcRequest)req);
      }, request);
    }

    protected virtual void OnResponse(JObject responseObject)
    {
      var response = responseObject.ToObject<JsonRpcResponse>();
      response.Original = responseObject;

      if (response.Id > 0)
      {
        Task.Factory.StartNew(res =>
        {
          var r = (JsonRpcResponse)res;
          if (OpenRequests.TryRemove(r.Id, out var task))
          {
            task.SetResult(r);
          }
        }, response);
      }
    }

    protected virtual void OnConnectionClosed(bool closedByHost)
    {
      ClosedByHost = closedByHost;
      ClosedByClient = !closedByHost;
    }

    protected virtual void OnConnectionError(Exception ex)
    {
      ClosedByHost = false;
      ClosedByClient = false;
      ClosedByError = true;
    }
  }
}
