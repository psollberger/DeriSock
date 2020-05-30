// ReSharper disable UnusedMember.Local

namespace DeriSock
{
  using System;
  using System.Collections.Generic;
  using System.Linq;
  using System.Threading.Tasks;
  using DeriSock.Converter;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using DeriSock.Utils;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;
  using Serilog;
  using Serilog.Events;
  using SubscriptionMap = System.Collections.Concurrent.ConcurrentDictionary<string, Utils.SubscriptionEntry>;
  using SubscriptionListEntry = System.Tuple<string, object, object>;
  using SubscriptionList = System.Collections.Generic.List<System.Tuple<string, object, object>>;

  public class DeribitV2Client
  {
    private readonly IJsonRpcClient _client;
    private readonly SubscriptionMap _subscriptionMap = new SubscriptionMap();
    private readonly SubscriptionList _subscriptions = new SubscriptionList();

    protected readonly ILogger Logger = Log.Logger;

    public DeribitV2Client(DeribitEndpointType endpointType)
    {
      switch (endpointType)
      {
        case DeribitEndpointType.Productive:
          _client = JsonRpcClientFactory.Create(new Uri("wss://www.deribit.com/ws/api/v2"));
          break;
        case DeribitEndpointType.Testnet:
          _client = JsonRpcClientFactory.Create(new Uri("wss://test.deribit.com/ws/api/v2"));
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(endpointType), endpointType, "Unsupported endpoint type");
      }

      _client.Request += OnServerRequest;
    }

    #region Properties

    public string AccessToken { get; private set; }

    public string RefreshToken { get; private set; }

    public bool ClosedByError => _client?.ClosedByError ?? false;
    public bool ClosedByClient => _client?.ClosedByClient ?? false;
    public bool ClosedByHost => _client?.ClosedByHost ?? false;

    public bool IsConnected => (_client?.SocketAvailable ?? false) && !(ClosedByHost || ClosedByClient || ClosedByError);

    #endregion

    #region Connection Handling

    public Task ConnectAsync()
    {
      lock (_subscriptions)
      {
        _subscriptions.Clear();
      }

      _subscriptionMap.Clear();
      return _client.ConnectAsync();
    }

    public Task DisconnectAsync()
    {
      return _client.DisconnectAsync();
    }

    private async Task<JsonRpcResponse<T>> SendAsync<T>(string method, object @params, IJsonConverter<T> converter) where T : class
    {
      var response = await _client.SendAsync(method, @params).ConfigureAwait(false);
      if (response.Error != null)
      {
        throw new ResponseErrorException(response, response.Error.Message);
      }
      return response.CreateTyped(converter.Convert(response.Result));
    }

