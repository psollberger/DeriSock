// ReSharper disable UnusedMember.Local

namespace DeriSock
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.ComponentModel;
  using System.Diagnostics;
  using System.Threading.Tasks;
  using DeriSock.Converter;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using DeriSock.Response;
  using Newtonsoft.Json.Linq;
  using Serilog;
  using Serilog.Events;

  public class DeribitV2Client
  {
    private readonly IJsonRpcClient _client;
    private readonly SubscriptionManager _subscriptionManager;

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
      _subscriptionManager = new SubscriptionManager(this);
    }

    #region Market data

    public Task<JsonRpcResponse<BookResponse>> PublicGetOrderBookAsync(string instrument, int depth)
    {
      return SendAsync(
        "public/get_order_book",
        new {instrument_name = instrument, depth},
        new ObjectJsonConverter<BookResponse>());
    }

    #endregion

    private class SubscriptionManager
    {
      private readonly DeribitV2Client _client;
      private readonly ConcurrentDictionary<string, SubscriptionEntry> _subscriptionMap;

      public SubscriptionManager(DeribitV2Client client)
      {
        _client = client;
        _subscriptionMap = new ConcurrentDictionary<string, SubscriptionEntry>();
      }

      public async Task<bool> Subscribe(string channel, Action<Notification> callback)
      {
        if (callback == null)
        {
          return false;
        }

        var entryFound = _subscriptionMap.TryGetValue(channel, out var entry);

        if (entryFound && entry.State == SubscriptionState.Subscribing)
        {
          if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            _client.Logger.Verbose("Already subscribing: Wait for action completion ({Channel})", channel);
          }

          //TODO: entry.CurrentAction could be null due to threading (already completed?)
          var currentAction = entry.CurrentAction;
          var result = currentAction != null && await currentAction.ConfigureAwait(false);

          if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            _client.Logger.Verbose("Already subscribing: Action result: {Result} ({Channel})", result, channel);
          }

          if (!result || entry.State != SubscriptionState.Subscribed)
          {
            if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              _client.Logger.Verbose("Already subscribing: Action failed or subscription not successful ({Channel})", channel);
            }

            return false;
          }

          if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            _client.Logger.Verbose("Already subscribing: Adding callback ({Channel})", channel);
          }

          entry.Callbacks.Add(callback);
          return true;
        }

        if (entryFound && entry.State == SubscriptionState.Subscribed)
        {
          if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            _client.Logger.Verbose("Subscription for channel already exists. Adding callback to list ({Channel})", channel);
          }

          entry.Callbacks.Add(callback);
          return true;
        }

        if (entryFound && entry.State == SubscriptionState.Unsubscribing)
        {
          if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            _client.Logger.Verbose("Currently unsubscribing from Channel. Abort Subscribe ({Channel})", channel);
          }

          return false;
        }

        TaskCompletionSource<bool> defer = null;

        if (entryFound && entry.State == SubscriptionState.Unsubscribed)
        {
          if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            _client.Logger.Verbose("Unsubscribed from channel. Re-Subscribing ({Channel})", channel);
          }

          defer = new TaskCompletionSource<bool>();
          entry.State = SubscriptionState.Subscribing;
          entry.CurrentAction = defer.Task;
        }

        if (!entryFound)
        {
          if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
          {
            _client.Logger.Verbose("Subscription for channel not found. Subscribing ({Channel})", channel);
          }

          defer = new TaskCompletionSource<bool>();
          entry = new SubscriptionEntry {State = SubscriptionState.Subscribing, Callbacks = new List<Action<Notification>>(), CurrentAction = defer.Task};
          _subscriptionMap[channel] = entry;
        }

        try
        {
          var subscribeResponse = IsPrivateChannel(channel)
            ? await _client.SendAsync(
              "private/subscribe", new {channels = new[] {channel}, access_token = _client.AccessToken},
              new ListJsonConverter<string>()).ConfigureAwait(false)
            : await _client.SendAsync(
                "public/subscribe", new {channels = new[] {channel}},
                new ListJsonConverter<string>())
              .ConfigureAwait(false);

          //TODO: Handle possible error in response

          var response = subscribeResponse.ResultData;

          if (response.Count != 1 || response[0] != channel)
          {
            if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              _client.Logger.Verbose("Invalid subscribe result: {@Response} {Channel}", response, channel);
            }

            Debug.Assert(defer != null, nameof(defer) + " != null");
            defer.SetResult(false);
          }
          else
          {
            if (_client.Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
            {
              _client.Logger.Verbose("Successfully subscribed. Adding callback ({Channel})", channel);
            }

            entry.State = SubscriptionState.Subscribed;
            entry.Callbacks.Add(callback);
            entry.CurrentAction = null;
            Debug.Assert(defer != null, nameof(defer) + " != null");
            defer.SetResult(true);
          }
        }
        catch (Exception e)
        {
          Debug.Assert(defer != null, nameof(defer) + " != null");
          defer.SetException(e);
        }

        return await defer.Task;
      }

      public async Task<bool> Unsubscribe(string channel, Action<Notification> callback)
      {
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

            break;
          default:
            return false;
        }

        // At this point it's only possible that the entry-State is Subscribed
        // and the callback list is empty after removing this callback.
        // Hence we unsubscribe at the server now
        entry.State = SubscriptionState.Unsubscribing;
        var defer = new TaskCompletionSource<bool>();
        entry.CurrentAction = defer.Task;

        try
        {
          var unsubscribeResponse = IsPrivateChannel(channel)
            ? await _client.SendAsync(
              "private/unsubscribe", new {channels = new[] {channel}, access_token = _client.AccessToken},
              new ListJsonConverter<string>())
            : await _client.SendAsync(
              "public/unsubscribe", new {channels = new[] {channel}},
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

      public IEnumerable<Action<Notification>> GetCallbacks(string channel)
      {
        return !_subscriptionMap.TryGetValue(channel, out var entry) ? null : entry.Callbacks;
      }

      public void Reset()
      {
        _subscriptionMap.Clear();
      }

      private static bool IsPrivateChannel(string channel)
      {
        return channel.StartsWith("user.");
      }
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

    public async Task ConnectAsync()
    {
      await _client.ConnectAsync();
      _subscriptionManager.Reset();
    }

    public Task DisconnectAsync()
    {
      return _client.DisconnectAsync();
    }

    private async Task<JsonRpcResponse<T>> SendAsync<T>(string method, object @params, IJsonConverter<T> converter)
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
        PublicTest("ok");
      }
    }

    private void OnNotification(Notification notification)
    {
      if (Logger?.IsEnabled(LogEventLevel.Verbose) ?? false)
      {
        Logger.Verbose("OnNotification: {@Notification}", notification);
      }

      var callbacks = _subscriptionManager.GetCallbacks(notification.Channel);

      if (callbacks == null)
      {
        if (Logger?.IsEnabled(LogEventLevel.Warning) ?? false)
        {
          Logger.Warning(
            "OnNotification: Could not find subscription for notification: {@Notification}",
            notification);
        }

        return;
      }

      foreach (var cb in callbacks)
      {
        try
        {
          Task.Factory.StartNew(() => { cb(notification); });
        }
        catch (Exception ex)
        {
          if (Logger?.IsEnabled(LogEventLevel.Error) ?? false)
          {
            Logger?.Error(ex, "OnNotification: Error during event callback call: {@Notification}", notification);
          }
        }
      }
    }

    #endregion

    #region Authentication

    public async Task<JsonRpcResponse<AuthResponse>> PublicAuthAsync(string accessKey, string accessSecret, string sessionName)
    {
      Logger.Debug("Authenticate");

      var scope = "connection";
      if (!string.IsNullOrEmpty(sessionName))
      {
        scope = $"session:{sessionName} expires:60";
      }

      var response = await SendAsync(
        "public/auth", new {grant_type = "client_credentials", client_id = accessKey, client_secret = accessSecret, scope},
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
        "public/auth", new {grant_type = "refresh_token", refresh_token = refreshToken},
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

    #endregion

    #region Session management

    /// <summary>
    ///   <para>
    ///     Signals the Websocket connection to send and request heartbeats. Heartbeats can be used to detect stale
    ///     connections.
    ///     When heartbeats have been set up, the API server will send heartbeat messages and test_request messages.
    ///     Your software should respond to test_request messages by sending a <see cref="PublicTest" /> request.
    ///     If your software fails to do so, the API server will immediately close the connection.
    ///     If your account is configured to cancel on disconnect, any orders opened over the connection will be cancelled.
    ///   </para>
    ///   <para>The <see cref="DeribitV2Client" /> will automatically respond to heartbeats.</para>
    /// </summary>
    /// <param name="interval">The heartbeat interval in seconds, but not less than 10</param>
    public Task<JsonRpcResponse<string>> PublicSetHeartbeatAsync(int interval)
    {
      return SendAsync("public/set_heartbeat", new {interval}, new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   Stop sending heartbeat messages.
    /// </summary>
    public Task<JsonRpcResponse<string>> PublicDisableHeartbeatAsync()
    {
      return SendAsync("public/disable_heartbeat", null, new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   <para>
    ///     Enable Cancel On Disconnect for the connection.
    ///     After enabling Cancel On Disconnect all orders created by the connection will be removed when connection is closed.
    ///   </para>
    ///   <para>
    ///     NOTICE: It does not affect orders created by other connections - they will remain active!
    ///   </para>
    ///   <para>
    ///     When change is applied for the account, then every newly opened connection will start with active Cancel on
    ///     Disconnect
    ///   </para>
    /// </summary>
    public Task<JsonRpcResponse<string>> PrivateEnableCancelOnDisconnectAsync()
    {
      return PrivateEnableCancelOnDisconnectAsync("connection");
    }

    /// <summary>
    ///   <para>
    ///     Enable Cancel On Disconnect for the connection.
    ///     After enabling Cancel On Disconnect all orders created by the connection will be removed when connection is closed.
    ///   </para>
    ///   <para>
    ///     NOTICE: It does not affect orders created by other connections - they will remain active!
    ///   </para>
    ///   <para>
    ///     When change is applied for the account, then every newly opened connection will start with active Cancel on
    ///     Disconnect
    ///   </para>
    /// </summary>
    /// <param name="scope">Specifies if Cancel On Disconnect change should be applied/checked for the current connection or the account (default - <c>connection</c>)</param>
    public Task<JsonRpcResponse<string>> PrivateEnableCancelOnDisconnectAsync(string scope)
    {
      return SendAsync(
        "private/enable_cancel_on_disconnect",
        new {scope, access_token = AccessToken},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   <para>Disable Cancel On Disconnect for the connection.</para>
    ///   <para>
    ///     When change is applied for the account, then every newly opened connection will start with inactive Cancel on
    ///     Disconnect
    ///   </para>
    /// </summary>
    public Task<JsonRpcResponse<string>> PrivateDisableCancelOnDisconnectAsync()
    {
      return PrivateDisableCancelOnDisconnectAsync("connection");
    }

    /// <summary>
    ///   <para>Disable Cancel On Disconnect for the connection.</para>
    ///   <para>
    ///     When change is applied for the account, then every newly opened connection will start with inactive Cancel on
    ///     Disconnect
    ///   </para>
    /// </summary>
    /// <param name="scope">Specifies if Cancel On Disconnect change should be applied/checked for the current connection or the account (default - <c>connection</c>)</param>
    public Task<JsonRpcResponse<string>> PrivateDisableCancelOnDisconnectAsync(string scope)
    {
      return SendAsync(
        "private/disable_cancel_on_disconnect",
        new {scope, access_token = AccessToken},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    /// Read current Cancel On Disconnect configuration for the account
    /// </summary>
    /// <param name="scope">Specifies if Cancel On Disconnect change should be applied/checked for the current connection or the account (default - <c>connection</c>)</param>
    public Task<JsonRpcResponse<GetCancelOnDisconnectResponseData>> PrivateGetCancelOnDisconnectAsync()
    {
      return PrivateGetCancelOnDisconnectAsync("connection");
    }

    /// <summary>
    /// Read current Cancel On Disconnect configuration for the account
    /// </summary>
    /// <param name="scope">Specifies if Cancel On Disconnect change should be applied/checked for the current connection or the account (default - <c>connection</c>)</param>
    public Task<JsonRpcResponse<GetCancelOnDisconnectResponseData>> PrivateGetCancelOnDisconnectAsync(string scope)
    {
      return SendAsync(
        "private/get_cancel_on_disconnect",
        new {scope, access_token = AccessToken},
        new ObjectJsonConverter<GetCancelOnDisconnectResponseData>());
    }

    #endregion

    #region Supporting

    /// <summary>
    ///   Retrieves the current time (in milliseconds).
    ///   This API endpoint can be used to check the clock skew between your software and Deribit's systems.
    /// </summary>
    public Task<JsonRpcResponse<DateTime>> PublicGetTime()
    {
      return SendAsync("public/get_time", null, new TimestampJsonConverter());
    }

    /// <summary>
    ///   Method used to introduce the client software connected to Deribit platform over websocket.
    ///   Provided data may have an impact on the maintained connection and will be collected for internal statistical
    ///   purposes.
    ///   In response, Deribit will also introduce itself.
    /// </summary>
    /// <param name="clientName">Client software name</param>
    /// <param name="clientVersion">Client software version</param>
    public Task<JsonRpcResponse<HelloResponseData>> PublicHello(string clientName, string clientVersion)
    {
      return SendAsync(
        "public/hello",
        new {client_name = clientName, client_version = clientVersion},
        new ObjectJsonConverter<HelloResponseData>());
    }

    /// <summary>
    ///   Tests the connection to the API server, and returns its version.
    ///   You can use this to make sure the API is reachable, and matches the expected version.
    /// </summary>
    /// <param name="expectedResult">
    ///   The value "exception" will trigger an error response. This may be useful for testing
    ///   wrapper libraries.
    /// </param>
    public Task<JsonRpcResponse<TestResponseData>> PublicTest(string expectedResult)
    {
      return SendAsync(
        "public/test",
        new {expected_result = expectedResult},
        new ObjectJsonConverter<TestResponseData>());
    }

    #endregion

    #region Account management

    #endregion

    #region Block Trade

    #endregion

    #region Trading

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

    //edit

    public Task<JsonRpcResponse<JObject>> PrivateCancelOrderAsync(string orderId)
    {
      return SendAsync(
        "private/cancel",
        new {order_id = orderId, access_token = AccessToken},
        new ObjectJsonConverter<JObject>());
    }

    public Task<JsonRpcResponse<JObject>> PrivateCancelAllOrdersByInstrumentAsync(string instrument)
    {
      return SendAsync(
        "private/cancel_all_by_instrument",
        new {instrument_name = instrument, access_token = AccessToken},
        new ObjectJsonConverter<JObject>());
    }

    public Task<JsonRpcResponse<OrderItem[]>> PrivateGetOpenOrdersAsync(string instrument)
    {
      return SendAsync(
        "private/get_open_orders_by_instrument",
        new {instrument_name = instrument, access_token = AccessToken},
        new ObjectJsonConverter<OrderItem[]>());
    }

    public Task<JsonRpcResponse<OrderResponse>> PrivateGetOrderStateAsync(string orderId)
    {
      return SendAsync(
        "private/get_order_state",
        new {order_id = orderId, access_token = AccessToken},
        new ObjectJsonConverter<OrderResponse>());
    }

    public Task<JsonRpcResponse<AccountSummaryResponse>> PrivateGetAccountSummaryAsync(string currency, bool extended)
    {
      return SendAsync(
        "private/get_account_summary", new {currency, extended, access_token = AccessToken},
        new ObjectJsonConverter<AccountSummaryResponse>());
    }

    public Task<JsonRpcResponse<SettlementResponse>> PrivateGetSettlementHistoryByInstrumentAsync(string instrument, string type, int count)
    {
      return SendAsync(
        "private/get_settlement_history_by_instrument",
        new {instrument_name = instrument, type, count, access_token = AccessToken},
        new ObjectJsonConverter<SettlementResponse>());
    }

    public Task<JsonRpcResponse<SettlementResponse>> PrivateGetSettlementHistoryByCurrencyAsync(string currency, string type, int count)
    {
      return SendAsync(
        "private/get_settlement_history_by_currency",
        new {currency, type, count, access_token = AccessToken},
        new ObjectJsonConverter<SettlementResponse>());
    }

    #endregion

    #region Wallet

    #endregion

    #region Subscriptions

    public Task<bool> PublicSubscribeBookAsync(string instrument, int group, int depth, Action<BookResponse> callback)
    {
      var groupName = group == 0 ? "none" : group.ToString();
      return _subscriptionManager.Subscribe(
        $"book.{instrument}.{groupName}.{depth}.100ms",
        n =>
        {
          callback(n.Data.ToObject<BookResponse>());
        });
    }

    public Task<bool> PrivateSubscribeOrdersAsync(string instrument, Action<OrderResponse> callback)
    {
      return _subscriptionManager.Subscribe(
        "user.orders." + instrument + ".raw",
        n =>
        {
          var orderResponse = n.Data.ToObject<OrderResponse>();
          orderResponse.timestamp = n.Timestamp;
          callback(orderResponse);
        });
    }

    public Task<bool> PrivateSubscribePortfolioAsync(string currency, Action<PortfolioResponse> callback)
    {
      return _subscriptionManager.Subscribe(
        $"user.portfolio.{currency.ToLower()}",
        n =>
        {
          callback(n.Data.ToObject<PortfolioResponse>());
        });
    }

    public Task<bool> PublicSubscribeTickerAsync(string instrument, string interval, Action<TickerResponse> callback)
    {
      return _subscriptionManager.Subscribe(
        $"ticker.{instrument}.{interval}",
        n =>
        {
          callback(n.Data.ToObject<TickerResponse>());
        });
    }

    #endregion
  }
}
