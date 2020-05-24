namespace DeriSock
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
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

  public class DeribitWebSocketClientV2
  {
    protected readonly ILogger _logger = Log.Logger;
    protected volatile ClientWebSocket _webSocket;
    private volatile int _requestId;
    private readonly CancellationTokenSource _receiveLoopCancellationTokenSource = new CancellationTokenSource();
    private readonly BlockingCollection<string> _receivedMessages = new BlockingCollection<string>();
    private readonly ConcurrentDictionary<int, TaskInfo> _tasks = new ConcurrentDictionary<int, TaskInfo>();
    private readonly Dictionary<string, SubscriptionEntry> _eventsMap = new Dictionary<string, SubscriptionEntry>();
    private readonly List<Tuple<string, object, object>> _listeners = new List<Tuple<string, object, object>>();

    #region Properties

    public Uri ServiceEndpoint { get; }

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public bool ClosedByError { get; private set; }

    public bool ClosedByClient { get; private set; }

    public bool ClosedByHost { get; private set; }

    public bool IsConnected
    {
      get => _webSocket != null && !(ClosedByHost || ClosedByClient || ClosedByError);
    }

    #endregion

    public DeribitWebSocketClientV2(string serviceBaseAddress)
    {
      ServiceEndpoint = new Uri($"wss://{serviceBaseAddress}/ws/api/v2");
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

      _tasks[request.id] = taskInfo;
      var message = JsonConvert.SerializeObject(request);
      var buffer = Encoding.UTF8.GetBytes(message);
      var msgInfo = new MessageInfo { task = taskInfo, message = buffer };
      _ = _webSocket.SendAsync(new ArraySegment<byte>(msgInfo.message), WebSocketMessageType.Text, true, CancellationToken.None);
      _logger?.Verbose("SendAsync task {@Request}", request);
      return tcs.Task;
    }

    #region Connection Handling

    private async Task ReceiveLoopAsync()
    {
      var buffer = new byte[9216];
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

            _logger?.Verbose($"Received Message: {resultMessage}");
            _logger?.Verbose($"Received Message Size is: {Encoding.UTF8.GetByteCount(resultMessage.ToString())}");

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
        _logger?.Verbose("Leaving ReceiveLoop");
      }
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

          _logger?.Verbose("ProcessMessageResponse task resolve Id {Id}", e.Response.id);
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
            Task.Factory.StartNew(
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
      SendAsync("public/test", new { expected_result = "ok" }, new ObjectJsonConverter<TestResponse>());
    }

    #endregion

    #region Subscriptions

    protected Task<List<string>> SubscribePublicAsync(string[] channels)
    {
      return SendAsync("public/subscribe", new { channels }, new ListJsonConverter<string>());
    }

    protected Task<List<string>> UnsubscribePublicAsync(string[] channels)
    {
      return SendAsync("public/unsubscribe", new { channels }, new ListJsonConverter<string>());
    }

    protected Task<List<string>> SubscribePrivateAsync(string[] channels, string accessToken)
    {
      return SendAsync("private/subscribe", new { channels, access_token = accessToken }, new ListJsonConverter<string>());
    }

    protected Task<List<string>> UnsubscribePrivateAsync(string[] channels, string accessToken)
    {
      return SendAsync("private/unsubscribe", new { channels, access_token = accessToken }, new ListJsonConverter<string>());
    }

    protected async Task<bool> ManagedSubscribeAsync(string channel, bool @private, string accessToken, Action<EventResponse> callback)
    {
      SubscriptionEntry entry;
      TaskCompletionSource<bool> defer = null;
      lock (_eventsMap)
      {
        if (_eventsMap.ContainsKey(channel))
        {
          entry = _eventsMap[channel];
          switch (entry.State)
          {
            case SubscriptionState.Subscribed:
              {
                _logger?.Verbose($"Already subsribed added to callbacks {channel}");
                if (callback != null)
                {
                  entry.Callbacks.Add(callback);
                }
                return true;
              }

            case SubscriptionState.Unsubscribing:
              _logger?.Verbose($"Unsubscribing return false {channel}");
              return false;

            case SubscriptionState.Unsubscribed:

              _logger?.Verbose($"Unsubscribed resubscribing {channel}");
              entry.State = SubscriptionState.Subscribing;
              defer = new TaskCompletionSource<bool>();
              entry.CurrentAction = defer.Task;
              break;
          }
        }
        else
        {
          _logger?.Verbose($"Not exists subscribing {channel}");
          defer = new TaskCompletionSource<bool>();
          entry = new SubscriptionEntry()
          {
            State = SubscriptionState.Subscribing,
            Callbacks = new List<Action<EventResponse>>(),
            CurrentAction = defer.Task
          };
          _eventsMap[channel] = entry;
        }
      }
      if (defer == null)
      {
        _logger?.Verbose($"Empty defer wait for already subscribing {channel}");
        var currentAction = entry.CurrentAction;
        var result = currentAction != null && await currentAction;
        _logger?.Verbose($"Empty defer wait for already subscribing res {result} {channel}");
        lock (_eventsMap)
        {
          if (!result || entry.State != SubscriptionState.Subscribed)
          {
            return false;
          }

          _logger?.Verbose($"Empty defer adding callback {channel}");
          if (callback != null)
          {
            entry.Callbacks.Add(callback);
          }
          return true;
        }
      }
      try
      {
        _logger?.Verbose($"Subscribing {channel}");
        var response = !@private ? await SubscribePublicAsync(new[] { channel }) : await SubscribePrivateAsync(new[] { channel }, accessToken);
        if (response.Count != 1 || response[0] != channel)
        {
          _logger?.Verbose($"Invalid subscribe result {response} {channel}");
          defer.SetResult(false);
        }
        else
        {
          lock (_eventsMap)
          {
            _logger?.Verbose($"Successfully subscribed adding callback {channel}");
            entry.State = SubscriptionState.Subscribed;
            if (callback != null)
            {
              entry.Callbacks.Add(callback);
            }
            entry.CurrentAction = null;
          }
          defer.SetResult(true);
        }
      }
      catch (Exception e)
      {
        defer.SetException(e);
      }
      return await defer.Task;
    }

    protected async Task<bool> ManagedUnsubscribeAsync(string channel, bool @private, string accessToken, Action<EventResponse> callback)
    {
      SubscriptionEntry entry;
      TaskCompletionSource<bool> defer;
      lock (_eventsMap)
      {
        if (!_eventsMap.ContainsKey(channel))
        {
          return false;
        }
        entry = _eventsMap[channel];
        if (!entry.Callbacks.Contains(callback))
        {
          return false;
        }
        switch (entry.State)
        {
          case SubscriptionState.Subscribing:
            return false;
          case SubscriptionState.Unsubscribed:
          case SubscriptionState.Unsubscribing:
            entry.Callbacks.Remove(callback);
            return true;
          case SubscriptionState.Subscribed:
            if (entry.Callbacks.Count > 1)
            {
              entry.Callbacks.Remove(callback);
              return true;
            }
            entry.State = SubscriptionState.Unsubscribing;
            defer = new TaskCompletionSource<bool>();
            entry.CurrentAction = defer.Task;
            break;
          default: return false;
        }
      }
      try
      {
        var response = !@private ? await UnsubscribePublicAsync(new[] { channel }) : await UnsubscribePrivateAsync(new[] { channel }, accessToken);
        if (response.Count != 1 || response[0] != channel)
        {
          defer.SetResult(false);
        }
        else
        {
          lock (_eventsMap)
          {
            entry.State = SubscriptionState.Unsubscribed;
            entry.Callbacks.Remove(callback);
            entry.CurrentAction = null;
          }
          defer.SetResult(true);
        }
      }
      catch (Exception e)
      {
        defer.SetException(e);
      }
      return await defer.Task;
    }

    protected async Task<bool> PublicSubscribeAsync<T>(string channelName, Action<T> originalCallback, Action<EventResponse> myCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, false, null, myCallback)) return false;
      lock (_listeners)
      {
        _listeners.Add(Tuple.Create(channelName, (object)originalCallback, (object)myCallback));
      }
      return true;
    }

    protected async Task<bool> UnsubscribePublicAsync<T>(string channelName, Action<T> originalCallback)
    {
      Tuple<string, object, object> entry;
      lock (_listeners)
      {
        entry = _listeners.FirstOrDefault(x => x.Item1 == channelName && x.Item2 == (object)originalCallback);
      }
      if (entry == null) return false;
      if (!await ManagedUnsubscribeAsync(channelName, false, null, (Action<EventResponse>)entry.Item3)) return false;
      lock (_listeners)
      {
        _listeners.Remove(entry);
      }
      return true;
    }

    protected async Task<bool> PrivateSubscribeAsync<T>(string channelName, Action<T> originalCallback, Action<EventResponse> myCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, true, AccessToken, myCallback)) return false;
      lock (_listeners)
      {
        _listeners.Add(Tuple.Create(channelName, (object)originalCallback, (object)myCallback));
      }
      return true;
    }

    protected async Task<bool> UnsubscribePrivateAsync<T>(string channelName, Action<T> originalCallback)
    {
      Tuple<string, object, object> entry;
      lock (_listeners)
      {
        entry = _listeners.FirstOrDefault(x => x.Item1 == channelName && x.Item2 == (object)originalCallback);
      }
      if (entry == null) return false;
      if (!await ManagedUnsubscribeAsync(channelName, true, AccessToken, (Action<EventResponse>)entry.Item3)) return false;
      lock (_listeners)
      {
        _listeners.Remove(entry);
      }
      return true;
    }

    #endregion

    #region API v2 Functionality

    public Task<string> PingAsync()
    {
      return SendAsync("public/ping", new { }, new ObjectJsonConverter<string>());
    }

    public async Task<AuthResponse> PublicAuthAsync(string accessKey, string accessSecret, string sessionName)
    {
      _logger.Debug("Authenticate");
      var scope = "connection";
      if (!string.IsNullOrEmpty(sessionName))
      {
        scope = $"session:{sessionName} expires:60";
      }
      var loginRes = await SendAsync("public/auth", new
      {
        grant_type = "client_credentials",
        client_id = accessKey,
        client_secret = accessSecret,
        scope = scope
      }, new ObjectJsonConverter<AuthResponse>());
      AccessToken = loginRes.access_token;
      RefreshToken = loginRes.refresh_token;
      _ = Task.Delay(TimeSpan.FromSeconds(loginRes.expires_in - 5)).ContinueWith(t =>
      {
        if (IsConnected) _ = PublicAuthRefreshAsync(RefreshToken);
      });
      return loginRes;
    }

    public async Task<AuthResponse> PublicAuthRefreshAsync(string refreshToken)
    {
      _logger.Debug("Refreshing Auth");
      var loginRes = await SendAsync("public/auth", new
      {
        grant_type = "refresh_token",
        refresh_token = refreshToken
      }, new ObjectJsonConverter<AuthResponse>());
      AccessToken = loginRes.access_token;
      RefreshToken = loginRes.refresh_token;
      _ = Task.Delay(TimeSpan.FromSeconds(loginRes.expires_in - 5)).ContinueWith(t =>
      {
        if (IsConnected) _ = PublicAuthRefreshAsync(RefreshToken);
      });
      return loginRes;
    }

    public Task<object> PublicSetHeartbeatAsync(int intervalSeconds)
    {
      return SendAsync("public/set_heartbeat", new { interval = intervalSeconds }, new ObjectJsonConverter<object>());
    }

    public Task<object> PublicDisableHeartbeatAsync()
    {
      return SendAsync("public/disable_heartbeat", new { }, new ObjectJsonConverter<object>());
    }

    public Task<object> PrivateDisableCancelOnDisconnectAsync()
    {
      return SendAsync("private/disable_cancel_on_disconnect", new { access_token = AccessToken }, new ObjectJsonConverter<object>());
    }

    public Task<AccountSummaryResponse> PrivateGetAccountSummaryAsync()
    {
      return SendAsync("private/get_account_summary", new { currency = "BTC", extended = true, access_token = AccessToken }, new ObjectJsonConverter<AccountSummaryResponse>());
    }

    public Task<bool> PublicSubscribeBookAsync(string instrument, int group, int depth, Action<BookResponse> callback)
    {
      return PublicSubscribeAsync("book." + instrument + "." + (group == 0 ? "none" : group.ToString()) + "." + depth + ".100ms", callback, response =>
      {
        callback(response.@params.data.ToObject<BookResponse>());
      });
    }

    public Task<bool> PrivateSubscribeOrdersAsync(string instrument, Action<OrderResponse> callback)
    {
      return PrivateSubscribeAsync("user.orders." + instrument + ".raw", callback, response =>
      {
        var orderResponse = response.@params.data.ToObject<OrderResponse>();
        orderResponse.timestamp = response.@params.timestamp;
        callback(orderResponse);
      });
    }

    public Task<bool> PrivateSubscribePortfolioAsync(string currency, Action<PortfolioResponse> callback)
    {
      return PrivateSubscribeAsync($"user.portfolio.{currency.ToLower()}", callback, response =>
      {
        callback(response.@params.data.ToObject<PortfolioResponse>());
      });
    }

    public Task<bool> PublicSubscribeTickerAsync(string instrument, string interval, Action<TickerResponse> callback)
    {
      return PublicSubscribeAsync($"ticker.{instrument}.{interval}", callback, response =>
      {
        callback(response.@params.data.ToObject<TickerResponse>());
      });
    }

    public Task<BookResponse> PublicGetOrderBookAsync(string instrument, int depth)
    {
      return SendAsync("public/get_order_book", new
      {
        instrument_name = instrument,
        depth
      }, new ObjectJsonConverter<BookResponse>());
    }

    public Task<OrderItem[]> PrivateGetOpenOrdersAsync(string instrument)
    {
      return SendAsync("private/get_open_orders_by_instrument", new
      {
        instrument_name = instrument,
        access_token = AccessToken
      }, new ObjectJsonConverter<OrderItem[]>());
    }

    public Task<BuySellResponse> PrivateBuyLimitAsync(string instrument, double amount, double price, string label)
    {
      return SendAsync("private/buy", new
      {
        instrument_name = instrument,
        amount,
        type = "limit",
        label = label,
        price,
        time_in_force = "good_til_cancelled",
        post_only = true,
        access_token = AccessToken
      }, new ObjectJsonConverter<BuySellResponse>());
    }

    public Task<BuySellResponse> PrivateSellLimitAsync(string instrument, double amount, double price, string label)
    {
      return SendAsync("private/sell", new
      {
        instrument_name = instrument,
        amount,
        type = "limit",
        label = label,
        price,
        time_in_force = "good_til_cancelled",
        post_only = true,
        access_token = AccessToken
      }, new ObjectJsonConverter<BuySellResponse>());
    }

    public async Task<OrderResponse> PrivateGetOrderStateAsnyc(string orderId)
    {
      try
      {
        var result = await SendAsync("private/get_order_state", new
        {
          order_id = orderId,
          access_token = AccessToken
        }, new ObjectJsonConverter<OrderResponse>());
        return result;
      }
      catch
      {
        return null;
      }
    }

    public Task<object> PrivateCancelOrderAsync(string orderId)
    {
      return SendAsync("private/cancel", new
      {
        order_id = orderId,
        access_token = AccessToken
      }, new ObjectJsonConverter<object>());
    }

    public Task<object> PrivateCancelAllOrdersByInstrumentAsync(string instrument)
    {
      return SendAsync("private/cancel_all_by_instrument", new
      {
        instrument_name = instrument,
        access_token = AccessToken
      }, new ObjectJsonConverter<object>());
    }

    public Task<SettlementResponse> PrivateGetSettlementHistoryByInstrumentAsync(string instrument, int count)
    {
      return SendAsync("private/get_settlement_history_by_instrument", new
      {
        instrument_name = instrument,
        type = "settlement",
        count = count,
        access_token = AccessToken
      }, new ObjectJsonConverter<SettlementResponse>());
    }

    public Task<SettlementResponse> PrivateGetSettlementHistoryByCurrencyAsync(string currency, int count)
    {
      return SendAsync("private/get_settlement_history_by_currency", new
      {
        currency = currency,
        type = "settlement",
        count = count,
        access_token = AccessToken
      }, new ObjectJsonConverter<SettlementResponse>());
    }

    #endregion
  }
}
