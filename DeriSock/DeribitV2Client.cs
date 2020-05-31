// ReSharper disable UnusedMember.Local

namespace DeriSock
{
  using System;
  using System.Collections.Concurrent;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Threading.Tasks;
  using DeriSock.Converter;
  using DeriSock.Extensions;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using DeriSock.Request;
  using DeriSock.Response;
  using Newtonsoft.Json.Linq;
  using Serilog;
  using Serilog.Events;

  public class DeribitV2Client
  {
    private readonly IJsonRpcClient _client;
    private readonly SubscriptionManager _subscriptionManager;
    protected readonly ILogger Logger = Log.Logger;

    public string AccessToken { get; private set; }

    public string RefreshToken { get; private set; }

    public bool ClosedByError => _client?.ClosedByError ?? false;
    public bool ClosedByClient => _client?.ClosedByClient ?? false;
    public bool ClosedByHost => _client?.ClosedByHost ?? false;

    public bool IsConnected => (_client?.SocketAvailable ?? false) && !(ClosedByHost || ClosedByClient || ClosedByError);

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

    public async Task ConnectAsync()
    {
      await _client.ConnectAsync();
      AccessToken = null;
      RefreshToken = null;
      _subscriptionManager.Reset();
    }

    public async Task DisconnectAsync()
    {
      await _client.DisconnectAsync();
      AccessToken = null;
      RefreshToken = null;
    }

    private async Task<JsonRpcResponse<T>> SendAsync<T>(string method, object @params, IJsonConverter<T> converter)
    {
      var response = await _client.SendAsync(method, @params).ConfigureAwait(false);
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

    private void EnqueueAuthRefresh(int expiresIn)
    {
      var expireTimeSpan = TimeSpan.FromSeconds(expiresIn);
      if (expireTimeSpan.TotalMilliseconds > Int32.MaxValue)
      {
        return;
      }

      _ = Task.Delay(expireTimeSpan.Subtract(TimeSpan.FromSeconds(5))).ContinueWith(t =>
      {
        if (!IsConnected)
        {
          return;
        }

        var result = PublicAuthAsync(new AuthRequestParams { GrantType = "refresh_token" }).GetAwaiter().GetResult();
        EnqueueAuthRefresh(result.ResultData.ExpiresIn);
      });
    }

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
          entry = new SubscriptionEntry { State = SubscriptionState.Subscribing, Callbacks = new List<Action<Notification>>(), CurrentAction = defer.Task };
          _subscriptionMap[channel] = entry;
        }

