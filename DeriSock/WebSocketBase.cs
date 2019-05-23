namespace DeriSock
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Collections.Concurrent;
  using System.Net.WebSockets;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Model;
  using Serilog;
  using Utils;

  public class WebSocketBase
  {
    public event EventHandler DisconnectEvent;
    public event EventHandler<EventResponseReceivedEventArgs> EventResponseReceived;
    public event EventHandler<MessageResponseReceivedEventArgs> MessageResponseReceived;

    protected readonly ILogger _logger = Log.Logger;
    protected readonly string _hostname;

    private volatile ClientWebSocket _websocket;
    private volatile int _requestId;

    private readonly ConcurrentDictionary<int, TaskInfo> _tasks = new ConcurrentDictionary<int, TaskInfo>();

    private readonly CancellationTokenSource _receiveLoopCancellationTokenSource = new CancellationTokenSource();

    public bool IsRunning
    {
      get => _websocket != null && _websocket.State == WebSocketState.Open;
    }

    public bool ClosedByHost { get; private set; }

    public WebSocketBase(string hostname)
    {
      _hostname = hostname;
    }

    public async Task ConnectAsync()
    {
      _websocket = new ClientWebSocket();

      _logger.Information("Connecting to websocket");
      try
      {
        await _websocket.ConnectAsync(new Uri($"wss://{_hostname}/ws/api/v2"), CancellationToken.None);
      }
      catch (Exception ex)
      {
        _websocket.Dispose();
        _websocket = null;
        _logger.Information($"Exception during connect: {ex.Message}");
        throw;
      }

      _logger.Information("Connected to websocket");

      _ = Task.Factory.StartNew(ReceiveLoopAsync, _receiveLoopCancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    public async Task DisconnectAsync()
    {
      if (_websocket == null)
      {
        return;
      }
      _logger.Information("Socket DisconnectAsync");

      _logger.Information("Shutting down the Receive Loop");
      _receiveLoopCancellationTokenSource.Cancel();

      if (_websocket.State == WebSocketState.Open)
      {
        _logger.Information("Closing Socket");
        await _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
      }

      _websocket.Dispose();
      _websocket = null;

      DisconnectEvent?.Invoke(this, EventArgs.Empty);
    }

    public Task<T> SendAsync<T>(string method, object @params, Converter.JsonConverter<T> converter)
    {
      Interlocked.CompareExchange(ref _requestId, 0, int.MaxValue);
      var reqId = Interlocked.Increment(ref _requestId);

      var request = new JsonRpcRequest()
      {
        jsonrpc = "2.0",
        id = reqId,
        method = method,
        @params = @params
      };
      var tcs = new TaskCompletionSource<T>();
      var taskInfo = new TypedTaskInfo<T>
      {
        Tcs = tcs,
        id = request.id,
        Converter = converter
      };
      _tasks[request.id] = taskInfo;
      var message = JsonConvert.SerializeObject(request);
      var buffer = Encoding.UTF8.GetBytes(message);
      _logger.Debug($"SendAsync task {method} {request.id} {message}");
      var msgInfo = new MessageInfo { task = taskInfo, message = buffer };
      _ = _websocket?.SendAsync(new ArraySegment<byte>(msgInfo.message), WebSocketMessageType.Text, true, CancellationToken.None);
      return tcs.Task;
    }

    private async Task ReceiveLoopAsync()
    {
      var buffer = new byte[4096];
      var resultMessage = "";
      while (!_receiveLoopCancellationTokenSource.IsCancellationRequested)
      {
        if (_websocket == null || _websocket.State != WebSocketState.Open) continue;

        try
        {
          var result = await _websocket.ReceiveAsync(new ArraySegment<byte>(buffer), _receiveLoopCancellationTokenSource.Token);
          if (result.MessageType == WebSocketMessageType.Close)
          {
            ClosedByHost = true;
            await _websocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None);
            await DisconnectAsync();
            break;
          }

          resultMessage = string.Concat(resultMessage, Encoding.UTF8.GetString(buffer, 0, result.Count));
          if (!result.EndOfMessage)
          {
            continue;
          }

          _ = Task.Factory.StartNew(
                                    (msg) =>
                                    {
                                      var message = (string)msg;
                                      try
                                      {
                                        _logger.Debug($"ReceiveLoopAsync message {message}");
                                        var jObject = (JObject)JsonConvert.DeserializeObject(message);
                                        if (jObject == null) return;

                                        if (jObject.ContainsKey("params"))
                                        {
                                          var eventRes = jObject.ToObject<EventResponse>();
                                          eventRes.@params.timestamp = DateTime.Now;
                                          EventResponseReceived?.Invoke(this, new EventResponseReceivedEventArgs(message, eventRes));
                                        }
                                        else
                                        {
                                          var parsedResult = jObject.ToObject<JsonRpcResponse>();
                                          if (_tasks.TryRemove(parsedResult.id, out var task))
                                          {
                                            MessageResponseReceived?.Invoke(this, new MessageResponseReceivedEventArgs(message, parsedResult, task));
                                          }
                                          else
                                          {
                                            _logger.Information($"ConsumeResponsesLoop cannot resolve task {parsedResult.id}");
                                          }
                                        }
                                      }
                                      catch (Exception ex)
                                      {
                                        _logger.Error($"ReceiveLoopAsync Error during parsing task {ex}");
                                      }
                                    }, resultMessage, _receiveLoopCancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);

          resultMessage = "";
        }
        catch (OperationCanceledException operationCanceledException)
        {
          if (operationCanceledException.CancellationToken.IsCancellationRequested)
          {
            //ignore - valid cancellation
            _logger.Debug("Valid manual Cancellation");
            return;
          }
          throw;
        }
        catch (Exception ex)
        {
          _logger.Error($"ReceiveLoopAsync error {ex}");
        }
      }
    }
  }
}
