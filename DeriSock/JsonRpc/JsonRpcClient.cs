namespace DeriSock.JsonRpc;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DeriSock.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;

internal class JsonRpcClient : IJsonRpcClient
{
  protected readonly ILogger Logger;
  protected readonly RequestIdGenerator RequestIdGenerator = new();
  protected readonly RequestManager RequestMgr;
  protected Thread ProcessReceiveThread;
  protected CancellationTokenSource ReceiveCancellationTokenSource;

  protected IWebSocket Socket;

  public JsonRpcClient(Uri serverUri, ILogger logger)
  {
    ServerUri = serverUri;
    Logger = logger;
    RequestMgr = new RequestManager();
  }

  public event EventHandler Connected;
  public event EventHandler<JsonRpcDisconnectEventArgs> Disconnected;
  public event EventHandler<JsonRpcRequest> RequestReceived;

  public Uri ServerUri { get; }

  public WebSocketState State => Socket?.State ?? WebSocketState.Closed;
  public WebSocketCloseStatus? CloseStatus { get; protected set; }
  public string CloseStatusDescription { get; protected set; }
  public Exception Error { get; protected set; }

  /// <inheritdoc />
  public virtual async Task Connect()
  {
    if (Socket != null)
    {
      throw new JsonRpcAlreadyConnectedException();
    }

    Socket?.Dispose();
    RequestMgr.Reset();

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

    InternalOnConnected();
  }