    private void OnServerRequest(object sender, JsonRpcRequest request)
    {
      if (string.Equals(request.Method, "subscription"))
      {
        OnNotification(request.Original.ToObject<Notification>());
      }
      else if (string.Equals(request.Method, "heartbeat"))
      {
        OnHeartbeat(request.Original.ToObject<Heartbeat>());
      }
      else
      {
        Logger.Warning("Unknown Server Request: {@Request}", request);
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
        _ = SendAsync("public/test", new { expected_result = "ok" }, new ObjectJsonConverter<TestResponse>());
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
            Logger.Warning("OnNotification: Could not find subscription for notification: {@Notification}",
              notification);
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

    #endregion

    #region Subscriptions

    private async Task<bool> ManagedSubscribeAsync(string channel, bool @private, string accessToken, Action<Notification> callback)
    {
      if (callback == null)
      {
        return false;
      }

      TaskCompletionSource<bool> defer = null;
      if (_subscriptionMap.TryGetValue(channel, out var entry))
      {
        switch (entry.State)
        {
          case SubscriptionState.Subscribed:
            {
              if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
              {
                Logger.Verbose("Subscription for channel already exists. Adding callback to list ({Channel})", channel);
              }

              entry.Callbacks.Add(callback);
              return true;
            }

          case SubscriptionState.Unsubscribing:
            if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              Logger.Verbose("Unsubscribing from Channel. Abort Subscribe ({Channel})", channel);
            }

            return false;

          case SubscriptionState.Unsubscribed:
            if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              Logger.Verbose("Unsubscribed from channel. Re-Subscribing ({Channel})", channel);
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
          Logger.Verbose("Subscription for channel not found. Subscribing ({Channel})", channel);
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

      if (defer == null)
      {
        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose("Empty defer: Wait for action completion ({Channel})", channel);
        }

        var currentAction = entry.CurrentAction;
        var result = currentAction != null && await currentAction.ConfigureAwait(false);
        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose("Empty defer: Action result: {Result} {Channel}", result, channel);
        }

        if (!result || entry.State != SubscriptionState.Subscribed)
        {
          return false;
        }

        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose("Empty defer: Adding callback ({Channel})", channel);
        }

        entry.Callbacks.Add(callback);
        return true;
      }

      try
      {
        if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
        {
          Logger.Verbose("Subscribing to {Channel}", channel);
        }

        var subscribeResponse = !@private
          ? await SendAsync(
              "public/subscribe", new { channels = new[] { channel } },
              new ListJsonConverter<string>())
            .ConfigureAwait(false)
          : await SendAsync(
            "private/subscribe", new { channels = new[] { channel }, access_token = accessToken },
            new ListJsonConverter<string>()).ConfigureAwait(false);

        //TODO: Handle possible error in response

        var response = subscribeResponse.ResultData;

        if (response.Count != 1 || response[0] != channel)
        {
          if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            Logger.Verbose("Invalid subscribe result: {@Response} {Channel}", response, channel);
          }

          defer.SetResult(false);
        }
        else
        {
          if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            Logger.Verbose("Successfully subscribed. Adding callback ({Channel})", channel);
          }

          entry.State = SubscriptionState.Subscribed;
          entry.Callbacks.Add(callback);
          entry.CurrentAction = null;
          defer.SetResult(true);
        }
      }
      catch (Exception e)
      {
        defer.SetException(e);
      }

      return await defer.Task;
    }

    private async Task<bool> ManagedUnsubscribeAsync(string channel, bool @private, string accessToken, Action<Notification> callback)
    {
      TaskCompletionSource<bool> defer;

      if (!_subscriptionMap.TryGetValue(channel, out var entry))
      {
        return false;
      }

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

      try
      {
        var unsubscribeResponse = !@private
          ? await SendAsync(
            "public/unsubscribe", new { channels = new[] { channel } },
            new ListJsonConverter<string>())
          : await SendAsync(
            "private/unsubscribe", new { channels = new[] { channel }, access_token = accessToken },
            new ListJsonConverter<string>());

        //TODO: Handle possible error in response

        var response = unsubscribeResponse.ResultData;

        if (response.Count != 1 || response[0] != channel)
        {
          defer.SetResult(false);
        }
        else
        {
          entry.State = SubscriptionState.Unsubscribed;
          entry.Callbacks.Remove(callback);
          entry.CurrentAction = null;
          defer.SetResult(true);
        }
      }
      catch (Exception e)
      {
        defer.SetException(e);
      }

      return await defer.Task;
    }

    private async Task<bool> PublicSubscribeAsync<T>(string channelName, Action<T> userCallback, Action<Notification> apiCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, false, null, apiCallback))
      {
        return false;
      }

      lock (_subscriptions)
      {
        _subscriptions.Add(Tuple.Create(channelName, (object)userCallback, (object)apiCallback));
      }

      return true;
    }

    private async Task<bool> UnsubscribePublicAsync<T>(string channelName, Action<T> userCallback)
    {
      if (!TryGetSubscriptionEntry(channelName, userCallback, out var entry))
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

    private async Task<bool> PrivateSubscribeAsync<T>(string channelName, Action<T> userCallback, Action<Notification> apiCallback)
    {
      if (!await ManagedSubscribeAsync(channelName, true, AccessToken, apiCallback))
      {
        return false;
      }

      lock (_subscriptions)
      {
        _subscriptions.Add(Tuple.Create(channelName, (object)userCallback, (object)apiCallback));
      }

      return true;
    }

    private async Task<bool> UnsubscribePrivateAsync<T>(string channelName, Action<T> userCallback)
    {
      if (!TryGetSubscriptionEntry(channelName, userCallback, out var entry))
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

    private bool TryGetSubscriptionEntry<T>(string channelName, Action<T> userCallback, out SubscriptionListEntry entry)
    {
      lock (_subscriptions)
      {
        entry = _subscriptions.FirstOrDefault(x => x.Item1 == channelName && x.Item2 == (object)userCallback);
      }

      return entry != null;
    }

    #endregion

    #region API v2 Functionality

    public Task<JsonRpcResponse<string>> PingAsync()
    {
      return SendAsync("public/ping", new { }, new ObjectJsonConverter<string>());
    }

    public async Task<JsonRpcResponse<AuthResponse>> PublicAuthAsync(string accessKey, string accessSecret, string sessionName)
    {
      Logger.Debug("Authenticate");

      var scope = "connection";
      if (!string.IsNullOrEmpty(sessionName))
      {
        scope = $"session:{sessionName} expires:60";
      }

      var response = await SendAsync(
        "public/auth", new { grant_type = "client_credentials", client_id = accessKey, client_secret = accessSecret, scope },
        new ObjectJsonConverter<AuthResponse>());

      //TODO: Handle possible error in response

      var loginRes = response.ResultData;

      AccessToken = loginRes.access_token;
      RefreshToken = loginRes.refresh_token;
      _ = Task.Delay(TimeSpan.FromSeconds(loginRes.expires_in - 5)).ContinueWith(t =>
      {
        if (IsConnected)
        {
          _ = PublicAuthRefreshAsync(RefreshToken);
        }
      });

      return response;
    }

    public async Task<JsonRpcResponse<AuthResponse>> PublicAuthRefreshAsync(string refreshToken)
    {
      Logger.Debug("Refreshing Auth");

      var response = await SendAsync(
        "public/auth", new { grant_type = "refresh_token", refresh_token = refreshToken },
        new ObjectJsonConverter<AuthResponse>());

      //TODO: Handle possible error in response

      var loginRes = response.ResultData;

      AccessToken = loginRes.access_token;
      RefreshToken = loginRes.refresh_token;
      _ = Task.Delay(TimeSpan.FromSeconds(loginRes.expires_in - 5)).ContinueWith(t =>
      {
        if (IsConnected)
        {
          _ = PublicAuthRefreshAsync(RefreshToken);
        }
      });

      return response;
    }

    public Task<JsonRpcResponse<string>> PublicSetHeartbeatAsync(int intervalSeconds)
    {
      return SendAsync("public/set_heartbeat", new { interval = intervalSeconds }, new ObjectJsonConverter<string>());
    }

    public Task<JsonRpcResponse<string>> PublicDisableHeartbeatAsync()
    {
      return SendAsync("public/disable_heartbeat", new { }, new ObjectJsonConverter<string>());
    }

    public Task<JsonRpcResponse<string>> PrivateDisableCancelOnDisconnectAsync()
    {
      return SendAsync(
        "private/disable_cancel_on_disconnect", new { access_token = AccessToken },
        new ObjectJsonConverter<string>());
    }

    public Task<JsonRpcResponse<AccountSummaryResponse>> PrivateGetAccountSummaryAsync(string currency, bool extended)
    {
      return SendAsync(
        "private/get_account_summary", new { currency, extended, access_token = AccessToken },
        new ObjectJsonConverter<AccountSummaryResponse>());
    }

    public Task<bool> PublicSubscribeBookAsync(string instrument, int group, int depth, Action<BookResponse> callback)
    {
      return PublicSubscribeAsync(
        "book." + instrument + "." + (group == 0 ? "none" : group.ToString()) + "." + depth + ".100ms",
        callback,
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

    public Task<JsonRpcResponse<BookResponse>> PublicGetOrderBookAsync(string instrument, int depth)
    {
      return SendAsync(
        "public/get_order_book",
        new { instrument_name = instrument, depth },
        new ObjectJsonConverter<BookResponse>());
    }

    public Task<JsonRpcResponse<OrderItem[]>> PrivateGetOpenOrdersAsync(string instrument)
    {
      return SendAsync(
        "private/get_open_orders_by_instrument",
        new { instrument_name = instrument, access_token = AccessToken },
        new ObjectJsonConverter<OrderItem[]>());
    }

    public Task<JsonRpcResponse<BuySellResponse>> PrivateBuyLimitAsync(string instrument, double amount, double price, string label)
    {
      return SendAsync(
        "private/buy",
        new
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

    public Task<JsonRpcResponse<BuySellResponse>> PrivateSellLimitAsync(string instrument, double amount, double price, string label)
    {
      return SendAsync(
        "private/sell",
        new
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

    public Task<JsonRpcResponse<OrderResponse>> PrivateGetOrderStateAsync(string orderId)
    {
      return SendAsync(
        "private/get_order_state",
        new { order_id = orderId, access_token = AccessToken },
        new ObjectJsonConverter<OrderResponse>());
    }

    public Task<JsonRpcResponse<JObject>> PrivateCancelOrderAsync(string orderId)
    {
      return SendAsync(
        "private/cancel",
        new { order_id = orderId, access_token = AccessToken },
        new ObjectJsonConverter<JObject>());
    }

    public Task<JsonRpcResponse<JObject>> PrivateCancelAllOrdersByInstrumentAsync(string instrument)
    {
      return SendAsync(
        "private/cancel_all_by_instrument",
        new { instrument_name = instrument, access_token = AccessToken },
        new ObjectJsonConverter<JObject>());
    }

    public Task<JsonRpcResponse<SettlementResponse>> PrivateGetSettlementHistoryByInstrumentAsync(string instrument, string type, int count)
    {
      return SendAsync(
        "private/get_settlement_history_by_instrument",
        new { instrument_name = instrument, type, count, access_token = AccessToken },
        new ObjectJsonConverter<SettlementResponse>());
    }

    public Task<JsonRpcResponse<SettlementResponse>> PrivateGetSettlementHistoryByCurrencyAsync(string currency, string type, int count)
    {
      return SendAsync(
        "private/get_settlement_history_by_currency",
        new { currency, type, count, access_token = AccessToken },
        new ObjectJsonConverter<SettlementResponse>());
    }

    #endregion
  }
}
