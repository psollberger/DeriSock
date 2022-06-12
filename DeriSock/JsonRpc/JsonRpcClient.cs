namespace DeriSock.JsonRpc;

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Utils;
using DeriSock.WebSocket;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;
using Serilog.Events;

internal class JsonRpcClient : IJsonRpcClient
{
  protected readonly ILogger? Logger;
  protected readonly RequestIdGenerator RequestIdGenerator = new();
  protected readonly RequestManager RequestMgr;

  protected ITextMessageWebSocket Socket;

  public JsonRpcClient(Uri serverUri, ILogger? logger)
  {
    ServerUri = serverUri;
    Logger = logger;
    RequestMgr = new RequestManager();
    Socket = TextMessageWebSocketFactory.Create(logger);

    Socket.MessageReceived += (_, e) => InternalOnMessageReceived(e.Message);
    Socket.ConnectionClosed += (_, e) => InternalOnDisconnected(new JsonRpcDisconnectEventArgs(e.CloseStatus, e.CloseStatusDescription, e.Error));
  }

  public event EventHandler? Connected;
  public event EventHandler<JsonRpcDisconnectEventArgs>? Disconnected;
  public event EventHandler<JsonRpcRequest>? RequestReceived;

  public Uri ServerUri { get; }

  public WebSocketState State => Socket.State;
  public WebSocketCloseStatus? CloseStatus { get; protected set; }
  public string? CloseStatusDescription { get; protected set; }
  public Exception? Error { get; protected set; }

  /// <inheritdoc />
  public virtual async Task Connect()
  {
    if (Socket.State is not (WebSocketState.Closed or WebSocketState.None))
      throw new JsonRpcAlreadyConnectedException();

    RequestMgr.Reset();

    if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      Logger.Information("Connecting to {Host}", ServerUri);

    try {
      await Socket.ConnectAsync(ServerUri, CancellationToken.None).ConfigureAwait(false);
    }
    catch (Exception ex) {
      Logger?.Error(ex, "Exception during ConnectAsync");
      throw;
    }

    if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      Logger?.Information("Connection established");

    _ = Task.Run(OnConnected).ConfigureAwait(false);
  }

  /// <inheritdoc />
  public virtual async Task Disconnect()
  {
    if (Socket is not { State: WebSocketState.Open })
      throw new JsonRpcNotConnectedException();

    if (Logger?.IsEnabled(LogEventLevel.Information) ?? false)
      Logger.Information("Disconnecting from {Host}", ServerUri);

    if (Socket.State == WebSocketState.Open)
      await Socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "user_close", CancellationToken.None);
  }

  /// <inheritdoc />
  public virtual void SendLogoutSync(string method, object parameters)
  {
    var request = new JsonRpcRequest
    {
      Id = RequestIdGenerator.Next(), Method = method, Params = parameters
    };

    if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      Logger.Verbose("Send: {@Request}", request);

    Socket.SendMessageAsync(JsonConvert.SerializeObject(request, Formatting.None), CancellationToken.None);
  }

  /// <inheritdoc />
  public virtual Task<JsonRpcResponse> Send(string method, object parameters)
  {
    var request = new JsonRpcRequest
    {
      Id = RequestIdGenerator.Next(), Method = method, Params = parameters
    };

    if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      Logger.Verbose("SendAsync: {@Request}", request);

    var taskSource = new TaskCompletionSource<JsonRpcResponse>();
    RequestMgr.Add(request.Id, request, taskSource);

    _ = Socket.SendMessageAsync(JsonConvert.SerializeObject(request, Formatting.None), CancellationToken.None);

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

  protected virtual void InternalOnDisconnected(JsonRpcDisconnectEventArgs args)
  {
    if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
      Logger?.Debug("Disconnected");

    CloseStatus = args.CloseStatus;
    CloseStatusDescription = args.CloseStatusDescription;
    Error = args.Exception;

    // After a disconnect, do not call Socket.CloseAsync unless the socket state is Open, CloseReceived or CloseSent
    if (args.Exception != null && Socket.State is WebSocketState.Open or WebSocketState.CloseReceived or WebSocketState.CloseSent)
      Socket.CloseAsync(args.CloseStatus, args.CloseStatusDescription, CancellationToken.None).GetAwaiter().GetResult();

    Task.Run(() => OnDisconnected(args)).ConfigureAwait(false);
  }

  protected virtual void InternalOnMessageReceived(string message)
  {
    if (JsonConvert.DeserializeObject(message) is not JObject jObject)
      return;

    if (jObject.TryGetValue("method", out _))
      InternalOnRequestReceived(message, jObject);
    else
      InternalOnResponseReceived(message, jObject);
  }

  protected virtual void InternalOnRequestReceived(string message, JObject requestObject)
  {
    var request = requestObject.ToObject<JsonRpcRequest>();
    Debug.Assert(request != null, nameof(request) + " != null");
    request.Original = requestObject;
    Task.Run(() => OnRequestReceived(request)).ConfigureAwait(false);
  }

  protected virtual void InternalOnResponseReceived(string message, JObject responseObject)
  {
    var response = responseObject.ToObject<JsonRpcResponse>();
    Debug.Assert(response != null, nameof(response) + " != null");
    response.Original = message;

    if (response.Id > 0)
      Task.Factory.StartNew(
        res =>
        {
          var r = (JsonRpcResponse)res;

          if (!RequestMgr.TryRemove(r.Id, out var request, out var taskSource)) {
            if (Logger?.IsEnabled(LogEventLevel.Warning) ?? false)
              Logger?.Warning("Could not find request id {reqId}", r.Id);

            return;
          }

          if (r.Error != null)
            taskSource!.SetException(new JsonRpcRequestException(request, r));
          else
            taskSource!.SetResult(r);
        }, response);
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

    public bool TryRemove(int id, out JsonRpcRequest request, out TaskCompletionSource<JsonRpcResponse>? taskSource)
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