  /// <inheritdoc />
  public virtual async Task Disconnect()
  {
    if (Socket == null || Socket.State != WebSocketState.Open)
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
      await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "user close", CancellationToken.None);
    }

    Socket?.Dispose();
    Socket = null;
  }

  /// <inheritdoc />
  public virtual void SendLogoutSync(string method, object parameters)
  {
    var request = new JsonRpcRequest
    {
      Id = RequestIdGenerator.Next(), Method = method, Params = parameters
    };

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
  public virtual Task<JsonRpcResponse> Send(string method, object parameters)
  {
    var request = new JsonRpcRequest
    {
      Id = RequestIdGenerator.Next(), Method = method, Params = parameters
    };

    if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
    {
      Logger.Verbose("SendAsync: {@Request}", request);
    }

    var taskSource = new TaskCompletionSource<JsonRpcResponse>();
    RequestMgr.Add(request.Id, request, taskSource);

    _ = Socket.SendAsync(
      new ArraySegment<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(request, Formatting.None))),
      WebSocketMessageType.Text,
      true,
      CancellationToken.None);

    return taskSource.Task;
  }

  protected virtual void OnConnected()
  {
    Connected?.Invoke(this, EventArgs.Empty);
  }

  protected virtual void OnDisconnected(JsonRpcDisconnectEventArgs args)
  {
    Disconnected?.Invoke(this, args);
  }

  protected virtual void OnRequestReceived(JsonRpcRequest request)
  {
    RequestReceived?.Invoke(this, request);
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
              Logger.Debug("ProcessReceive: The host closed the connection ({Status}: {StatusDescription})", receiveResult.CloseStatus, receiveResult.CloseStatusDescription);
            }

            InternalOnDisconnected(new JsonRpcDisconnectEventArgs(
              receiveResult.CloseStatus ?? WebSocketCloseStatus.Empty,
              receiveResult.CloseStatusDescription, null));
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
            InternalOnMessageReceived(message);
          }

          msgBuilder.Clear();
        }
        catch (OperationCanceledException) when (ReceiveCancellationTokenSource.IsCancellationRequested)
        {
          if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
          {
            Logger?.Debug("ProcessReceive: Valid manual cancellation");
          }

          InternalOnDisconnected(new JsonRpcDisconnectEventArgs(WebSocketCloseStatus.NormalClosure, "user close", null));
          break;
        }
        catch (Exception ex)
        {
          Logger?.Error(ex, "ProcessReceive: Connection closed by unknown error");
          //Fixing #18: When using close status code 'Empty' the description must be null
          InternalOnDisconnected(new JsonRpcDisconnectEventArgs(WebSocketCloseStatus.Empty, null, ex));
          break;
        }
      }
    }
    finally
    {
      Logger?.Debug("ProcessReceive: Leaving");
    }
  }

  protected virtual void InternalOnConnected()
  {
    if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
    {
      Logger.Information("Connected. Start collecting messages");
    }

    //Start processing Threads
    ProcessReceiveThread = new Thread(CollectMessages)
    {
      Name = "ProcessReceive"
    };
    ProcessReceiveThread.Start();

    Task.Factory.StartNew(OnConnected);
  }

  protected virtual void InternalOnDisconnected(JsonRpcDisconnectEventArgs args)
  {
    if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
    {
      Logger.Debug("Disconnected");
    }

    CloseStatus = args.CloseStatus;
    CloseStatusDescription = args.CloseStatusDescription;
    Error = args.Exception;

    // After a disconnect, do not call Socket.CloseAsync unless the socket state is Open, CloseReceived or CloseSent
    if (args.Exception != null && Socket.State is WebSocketState.Open or WebSocketState.CloseReceived or WebSocketState.CloseSent)
    {
      Socket.CloseAsync(args.CloseStatus, args.CloseStatusDescription, CancellationToken.None).GetAwaiter().GetResult();
    }

    Socket?.Dispose();
    Socket = null;

    Task.Factory.StartNew(a =>
    {
      OnDisconnected((JsonRpcDisconnectEventArgs)a);
    }, args);
  }

  protected virtual void InternalOnMessageReceived(string message)
  {
    var jObject = (JObject)JsonConvert.DeserializeObject(message);
    if (jObject == null)
    {
      return;
    }

    if (jObject.TryGetValue("method", out _))
    {
      InternalOnRequestReceived(message, jObject);
    }
    else
    {
      InternalOnResponseReceived(message, jObject);
    }
  }

  protected virtual void InternalOnRequestReceived(string message, JObject requestObject)
  {
    var request = requestObject.ToObject<JsonRpcRequest>();
    Debug.Assert(request != null, nameof(request) + " != null");
    request.Original = requestObject;
    Task.Factory.StartNew(req =>
    {
      OnRequestReceived((JsonRpcRequest)req);
    }, request);
  }

  protected virtual void InternalOnResponseReceived(string message, JObject responseObject)
  {
    var response = responseObject.ToObject<JsonRpcResponse>();
    Debug.Assert(response != null, nameof(response) + " != null");
    response.Original = message;

    if (response.Id > 0)
    {
      Task.Factory.StartNew(res =>
      {
        var r = (JsonRpcResponse)res;
        if (!RequestMgr.TryRemove(r.Id, out var request, out var taskSource))
        {
          if (Logger?.IsEnabled(LogEventLevel.Warning) ?? false)
          {
            Logger.Warning("Could not find request id {reqId}", r.Id);
          }

          return;
        }

        if (r.Error != null)
        {
          taskSource.SetException(new JsonRpcRequestException(request, r));
        }
        else
        {
          taskSource.SetResult(r);
        }
      }, response);
    }
  }

  internal class RequestManager
  {
    private readonly ConcurrentDictionary<int, JsonRpcRequest> _requestObjects;
    private readonly ConcurrentDictionary<int, TaskCompletionSource<JsonRpcResponse>> _taskSources;

    public RequestManager()
    {
      _taskSources = new ConcurrentDictionary<int, TaskCompletionSource<JsonRpcResponse>>();
      _requestObjects = new ConcurrentDictionary<int, JsonRpcRequest>();
    }

    public void Add(int id, JsonRpcRequest request, TaskCompletionSource<JsonRpcResponse> taskSource)
    {
      _taskSources[id] = taskSource;
      _requestObjects[id] = request;
    }

    public bool TryRemove(int id, out JsonRpcRequest request, out TaskCompletionSource<JsonRpcResponse> taskSource)
    {
      taskSource = null;
      return _requestObjects.TryRemove(id, out request) && _taskSources.TryRemove(id, out taskSource);
    }

    public void Reset()
    {
      _taskSources.Clear();
      _requestObjects.Clear();
    }
  }
}
