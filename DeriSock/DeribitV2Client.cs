// ReSharper disable UnusedMember.Local

namespace DeriSock
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.Converter;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using DeriSock.Utils;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Serilog.Events;

  public class DeribitV2Client : ClientBase
  {
    private readonly ConcurrentDictionary<int, TaskInfo> _openRequests = new ConcurrentDictionary<int, TaskInfo>();

    private readonly Dictionary<string, SubscriptionEntry> _subscriptionMap =
      new Dictionary<string, SubscriptionEntry>();

    private readonly List<Tuple<string, object, object>> _subscriptions = new List<Tuple<string, object, object>>();
    private volatile int _requestId;

    public DeribitV2Client(string serviceBaseAddress) : base(new Uri($"wss://{serviceBaseAddress}/ws/api/v2"))
    {
    }

    #region Properties

    public string AccessToken { get; set; }

    public string RefreshToken { get; set; }

    public bool IsConnected => SocketAvailable && !(ClosedByHost || ClosedByClient || ClosedByError);

    #endregion

    #region Connection Handling

    /// <summary>
    ///   Sends a message to the endpoint
    /// </summary>
    /// <typeparam name="T">The message type to be sent</typeparam>
    /// <param name="method">The method to be called</param>
    /// <param name="params">The params to be transmitted</param>
    /// <param name="converter">The converter to be used to parse the response</param>
    /// <returns>A Task object</returns>
    public Task<T> SendAsync<T>(string method, object @params, IJsonConverter<T> converter)
    {
      var reqId = GetNextRequestId();
      var taskInfo = new TypedTaskInfo<T> {Tcs = new TaskCompletionSource<T>(), id = reqId, Converter = converter};

      _openRequests[reqId] = taskInfo;

      var request = new Request {Id = reqId, Method = method, Params = @params};

      if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      {
        Logger.Verbose("SendAsync: {@Request}", request);
      }

      _ = SendAsync(JsonConvert.SerializeObject(request, Formatting.None));
      
      return taskInfo.Tcs.Task;
    }

    protected override void OnMessage(Message message)
    {
      try
      {
        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose("OnMessage: {@Message}", message);
        }

        try
        {
          var jObject = (JObject)JsonConvert.DeserializeObject(message.Data);
          if (jObject == null)
          {
            return;
          }

          if (jObject.TryGetValue("method", out var method))
          {
            var methodName = method.Value<string>();
            if (methodName == "subscription")
            {
              var eventRes = jObject.ToObject<Notification>();
              eventRes.Timestamp = message.ReceiveEnd;
              OnNotification(eventRes);
            }
            else if (methodName == "heartbeat")
            {
              OnHeartbeat(jObject.ToObject<Heartbeat>());
            }
          }
          else
          {
            var parsedResult = jObject.ToObject<Response>();
            if (_openRequests.TryRemove(parsedResult.Id, out var task))
            {
              OnResponse(task, parsedResult);
            }
            else
            {
              if (Logger?.IsEnabled(LogEventLevel.Warning) ?? false)
              {
                Logger.Warning("OnMessage: Cannot resolve request for response: {@ParsedResult}", parsedResult);
              }
            }
          }
        }
        catch (Exception ex)
        {
          Logger?.Error(ex, "OnMessage: Error during parsing message");
        }
      }
      catch (Exception ex)
      {
        Logger?.Error(ex, "OnMessage: Error during receive handling");
      }
    }

    private void OnHeartbeat(Heartbeat heartbeat)
    {
      if (Logger?.IsEnabled(LogEventLevel.Debug) ?? false)
      {
        Logger.Debug("OnHeartbeat: {@Heartbeat}", heartbeat);
      }

      if (heartbeat.Type == "test_request")
      {
        SendAsync("public/test", new {expected_result = "ok"}, new ObjectJsonConverter<TestResponse>());
      }
    }

    private void OnResponse(TaskInfo taskInfo, Response response)
    {
      if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      {
        Logger.Verbose("OnResponse: {@Response}", response);
      }

      try
      {
        if (response.Error != null)
        {
          Task.Factory.StartNew(
            () =>
            {
              taskInfo.Reject(new InvalidResponseException(response,
                $"(Object) Invalid response for {response.Id}, code: {response.Error.Code}, message: {response.Error.Message}"));
            }, TaskCreationOptions.LongRunning);
        }
        else
        {
          var convertedResult = taskInfo.Convert(response.Result);
          if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            Logger.Verbose("OnResponse: request for found for Id {Id}", response.Id);
          }

          Task.Factory.StartNew(
            () =>
            {
              taskInfo.Resolve(convertedResult);
            }, TaskCreationOptions.LongRunning);
        }
      }
      catch (Exception ex)
      {
        Logger?.Error(ex, "OnResponse: Error during parsing");
        Task.Factory.StartNew(
          () =>
          {
            taskInfo.Reject(ex);
          }, TaskCreationOptions.LongRunning);
      }
    }

    private void OnNotification(Notification notification)
    {
      if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      {
        Logger.Verbose("OnNotification: {@Notification}", notification);
      }
      lock (_subscriptionMap)
      {
        if (!_subscriptionMap.TryGetValue(notification.Channel, out var entry))
        {
          if (Logger?.IsEnabled(LogEventLevel.Warning) ?? false)
          {
            Logger.Warning("OnNotification: Could not find subscription for notification: {@Notification}", notification);
          }

          return;
        }

        foreach (var callback in entry.Callbacks)
        {
          try
          {
            Task.Factory.StartNew(
              () =>
              {
                callback(notification);
              });
          }
          catch (Exception ex)
          {
            Logger?.Error(ex, "OnNotification: Error during event callback call: {@Notification}", notification);
          }
        }
      }
    }

    private int GetNextRequestId()
    {
      Thread.BeginCriticalRegion();
      try
      {
        Interlocked.CompareExchange(ref _requestId, 0, int.MaxValue);
        return ++_requestId;
      }
      finally
      {
        Thread.EndCriticalRegion();
      }
    }

    #endregion

    #region Subscriptions

    private Task<List<string>> SubscribePublicAsync(string[] channels)
    {
      return SendAsync("public/subscribe", new {channels}, new ListJsonConverter<string>());
    }

    private Task<List<string>> UnsubscribePublicAsync(string[] channels)
    {
      return SendAsync("public/unsubscribe", new {channels}, new ListJsonConverter<string>());
    }

    private Task<List<string>> SubscribePrivateAsync(string[] channels, string accessToken)
    {
      return SendAsync("private/subscribe", new {channels, access_token = accessToken},
        new ListJsonConverter<string>());
    }

    private Task<List<string>> UnsubscribePrivateAsync(string[] channels, string accessToken)
    {
      return SendAsync("private/unsubscribe", new {channels, access_token = accessToken},
        new ListJsonConverter<string>());
    }

    private async Task<bool> ManagedSubscribeAsync(string channel, bool @private, string accessToken,
      Action<Notification> callback)
    {
      SubscriptionEntry entry;
      TaskCompletionSource<bool> defer = null;
      lock (_subscriptionMap)
      {
        if (_subscriptionMap.ContainsKey(channel))
        {
          entry = _subscriptionMap[channel];
          switch (entry.State)
          {
            case SubscriptionState.Subscribed:
            {
              if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
              {
                Logger.Verbose($"Already subscribed added to callbacks {channel}");
              }

              if (callback != null)
              {
                entry.Callbacks.Add(callback);
              }

              return true;
            }

            case SubscriptionState.Unsubscribing:
              if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
              {
                Logger.Verbose($"Unsubscribing return false {channel}");
              }

              return false;

            case SubscriptionState.Unsubscribed:
              if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
              {
                Logger.Verbose($"Unsubscribed resubscribing {channel}");
              }

              entry.State = SubscriptionState.Subscribing;
              defer = new TaskCompletionSource<bool>();
              entry.CurrentAction = defer.Task;
              break;
          }
        }
        else
        {
          if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            Logger.Verbose($"Not exists subscribing {channel}");
          }

          defer = new TaskCompletionSource<bool>();
          entry = new SubscriptionEntry
          {
            State = SubscriptionState.Subscribing,
            Callbacks = new List<Action<Notification>>(),
            CurrentAction = defer.Task
          };
          _subscriptionMap[channel] = entry;
        }
      }

      if (defer == null)
      {
        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose($"Empty defer wait for already subscribing {channel}");
        }

        var currentAction = entry.CurrentAction;
        var result = currentAction != null && await currentAction;
        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose($"Empty defer wait for already subscribing res {result} {channel}");
        }

        lock (_subscriptionMap)
        {
          if (!result || entry.State != SubscriptionState.Subscribed)
          {
            return false;
          }

          if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            Logger.Verbose($"Empty defer adding callback {channel}");
          }

          if (callback != null)
          {
            entry.Callbacks.Add(callback);
          }

          return true;
        }
      }

      try
      {
        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose($"Subscribing {channel}");
        }

        var response = !@private
          ? await SubscribePublicAsync(new[] {channel})
          : await SubscribePrivateAsync(new[] {channel}, accessToken);
        if (response.Count != 1 || response[0] != channel)
        {
          if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            Logger.Verbose($"Invalid subscribe result {response} {channel}");
          }

          defer.SetResult(false);
        }
        else
        {
          lock (_subscriptionMap)
          {
            if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              Logger.Verbose($"Successfully subscribed adding callback {channel}");
            }

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

    private async Task<bool> ManagedUnsubscribeAsync(string channel, bool @private, string accessToken,
      Action<Notification> callback)
    {
      SubscriptionEntry entry;
      TaskCompletionSource<bool> defer;
      lock (_subscriptionMap)
      {
        if (!_subscriptionMap.ContainsKey(channel))
        {
          return false;
        }

        entry = _subscriptionMap[channel];
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
        var response = !@private
          ? await UnsubscribePublicAsync(new[] {channel})
          : await UnsubscribePrivateAsync(new[] {channel}, accessToken);
        if (response.Count != 1 || response[0] != channel)
        {
          defer.SetResult(false);
        }
        else
        {
          lock (_subscriptionMap)
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

    private async Task<bool> PublicSubscribeAsync<T>(string channelName, Action<T> originalCallback,
      Action<Notification> myCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, false, null, myCallback))
      {
        return false;
      }

      lock (_subscriptions)
      {
        _subscriptions.Add(Tuple.Create(channelName, (object)originalCallback, (object)myCallback));
      }

      return true;
    }

    private async Task<bool> UnsubscribePublicAsync<T>(string channelName, Action<T> originalCallback)
    {
      Tuple<string, object, object> entry;
      lock (_subscriptions)
      {
        entry = _subscriptions.FirstOrDefault(x => x.Item1 == channelName && x.Item2 == (object)originalCallback);
      }

      if (entry == null)
      {
        return false;
      }

      if (!await ManagedUnsubscribeAsync(channelName, false, null, (Action<Notification>)entry.Item3))
      {
        return false;
      }

      lock (_subscriptions)
      {
        _subscriptions.Remove(entry);
      }

      return true;
    }

    private async Task<bool> PrivateSubscribeAsync<T>(string channelName, Action<T> originalCallback,
      Action<Notification> myCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, true, AccessToken, myCallback))
      {
        return false;
      }

      lock (_subscriptions)
      {
        _subscriptions.Add(Tuple.Create(channelName, (object)originalCallback, (object)myCallback));
      }

      return true;
    }

    private async Task<bool> UnsubscribePrivateAsync<T>(string channelName, Action<T> originalCallback)
    {
      Tuple<string, object, object> entry;
      lock (_subscriptions)
      {
        entry = _subscriptions.FirstOrDefault(x => x.Item1 == channelName && x.Item2 == (object)originalCallback);
      }

      if (entry == null)
      {
        return false;
      }

      if (!await ManagedUnsubscribeAsync(channelName, true, AccessToken, (Action<Notification>)entry.Item3))
      {
        return false;
      }

      lock (_subscriptions)
      {
        _subscriptions.Remove(entry);
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
      Logger.Debug("Authenticate");
      var scope = "connection";
      if (!string.IsNullOrEmpty(sessionName))
      {
        scope = $"session:{sessionName} expires:60";
      }

      var loginRes = await SendAsync("public/auth",
        new {grant_type = "client_credentials", client_id = accessKey, client_secret = accessSecret, scope},
        new ObjectJsonConverter<AuthResponse>());
      AccessToken = loginRes.access_token;
      RefreshToken = loginRes.refresh_token;
      _ = Task.Delay(TimeSpan.FromSeconds(loginRes.expires_in - 5)).ContinueWith(t =>
      {
        if (IsConnected)
        {
          _ = PublicAuthRefreshAsync(RefreshToken);
        }
      });
      return loginRes;
    }

    public async Task<AuthResponse> PublicAuthRefreshAsync(string refreshToken)
    {
      Logger.Debug("Refreshing Auth");
      var loginRes = await SendAsync("public/auth", new {grant_type = "refresh_token", refresh_token = refreshToken},
        new ObjectJsonConverter<AuthResponse>());
      AccessToken = loginRes.access_token;
      RefreshToken = loginRes.refresh_token;
      _ = Task.Delay(TimeSpan.FromSeconds(loginRes.expires_in - 5)).ContinueWith(t =>
      {
        if (IsConnected)
        {
          _ = PublicAuthRefreshAsync(RefreshToken);
        }
      });
      return loginRes;
    }

    public Task<object> PublicSetHeartbeatAsync(int intervalSeconds)
    {
      return SendAsync("public/set_heartbeat", new {interval = intervalSeconds}, new ObjectJsonConverter<object>());
    }

    public Task<object> PublicDisableHeartbeatAsync()
    {
      return SendAsync("public/disable_heartbeat", new { }, new ObjectJsonConverter<object>());
    }

    public Task<object> PrivateDisableCancelOnDisconnectAsync()
    {
      return SendAsync("private/disable_cancel_on_disconnect", new {access_token = AccessToken},
        new ObjectJsonConverter<object>());
    }

    public Task<AccountSummaryResponse> PrivateGetAccountSummaryAsync()
    {
      return SendAsync("private/get_account_summary",
        new {currency = "BTC", extended = true, access_token = AccessToken},
        new ObjectJsonConverter<AccountSummaryResponse>());
    }

    public Task<bool> PublicSubscribeBookAsync(string instrument, int group, int depth, Action<BookResponse> callback)
    {
      return PublicSubscribeAsync(
        "book." + instrument + "." + (group == 0 ? "none" : group.ToString()) + "." + depth + ".100ms", callback,
        response =>
        {
          callback(response.Data.ToObject<BookResponse>());
        });
    }

    public Task<bool> PrivateSubscribeOrdersAsync(string instrument, Action<OrderResponse> callback)
    {
      return PrivateSubscribeAsync("user.orders." + instrument + ".raw", callback, response =>
      {
        var orderResponse = response.Data.ToObject<OrderResponse>();
        orderResponse.timestamp = response.Timestamp;
        callback(orderResponse);
      });
    }

    public Task<bool> PrivateSubscribePortfolioAsync(string currency, Action<PortfolioResponse> callback)
    {
      return PrivateSubscribeAsync($"user.portfolio.{currency.ToLower()}", callback, response =>
      {
        callback(response.Data.ToObject<PortfolioResponse>());
      });
    }

    public Task<bool> PublicSubscribeTickerAsync(string instrument, string interval, Action<TickerResponse> callback)
    {
      return PublicSubscribeAsync($"ticker.{instrument}.{interval}", callback, response =>
      {
        callback(response.Data.ToObject<TickerResponse>());
      });
    }

    public Task<BookResponse> PublicGetOrderBookAsync(string instrument, int depth)
    {
      return SendAsync("public/get_order_book", new {instrument_name = instrument, depth},
        new ObjectJsonConverter<BookResponse>());
    }

    public Task<OrderItem[]> PrivateGetOpenOrdersAsync(string instrument)
    {
      return SendAsync("private/get_open_orders_by_instrument",
        new {instrument_name = instrument, access_token = AccessToken}, new ObjectJsonConverter<OrderItem[]>());
    }

    public Task<BuySellResponse> PrivateBuyLimitAsync(string instrument, double amount, double price, string label)
    {
      return SendAsync("private/buy", new
      {
        instrument_name = instrument,
        amount,
        type = "limit",
        label,
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
        label,
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
        var result = await SendAsync("private/get_order_state", new {order_id = orderId, access_token = AccessToken},
          new ObjectJsonConverter<OrderResponse>());
        return result;
      }
      catch
      {
        return null;
      }
    }

    public Task<object> PrivateCancelOrderAsync(string orderId)
    {
      return SendAsync("private/cancel", new {order_id = orderId, access_token = AccessToken},
        new ObjectJsonConverter<object>());
    }

    public Task<object> PrivateCancelAllOrdersByInstrumentAsync(string instrument)
    {
      return SendAsync("private/cancel_all_by_instrument",
        new {instrument_name = instrument, access_token = AccessToken}, new ObjectJsonConverter<object>());
    }

    public Task<SettlementResponse> PrivateGetSettlementHistoryByInstrumentAsync(string instrument, int count)
    {
      return SendAsync("private/get_settlement_history_by_instrument",
        new {instrument_name = instrument, type = "settlement", count, access_token = AccessToken},
        new ObjectJsonConverter<SettlementResponse>());
    }

    public Task<SettlementResponse> PrivateGetSettlementHistoryByCurrencyAsync(string currency, int count)
    {
      return SendAsync("private/get_settlement_history_by_currency",
        new {currency, type = "settlement", count, access_token = AccessToken},
        new ObjectJsonConverter<SettlementResponse>());
    }

    #endregion
  }
}