        try
        {
          //TODO: check if private subscribe works without access_token being sent
          var subscribeResponse = IsPrivateChannel(channel)
            ? await _client.SendAsync(
              "private/subscribe", new { channels = new[] { channel }/*, access_token = _client.AccessToken*/ },
              new ListJsonConverter<string>()).ConfigureAwait(false)
            : await _client.SendAsync(
                "public/subscribe", new { channels = new[] { channel } },
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
          //TODO: check if private unsubscribe works without access_token being sent
          var unsubscribeResponse = IsPrivateChannel(channel)
            ? await _client.SendAsync(
              "private/unsubscribe", new { channels = new[] { channel }/*, access_token = _client.AccessToken*/},
              new ListJsonConverter<string>())
            : await _client.SendAsync(
              "public/unsubscribe", new { channels = new[] { channel } },
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

    #region API Calls

    #region Authentication

    /// <summary>
    ///   <para>Retrieve an Oauth access token, to be used for authentication of 'private' requests.</para>
    ///   <para>Three methods of authentication are supported:</para>
    ///   <para>
    ///     <list type="table">
    ///       <item>
    ///         <term>client_credentials</term>
    ///         <description>using the access key and access secret that can be found on the API page on the website</description>
    ///       </item>
    ///       <item>
    ///         <term>client_signature</term>
    ///         <description>using the access key that can be found on the API page on the website and user generated signature</description>
    ///       </item>
    ///       <item>
    ///         <term>refresh_token</term>
    ///         <description>using a refresh token that was received from an earlier invocation</description>
    ///       </item>
    ///     </list>
    ///   </para>
    ///   <para>
    ///     The response will contain an access token, expiration period (number of seconds that the token is valid) and a
    ///     refresh token that can be used to get a new set of tokens
    ///   </para>
    ///   <para>
    ///     NOTE: The necessary values for client_signature are automatically calculated.
    ///     Provide <see cref="AuthRequestParams.ClientId" /> and <see cref="AuthRequestParams.ClientSecret" />.
    ///     Optional: Provide <see cref="AuthRequestParams.Data" /> for more random data for signature calculation
    ///   </para>
    /// </summary>
    /// <param name="args"><see cref="AuthRequestParams" /> object containing the necessary values</param>
    public async Task<JsonRpcResponse<AuthResponseData>> PublicAuthAsync(AuthRequestParams args)
    {
      Logger.Debug("Authenticate ({GrantType})", args?.GrantType);

      if (!string.Equals(args.GrantType, "client_credentials") &&
          !string.Equals(args.GrantType, "client_signature") &&
          !string.Equals(args.GrantType, "refresh_token"))
      {
        throw new ArgumentException($"Unknown GrantType: {args.GrantType}");
      }

      if (!string.IsNullOrEmpty(AccessToken) && !string.Equals(args.GrantType, "refresh_token"))
      {
        throw new InvalidOperationException("Already authorized");
      }

      var state = string.IsNullOrEmpty(args.State) ? string.Empty : args.State;
      var scope = string.IsNullOrWhiteSpace(args.Scope) ? string.Empty : args.Scope;

      object reqParams = null;

      if (string.Equals(args.GrantType, "client_credentials"))
      {
        if (string.IsNullOrEmpty(args.ClientId))
        {
          throw new ArgumentNullException(nameof(args.ClientId));
        }

        if (string.IsNullOrEmpty(args.ClientSecret))
        {
          throw new ArgumentNullException(nameof(args.ClientSecret));
        }

        reqParams = new
        {
          grant_type = "client_credentials",
          client_id = args.ClientId,
          client_secret = args.ClientSecret,
          state,
          scope
        };
      }
      else if (string.Equals(args.GrantType, "client_signature"))
      {
        if (string.IsNullOrEmpty(args.ClientId))
        {
          throw new ArgumentNullException(nameof(args.ClientId));
        }

        if (args.Signature == null)
        {
          throw new ArgumentNullException(nameof(args.Signature));
        }

        reqParams = new
        {
          grant_type = args.GrantType,
          client_id = args.ClientId,
          timestamp = args.Signature.Timestamp,
          signature = args.Signature.Signature,
          nonce = args.Signature.Nonce,
          data = args.Signature.Data,
          state,
          scope
        };
      }
      else if (string.Equals(args.GrantType, "refresh_token"))
      {
        if (string.IsNullOrEmpty(RefreshToken))
        {
          throw new ArgumentNullException(nameof(RefreshToken));
        }

        reqParams = new { grant_type = "refresh_token", refresh_token = RefreshToken };
      }

      var response = await SendAsync("public/auth", reqParams, new ObjectJsonConverter<AuthResponseData>());

      var loginRes = response.ResultData;

      AccessToken = loginRes.AccessToken;
      RefreshToken = loginRes.RefreshToken;

      if (!string.Equals(args.GrantType, "refresh_token"))
      {
        EnqueueAuthRefresh(loginRes.ExpiresIn);
      }

      return response;
    }

    /// <summary>
    ///   Generates token for new subject id. This method can be used to switch between subaccounts.
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="subjectId">New subject id</param>
    public Task<JsonRpcResponse<ExchangeTokenResponseData>> PublicExchangeToken(string refreshToken, int subjectId)
    {
      return SendAsync(
        "public/exchange_token",
        new { refresh_token = refreshToken, subject_id = subjectId },
        new ObjectJsonConverter<ExchangeTokenResponseData>());
    }

    /// <summary>
    ///   Generates token for new named session. This method can be used only with session scoped tokens.
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="sessionName">New session name</param>
    public Task<JsonRpcResponse<ForkTokenResponseData>> PublicForkToken(string refreshToken, string sessionName)
    {
      return SendAsync(
        "public/fork_token",
        new { refresh_token = refreshToken, session_name = sessionName },
        new ObjectJsonConverter<ForkTokenResponseData>());
    }

    /// <summary>
    ///   Gracefully close websocket connection, when COD (Cancel On Disconnect) is enabled orders are not cancelled
    /// </summary>
    public bool PrivateLogout()
    {
      if (string.IsNullOrEmpty(AccessToken))
      {
        return false;
      }

      //TODO: check if logout works without access_token being sent
      //_client.SendLogout("private/logout", new {access_token = AccessToken});
      _client.SendLogout("private/logout", null);
      AccessToken = null;
      RefreshToken = null;
      return true;
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
      return SendAsync("public/set_heartbeat", new { interval }, new ObjectJsonConverter<string>());
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
    /// <param name="scope">
    ///   Specifies if Cancel On Disconnect change should be applied/checked for the current connection or
    ///   the account (default - <c>connection</c>)
    /// </param>
    public Task<JsonRpcResponse<string>> PrivateEnableCancelOnDisconnectAsync(string scope)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/enable_cancel_on_disconnect",
        new { scope/*, access_token = AccessToken*/},
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
    /// <param name="scope">
    ///   Specifies if Cancel On Disconnect change should be applied/checked for the current connection or
    ///   the account (default - <c>connection</c>)
    /// </param>
    public Task<JsonRpcResponse<string>> PrivateDisableCancelOnDisconnectAsync(string scope)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/disable_cancel_on_disconnect",
        new { scope/*, access_token = AccessToken*/},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   Read current Cancel On Disconnect configuration for the account
    /// </summary>
    /// <param name="scope">
    ///   Specifies if Cancel On Disconnect change should be applied/checked for the current connection or
    ///   the account (default - <c>connection</c>)
    /// </param>
    public Task<JsonRpcResponse<GetCancelOnDisconnectResponseData>> PrivateGetCancelOnDisconnectAsync()
    {
      return PrivateGetCancelOnDisconnectAsync("connection");
    }

    /// <summary>
    ///   Read current Cancel On Disconnect configuration for the account
    /// </summary>
    /// <param name="scope">
    ///   Specifies if Cancel On Disconnect change should be applied/checked for the current connection or
    ///   the account (default - <c>connection</c>)
    /// </param>
    public Task<JsonRpcResponse<GetCancelOnDisconnectResponseData>> PrivateGetCancelOnDisconnectAsync(string scope)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/get_cancel_on_disconnect",
        new { scope/*, access_token = AccessToken*/},
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
        new { client_name = clientName, client_version = clientVersion },
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
        new { expected_result = expectedResult },
        new ObjectJsonConverter<TestResponseData>());
    }

    #endregion

    #region Account management

    /// <summary>
    /// Retrieves user account summary
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="extended">Include additional fields</param>
    public Task<JsonRpcResponse<AccountSummaryResponseData>> PrivateGetAccountSummary(string currency, bool extended)
    {
      return SendAsync(
        "private/get_account_summary",
        new {currency, extended},
        new ObjectJsonConverter<AccountSummaryResponseData>());
    }

    /// <summary>
    /// Retrieves the language to be used for emails
    /// </summary>
    public Task<JsonRpcResponse<string>> PrivateGetEmailLanguage()
    {
      return SendAsync(
        "private/get_email_language",
        null,
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    /// Changes the language to be used for emails
    /// </summary>
    /// <param name="language">The abbreviated language name. Valid values include <c>en</c>, <c>ko</c>, <c>zh</c>, <c>ja</c>, <c>ru</c></param>
    public Task<JsonRpcResponse<string>> PrivateSetEmailLanguage(string language)
    {
      return SendAsync(
        "private/set_email_language",
        new {language},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    /// Retrieve user position
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    public Task<JsonRpcResponse<PositionResponseData>> PrivateGetPosition(string instrumentName)
    {
      return SendAsync(
        "private/get_position",
        new {instrument_name = instrumentName},
        new ObjectJsonConverter<PositionResponseData>());
    }

    /// <inheritdoc cref="PrivateGetPositions(string, string)"/>
    public Task<JsonRpcResponse<PositionResponseData[]>> PrivateGetPositions(string currency)
    {
      return SendAsync(
        "private/get_positions",
        new {currency},
        new ObjectJsonConverter<PositionResponseData[]>());
    }

    /// <summary>
    /// Retrieve user positions
    /// </summary>
    /// <param name="currency"><c>BTC</c> or <c>ETH</c></param>
    /// <param name="kind">Kind filter on positions: <c>future</c> or <c>option</c></param>
    public Task<JsonRpcResponse<PositionResponseData[]>> PrivateGetPositions(string currency, string kind)
    {
      return SendAsync(
        "private/get_positions",
        new {currency, kind},
        new ObjectJsonConverter<PositionResponseData[]>());
    }

    #region Announcements

    /// <inheritdoc cref="PublicGetAnnouncements(DateTime, int)" />
    public Task<JsonRpcResponse<AnnouncementResponseData[]>> PublicGetAnnouncements()
    {
      return SendAsync(
        "public/get_announcements",
        null,
        new ObjectJsonConverter<AnnouncementResponseData[]>());
    }

    /// <inheritdoc cref="PublicGetAnnouncements(DateTime, int)" />
    public Task<JsonRpcResponse<AnnouncementResponseData[]>> PublicGetAnnouncements(DateTime startTime)
    {
      return SendAsync(
        "public/get_announcements",
        new { start_timestamp = startTime.AsMilliseconds() },
        new ObjectJsonConverter<AnnouncementResponseData[]>());
    }

    /// <inheritdoc cref="PublicGetAnnouncements(DateTime, int)" />
    public Task<JsonRpcResponse<AnnouncementResponseData[]>> PublicGetAnnouncements(int count)
    {
      if (count < 1 || count > 50)
      {
        throw new ArgumentException("count must be between 1 and 50", nameof(count));
      }

      return SendAsync(
        "public/get_announcements",
        new { count },
        new ObjectJsonConverter<AnnouncementResponseData[]>());
    }

    /// <summary>
    ///   <para>Retrieves announcements.</para>
    ///   <para>Default <c>startTime</c> value is current timestamp. Default <c>count</c> value is 5</para>
    /// </summary>
    /// <param name="startTime">Timestamp from which we want to retrieve announcements</param>
    /// <param name="count">Maximum count of returned announcements. Must be between 1 and 50</param>
    public Task<JsonRpcResponse<AnnouncementResponseData[]>> PublicGetAnnouncements(DateTime startTime, int count)
    {
      if (count < 1 || count > 50)
      {
        throw new ArgumentException("count must be between 1 and 50", nameof(count));
      }

      return SendAsync(
        "public/get_announcements",
        new { start_timestamp = startTime.AsMilliseconds(), count },
        new ObjectJsonConverter<AnnouncementResponseData[]>());
    }

    /// <summary>
    /// Retrieves announcements that have not been marked read by the user
    /// </summary>
    public Task<JsonRpcResponse<AnnouncementResponseData[]>> PrivateGetNewAnnouncements()
    {
      return SendAsync(
        "private/get_new_announcements",
        null,
        new ObjectJsonConverter<AnnouncementResponseData[]>());
    }

    /// <summary>
    /// Marks an announcement as read, so it will not be shown in <see cref="PrivateGetNewAnnouncements"/>
    /// </summary>
    /// <param name="id">The ID of the announcement</param>
    public Task<JsonRpcResponse<string>> PrivateSetAnnouncementAsRead(long id)
    {
      return SendAsync(
        "private/set_announcement_as_read",
        new { announcement_id = id },
        new ObjectJsonConverter<string>());
    }

    #endregion

    #region Api Key

    //TODO: Check this
    /// <summary>
    /// Changes name for key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    /// <param name="name">Name of key (only letters, numbers and underscores allowed; maximum length is 16 characters)</param>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateChangeApiKeyName(int id, string name)
    {
      return SendAsync(
        "private/change_api_key_name",
        new { id, name },
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <summary>
    /// Changes scope for key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    /// <param name="maxScope"></param>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateChangeScopeInApiKey(int id, string maxScope)
    {
      return SendAsync(
        "private/change_scope_in_api_key",
        new { id, max_scope = maxScope },
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <inheritdoc cref="PrivateCreateApiKey(string, string, bool)"/>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateCreateApiKey(string maxScope)
    {
      return SendAsync(
        "private/create_api_key",
        new { max_scope = maxScope },
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <inheritdoc cref="PrivateCreateApiKey(string, string, bool)"/>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateCreateApiKey(string maxScope, string name)
    {
      return SendAsync(
        "private/create_api_key",
        new { name, max_scope = maxScope },
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <inheritdoc cref="PrivateCreateApiKey(string, string, bool)"/>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateCreateApiKey(string maxScope, bool asDefault)
    {
      return SendAsync(
        "private/create_api_key",
        new { @default = asDefault, max_scope = maxScope },
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <summary>
    /// Creates new api key with given scope
    /// </summary>
    /// <param name="maxScope"></param>
    /// <param name="name">Name of key (only letters, numbers and underscores allowed; maximum length is 16 characters)</param>
    /// <param name="asDefault">If <c>true</c>, new key is marked as default</param>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateCreateApiKey(string maxScope, string name, bool asDefault)
    {
      return SendAsync(
        "private/create_api_key",
        new { name, @default = asDefault, max_scope = maxScope },
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <summary>
    /// Disables api key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateDisableApiKey(int id)
    {
      return SendAsync(
        "private/disable_api_key",
        new {id},
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <summary>
    /// Enables api key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateEnableApiKey(int id)
    {
      return SendAsync(
        "private/enable_api_key",
        new {id},
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    /// <summary>
    /// Retrieves list of api keys
    /// </summary>
    public Task<JsonRpcResponse<ApiKeyResponseData[]>> PrivateListApiKeys()
    {
      return SendAsync(
        "private/list_api_keys",
        null,
        new ObjectJsonConverter<ApiKeyResponseData[]>());
    }

    /// <summary>
    /// Removes api key
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<string>> PrivateRemoveApiKey(int id)
    {
      return SendAsync(
        "private/remove_api_key",
        new {id},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    /// Resets secret in api key
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateResetApiKey(int id)
    {
      return SendAsync(
        "private/reset_api_key",
        new {id},
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    //TODO: Check this
    /// <summary>
    /// Sets key with given id as default one
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyResponseData>> PrivateSetApiKeyAsDefault(int id)
    {
      return SendAsync(
        "private/set_api_key_as_default",
        new {id},
        new ObjectJsonConverter<ApiKeyResponseData>());
    }

    #endregion

    #region Subaccount

    //TODO: Check this
    /// <summary>
    /// Change the user name for a subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="name">The new user name</param>
    public Task<JsonRpcResponse<string>> PrivateChangeSubaccountName(int sid, string name)
    {
      return SendAsync(
        "private/change_subaccount_name",
        new {sid, name},
        new ObjectJsonConverter<string>());
    }

    //TODO: check this
    /// <summary>
    /// Create a new subaccount
    /// </summary>
    public Task<JsonRpcResponse<SubaccountResponseData>> PrivateCreateSubaccount()
    {
      return SendAsync(
        "private/create_subaccount",
        null,
        new ObjectJsonConverter<SubaccountResponseData>());
    }

    /// <summary>
    /// Disable two factor authentication for a subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    public Task<JsonRpcResponse<string>> PrivateDisableTfaForSubaccount(int sid)
    {
      return SendAsync(
        "private/disable_tfa_for_subaccount",
        new {sid},
        new ObjectJsonConverter<string>());
    }

    //TODO: check this
    /// <inheritdoc cref="PrivateGetSubaccounts(bool)"/>
    public Task<JsonRpcResponse<SubaccountResponseData[]>> PrivateGetSubaccounts()
    {
      return SendAsync(
        "private/get_subaccounts",
        null,
        new ObjectJsonConverter<SubaccountResponseData[]>());
    }

    //TODO: check this
    /// <summary>
    /// Get information about subaccounts
    /// </summary>
    public Task<JsonRpcResponse<SubaccountResponseData[]>> PrivateGetSubaccounts(bool withPortfolio)
    {
      return SendAsync(
        "private/get_subaccounts",
        new {with_portfolio = withPortfolio},
        new ObjectJsonConverter<SubaccountResponseData[]>());
    }

    //TODO: Check this
    /// <summary>
    /// Assign an email address to a subaccount. User will receive an email with confirmation link.
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="email">The email address for the subaccount</param>
    public Task<JsonRpcResponse<string>> PrivateSetEmailForSubaccount(int sid, string email)
    {
      return SendAsync(
        "private/set_email_for_subaccount",
        new {sid, email},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    /// Set the password for the subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="password">The password address for the subaccount</param>
    public Task<JsonRpcResponse<string>> PrivateSetPasswordForSubaccount(int sid, string password)
    {
      return SendAsync(
        "private/set_password_for_subaccount",
        new {sid, password},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    /// Enable or disable sending of notifications for the subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="state">enable (<c>true</c>) or disable (<c>false</c>) notifications</param>
    public Task<JsonRpcResponse<string>> PrivateToggleNotificationsFromSubaccount(int sid, bool state)
    {
      return SendAsync(
        "private/toggle_notifications_from_subaccount",
        new {sid, state},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    /// Enable or disable login for a subaccount. If login is disabled and a session for the subaccount exists, this session will be terminated
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="state">enable (<c>true</c>) or disable (<c>false</c>) login</param>
    public Task<JsonRpcResponse<string>> PrivateToggleSubaccountLogin(int sid, bool state)
    {
      return SendAsync(
        "private/toggle_subaccount_login",
        new {sid, state = state ? "enable" : "disable"},
        new ObjectJsonConverter<string>());
    }

    #endregion

    #endregion

    //TODO: Finish this

    #region Block Trade

    #endregion

    //TODO: Finish this

    #region Trading

    public Task<JsonRpcResponse<BuySellResponse>> PrivateBuyLimitAsync(string instrument, double amount, double price, string label)
    {
      //TODO: check if private method works without access_token being sent
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
          post_only = true/*, access_token = AccessToken*/
        }, new ObjectJsonConverter<BuySellResponse>());
    }

    public Task<JsonRpcResponse<BuySellResponse>> PrivateSellLimitAsync(string instrument, double amount, double price, string label)
    {
      //TODO: check if private method works without access_token being sent
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
          post_only = true/*, access_token = AccessToken*/
        }, new ObjectJsonConverter<BuySellResponse>());
    }

    //edit

    public Task<JsonRpcResponse<JObject>> PrivateCancelOrderAsync(string orderId)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/cancel",
        new { order_id = orderId/*, access_token = AccessToken*/},
        new ObjectJsonConverter<JObject>());
    }

    public Task<JsonRpcResponse<JObject>> PrivateCancelAllOrdersByInstrumentAsync(string instrument)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/cancel_all_by_instrument",
        new { instrument_name = instrument/*, access_token = AccessToken*/},
        new ObjectJsonConverter<JObject>());
    }

    public Task<JsonRpcResponse<OrderItem[]>> PrivateGetOpenOrdersAsync(string instrument)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/get_open_orders_by_instrument",
        new { instrument_name = instrument/*, access_token = AccessToken*/},
        new ObjectJsonConverter<OrderItem[]>());
    }

    public Task<JsonRpcResponse<OrderResponse>> PrivateGetOrderStateAsync(string orderId)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/get_order_state",
        new { order_id = orderId/*, access_token = AccessToken*/},
        new ObjectJsonConverter<OrderResponse>());
    }
    
    public Task<JsonRpcResponse<SettlementResponse>> PrivateGetSettlementHistoryByInstrumentAsync(string instrument, string type, int count)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/get_settlement_history_by_instrument",
        new { instrument_name = instrument, type, count/*, access_token = AccessToken*/},
        new ObjectJsonConverter<SettlementResponse>());
    }

    public Task<JsonRpcResponse<SettlementResponse>> PrivateGetSettlementHistoryByCurrencyAsync(string currency, string type, int count)
    {
      //TODO: check if private method works without access_token being sent
      return SendAsync(
        "private/get_settlement_history_by_currency",
        new { currency, type, count/*, access_token = AccessToken*/},
        new ObjectJsonConverter<SettlementResponse>());
    }

    #endregion

    //TODO: Finish this

    #region Market data

    public Task<JsonRpcResponse<BookResponse>> PublicGetOrderBookAsync(string instrument, int depth)
    {
      return SendAsync(
        "public/get_order_book",
        new { instrument_name = instrument, depth },
        new ObjectJsonConverter<BookResponse>());
    }

    #endregion

    //TODO: Finish this

    #region Wallet

    #endregion

    //TODO: Finish this

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

    #endregion
  }
}
