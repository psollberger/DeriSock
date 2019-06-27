namespace DeriSock
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Net.WebSockets;
  using System.Text;
  using System.Threading;
  using System.Threading.Tasks;
  using Converter;
  using Events;
  using Exceptions;
  using Model;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Serilog;
  using Utils;

  public class JsonRpcWebSocketClientConnection
  {
    protected readonly ILogger _logger = Log.Logger;
    private volatile ClientWebSocket _webSocket;
    private volatile int _requestId;
    private readonly CancellationTokenSource _receiveLoopCancellationTokenSource = new CancellationTokenSource();
    private readonly BlockingCollection<string> _receivedMessages = new BlockingCollection<string>();
    private readonly ConcurrentDictionary<int, TaskInfo> _tasks = new ConcurrentDictionary<int, TaskInfo>();
    private readonly Dictionary<string, SubscriptionEntry> _eventsMap = new Dictionary<string, SubscriptionEntry>();

    /// <summary>
    /// The Endpoint of the Service
    /// </summary>
    /// <example>wss://www.deribit.com/ws/api/v2</example>
    public Uri ServiceEndpoint { get; }
    /// <summary>
    /// Indicates if the connection was closed by an error
    /// </summary>
    public bool ClosedByError { get; private set; }
    /// <summary>
    /// Indicates if the connection was closed by the client
    /// </summary>
    public bool ClosedByClient { get; private set; }
    /// <summary>
    /// Indicates if the connection was closed by the host
    /// </summary>
    public bool ClosedByHost { get; private set; }
    /// <summary>
    /// Indicates if the connections is etablished or not
    /// </summary>
    public bool IsConnected
    {
      get => _webSocket != null && !(ClosedByHost || ClosedByClient || ClosedByError);
    }

    /// <summary>
    /// Initializes the Connection
    /// </summary>
    /// <param name="serviceEndpoint">URI to the ServiceEndpoing (e.g. wss://www.deribit.com/ws/api/v2)</param>
    public JsonRpcWebSocketClientConnection(string serviceEndpoint)
    {
      ServiceEndpoint = new Uri(serviceEndpoint);
    }

    /// <summary>
    /// Connects to the Endpoint
    /// </summary>
    /// <returns>A Task object</returns>
    public async Task ConnectAsync()
    {
      if (_webSocket != null)
      {
        throw new WebSocketAlreadyConnectedException();
      }

      _logger?.Information("Connecting to {Host}", ServiceEndpoint);
      _webSocket = new ClientWebSocket();
      try
      {
        await _webSocket.ConnectAsync(ServiceEndpoint, CancellationToken.None);
      }
      catch (Exception ex)
      {
        _logger?.Error(ex, "Exception during ConnectAsync");
        _webSocket.Dispose();
        _webSocket = null;
        throw;
      }

      _ = Task.Factory.StartNew(ProcessMessageLoop, CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Current);
      _ = Task.Factory.StartNew(ReceiveLoopAsync, _receiveLoopCancellationTokenSource.Token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    /// <summary>
    /// Disconnects from the Endpoint
    /// </summary>
    /// <returns>A Task object</returns>
    public async Task DisconnectAsync()
    {
      if (_webSocket == null || _webSocket.State != WebSocketState.Open)
      {
        throw new WebSocketNotConnectedException();
      }

      _logger?.Information("Disconnecting from {Host}", ServiceEndpoint);

      //Shutdown the Receive Thread
      _receiveLoopCancellationTokenSource.Cancel();

      //Wait for the Thread to shutdown gracefully
      while (IsConnected)
      {
        Thread.Sleep(1);
      }

      ClosedByClient = true;

      //Close the connection
      if (_webSocket.State == WebSocketState.Open)
      {
        await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing Connection", CancellationToken.None);
      }

      _webSocket.Dispose();
      _webSocket = null;
    }

    /// <summary>
    /// Sends a message to the endpoint
    /// </summary>
    /// <typeparam name="T">The message type to be sent</typeparam>
    /// <param name="method">The method to be called</param>
    /// <param name="params">The params to be transmitted</param>
    /// <param name="converter">The converter to be used to parse the response</param>
    /// <returns>A Task object</returns>
    public Task<T> SendAsync<T>(string method, object @params, Converter.IJsonConverter<T> converter)
    {
      Interlocked.CompareExchange(ref _requestId, 0, int.MaxValue);
      var reqId = Interlocked.Increment(ref _requestId);

      var request = new JsonRpcRequest() { jsonrpc = "2.0", id = reqId, method = method, @params = @params };
      var tcs = new TaskCompletionSource<T>();
      var taskInfo = new TypedTaskInfo<T> { Tcs = tcs, id = request.id, Converter = converter };

      _logger?.Verbose("SendAsync task {@Request}", request);

      _tasks[request.id] = taskInfo;
      var message = JsonConvert.SerializeObject(request);
      var buffer = Encoding.UTF8.GetBytes(message);
      var msgInfo = new MessageInfo { task = taskInfo, message = buffer };
      _ = _webSocket.SendAsync(new ArraySegment<byte>(msgInfo.message), WebSocketMessageType.Text, true, CancellationToken.None);
      return tcs.Task;
    }

    public Task<TestResponse> TestAsync(string expectedResult)
    {
      return SendAsync("public/test", new { expected_result = expectedResult }, new ObjectJsonConverter<TestResponse>());
    }

    private async Task ReceiveLoopAsync()
    {
      var buffer = new byte[4096];
      var resultMessage = new StringBuilder();
      try
      {
        while (!_receiveLoopCancellationTokenSource.IsCancellationRequested)
        {
          if (_webSocket == null || _webSocket.State != WebSocketState.Open) continue;

          try
          {
            var result = await _webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), _receiveLoopCancellationTokenSource.Token);
            if (result.MessageType == WebSocketMessageType.Close)
            {
              ClosedByHost = true;
              ClosedByClient = false;
              _logger?.Debug("The host closed the connection");
              break;
            }

            resultMessage.Append(Encoding.UTF8.GetString(buffer, 0, result.Count));
            if (!result.EndOfMessage)
            {
              continue;
            }

            _receivedMessages.Add(resultMessage.ToString());
            resultMessage.Clear();
          }
          catch (OperationCanceledException)
          {
            if (_receiveLoopCancellationTokenSource.IsCancellationRequested)
            {
              //ignore - valid cancellation
              ClosedByClient = true;
              _logger?.Verbose("Valid manual Cancellation");
              return;
            }

            throw;
          }
          catch (Exception)
          {
            ClosedByError = true;
          }
        }
      }
      finally
      {
        _receivedMessages.CompleteAdding();
      }
      _logger?.Verbose("Leaving ReceiveLoop");
    }

    private void ProcessMessageLoop()
    {
      while (!_receivedMessages.IsCompleted)
      {
        string message = null;
        // Blocks if _receivedMessages.Count == 0.
        // IOE means that Take() was called on a completed collection.
        // Some other thread can call CompleteAdding after we pass the
        // IsCompleted check but before we call Take. 
        // In this example, we can simply catch the exception since the 
        // loop will break on the next iteration.
        try
        {
          message = _receivedMessages.Take();
        }
        catch (InvalidOperationException) { }

        if (string.IsNullOrEmpty(message))
        {
          continue;
        }

        try
        {
          _logger?.Verbose("ProcessMessageLoop {Message}", message);
          var jObject = (JObject)JsonConvert.DeserializeObject(message);
          if (jObject == null) return;

          if (jObject.ContainsKey("params"))
          {
            var eventRes = jObject.ToObject<EventResponse>();
            eventRes.@params.timestamp = DateTime.Now;
            ProcessEventResponse(new EventResponseReceivedEventArgs(message, eventRes));
          }
          else
          {
            var parsedResult = jObject.ToObject<JsonRpcResponse>();
            if (_tasks.TryRemove(parsedResult.id, out var task))
            {
              ProcessMessageResponse(new MessageResponseReceivedEventArgs(message, parsedResult, task));
            }
            else
            {
              _logger?.Warning("ProcessMessageLoop cannot resolve task {@ParsedResult}", parsedResult);
            }
          }
        }
        catch (Exception ex)
        {
          _logger?.Error(ex, "ProcessMessageLoop: Error during parsing message");
        }
      }
    }

    private void ProcessMessageResponse(MessageResponseReceivedEventArgs e)
    {
      try
      {
        if (e.Response.error != null)
        {
          if (e.Response.error.Type == JTokenType.Object)
          {
            var error = e.Response.error.ToObject<JsonRpcError>();
            Task.Factory.StartNew(
                                  () =>
                                  {
                                    e.TaskInfo.Reject(new InvalidResponseException(e.Response, error, $"(Object) Invalid response for {e.Response.id}, code: {error.code}, message: {error.message}"));
                                  }, TaskCreationOptions.LongRunning);
          }
          else
          {
            Task.Factory.StartNew(
                                  () =>
                                  {
                                    e.TaskInfo.Reject(new InvalidResponseException(e.Response, null, $"(Non-Object) Invalid response for {e.Response.id}, code: {e.Response.error}"));
                                  }, TaskCreationOptions.LongRunning);
          }
        }
        else
        {
          var convertedResult = e.TaskInfo.Convert(e.Response.result);

          _logger?.Verbose("MessageResponseReceived task resolve Id {Id}", e.Response.id);
          Task.Factory.StartNew(
                                () =>
                                {
                                  e.TaskInfo.Resolve(convertedResult);
                                }, TaskCreationOptions.LongRunning);
        }
      }
      catch (Exception ex)
      {
        _logger?.Error(ex, "ConsumeResponsesLoop Error during parsing task");
        Task.Factory.StartNew(
                              () =>
                              {
                                e.TaskInfo.Reject(ex);
                              }, TaskCreationOptions.LongRunning);
      }
    }

    private void ProcessEventResponse(EventResponseReceivedEventArgs e)
    {
      lock (_eventsMap)
      {
        if (e.EventData.method == "heartbeat")
        {
          _logger?.Debug("Hearbeat received: {@Heartbeat}", e.EventData);
          if (e.EventData.@params.type == "test_request")
          {
            ProcessHearbeatResponse();
          }
          return;
        }
        if (!_eventsMap.TryGetValue(e.EventData.@params.channel, out var entry))
        {
          _logger?.Warning("Could not find event for: {@EventData}", e.EventData);
          return;
        }

        foreach (var callback in entry.Callbacks)
        {
          try
          {
            var t = Task.Factory.StartNew(
                                  () =>
                                  {
                                    callback(e.EventData);
                                  });
          }
          catch (Exception ex)
          {
            _logger?.Error(ex, "ProcessEventResponse Error during calling event callback {@EventData}", e.EventData);
          }
        }
      }
    }

    private void ProcessHearbeatResponse()
    {
      TestAsync("ok");
    }
  }
}
