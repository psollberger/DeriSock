// ReSharper disable UnusedMember.Local

namespace DeriSock
{
  using System;
  using System.Collections.Generic;
  using System.Diagnostics;
  using System.Dynamic;
  using System.Net.WebSockets;
  using System.Threading.Tasks;
  using DeriSock.Converter;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using DeriSock.Request;
  using Serilog;
  using Serilog.Events;

  /// <summary>
  ///   <para>The implementation of the API methods from Deribit</para>
  ///   <para>All methods are asynchronous. Synchronous methods are suffixed with <c>Sync</c></para>
  /// </summary>
  public class DeribitV2Client
  {
    public event EventHandler Connected;
    public event EventHandler<JsonRpcDisconnectEventArgs> Disconnected;
    private readonly IJsonRpcClient _client;
    private readonly SubscriptionManager _subscriptionManager;
    protected readonly ILogger Logger;

    public string AccessToken { get; private set; }

    public string RefreshToken { get; private set; }

    public WebSocketState State => _client.State;
    public WebSocketCloseStatus? CloseStatus => _client.CloseStatus;
    public string CloseStatusDescription => _client.CloseStatusDescription;
    public Exception Error => _client.Error;

    public DeribitV2Client(DeribitEndpointType endpointType, ILogger logger = null)
    {
      Logger = logger ?? Log.Logger;

      switch (endpointType)
      {
        case DeribitEndpointType.Productive:
          _client = JsonRpcClientFactory.Create(new Uri("wss://www.deribit.com/ws/api/v2"), Logger);
          break;
        case DeribitEndpointType.Testnet:
          _client = JsonRpcClientFactory.Create(new Uri("wss://test.deribit.com/ws/api/v2"), Logger);
          break;
        default:
          throw new ArgumentOutOfRangeException(nameof(endpointType), endpointType, "Unsupported endpoint type");
      }

      _client.Connected += OnServerConnected;
      _client.Disconnected += OnServerDisconnected;
      _client.RequestReceived += OnServerRequest;
      _subscriptionManager = new SubscriptionManager(this);
    }

    public async Task Connect()
    {
      await _client.Connect();
      AccessToken = null;
      RefreshToken = null;
      _subscriptionManager.Reset();
    }

    public async Task Disconnect()
    {
      await _client.Disconnect();
      AccessToken = null;
      RefreshToken = null;
    }

    protected virtual async Task<JsonRpcResponse<T>> Send<T>(string method, object @params, IJsonConverter<T> converter)
    {
      var response = await _client.Send(method, @params).ConfigureAwait(false);
      return response.CreateTyped(converter.Convert(response.Result));
    }

    protected virtual void OnServerConnected(object sender, EventArgs e)
    {
      Connected?.Invoke(this, e);
    }

    protected virtual void OnServerDisconnected(object sender, JsonRpcDisconnectEventArgs e)
    {
      Disconnected?.Invoke(this, e);
    }

    protected virtual void OnServerRequest(object sender, JsonRpcRequest request)
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

    protected virtual void OnHeartbeat(Heartbeat heartbeat)
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

    protected virtual void OnNotification(Notification notification)
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
          Logger.Warning("OnNotification: Could not find subscription for notification: {@Notification}", notification);
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

    protected virtual void EnqueueAuthRefresh(int expiresIn)
    {
      var expireTimeSpan = TimeSpan.FromSeconds(expiresIn);
      if (expireTimeSpan.TotalMilliseconds > Int32.MaxValue)
      {
        return;
      }

      _ = Task.Delay(expireTimeSpan.Subtract(TimeSpan.FromSeconds(5))).ContinueWith(t =>
      {
        if (_client.State != WebSocketState.Open)
        {
          return;
        }

        var result = PublicAuth(new AuthParams {GrantType = "refresh_token"}).GetAwaiter().GetResult();
        EnqueueAuthRefresh(result.ResultData.ExpiresIn);
      });
    }

    protected class SubscriptionManager
    {
      private readonly DeribitV2Client _client;
      private readonly SortedDictionary<string, SubscriptionEntry> _subscriptionMap;

      public SubscriptionManager(DeribitV2Client client)
      {
        _client = client;
        _subscriptionMap = new SortedDictionary<string, SubscriptionEntry>();
      }

      public async Task<SubscriptionToken> Subscribe(ISubscriptionChannel channel, Action<Notification> callback)
      {
        if (callback == null)
        {
          return SubscriptionToken.Invalid;
        }

        var channelName = channel.ToChannelName();
        TaskCompletionSource<SubscriptionToken> taskSource = null;
        SubscriptionEntry entry;

        lock (_subscriptionMap)
        {
          if (!_subscriptionMap.TryGetValue(channelName, out entry))
          {
            entry = new SubscriptionEntry();
            if (!_subscriptionMap.TryAdd(channelName, entry))
            {
              _client.Logger?.Error("Subscribe: Could not add internal item for channel {Channel}", channelName);
              return SubscriptionToken.Invalid;
            }

            taskSource = new TaskCompletionSource<SubscriptionToken>();
            entry.State = SubscriptionState.Subscribing;
            entry.SubscribeTask = taskSource.Task;
          }

          // Entry already exists but is completely unsubscribed
          if (entry.State == SubscriptionState.Unsubscribed)
          {
            taskSource = new TaskCompletionSource<SubscriptionToken>();
            entry.State = SubscriptionState.Subscribing;
            entry.SubscribeTask = taskSource.Task;
          }

          // Already subscribed - Put the callback in there and let's go
          if (entry.State == SubscriptionState.Subscribed)
          {
            _client.Logger?.Debug("Subscribe: Subscription for channel already exists. Adding callback to list (Channel: {Channel})", channelName);
            var callbackEntry = new SubscriptionCallback(new SubscriptionToken(Guid.NewGuid()), callback);
            entry.Callbacks.Add(callbackEntry);
            return callbackEntry.Token;
          }

          // We are in the middle of unsubscribing from the channel
          if (entry.State == SubscriptionState.Unsubscribing)
          {
            _client.Logger?.Debug("Subscribe: Channel is unsubscribing. Abort subscribe (Channel: {Channel})", channelName);
            return SubscriptionToken.Invalid;
          }
        }

        // Only one state left: Subscribing

        // We are already subscribing
        if (taskSource == null && entry.State == SubscriptionState.Subscribing)
        {
          _client.Logger?.Debug("Subscribe: Channel is already subscribing. Waiting for the task to complete ({Channel})", channelName);

          var subscribeResult = entry.SubscribeTask != null && await entry.SubscribeTask != SubscriptionToken.Invalid;

          if (!subscribeResult && entry.State != SubscriptionState.Subscribed)
          {
            _client.Logger?.Debug("Subscribe: Subscription has failed. Abort subscribe (Channel: {Channel})", channelName);
            return SubscriptionToken.Invalid;
          }

          _client.Logger?.Debug("Subscribe: Subscription was successful. Adding callback (Channel: {Channel}", channelName);
          var callbackEntry = new SubscriptionCallback(new SubscriptionToken(Guid.NewGuid()), callback);
          entry.Callbacks.Add(callbackEntry);
          return callbackEntry.Token;
        }

        if (taskSource == null)
        {
          _client.Logger?.Error("Subscribe: Invalid execution state. Missing TaskCompletionSource (Channel: {Channel}", channelName);
          return SubscriptionToken.Invalid;
        }

        try
        {
          var subscribeResponse = await _client.Send(
            IsPrivateChannel(channelName) ? "private/subscribe" : "public/subscribe",
            new {channels = new[] {channelName}},
            new ListJsonConverter<string>()).ConfigureAwait(false);

          var response = subscribeResponse.ResultData;

          if (response.Count != 1 || response[0] != channelName)
          {
            _client.Logger?.Debug("Subscribe: Invalid result (Channel: {Channel}): {@Response}", channelName, response);
            entry.State = SubscriptionState.Unsubscribed;
            entry.SubscribeTask = null;
            Debug.Assert(taskSource != null, nameof(taskSource) + " != null");
            taskSource.SetResult(SubscriptionToken.Invalid);
          }
          else
          {
            _client.Logger?.Debug("Subscribe: Successfully subscribed. Adding callback (Channel: {Channel})", channelName);

            var callbackEntry = new SubscriptionCallback(new SubscriptionToken(Guid.NewGuid()), callback);
            entry.Callbacks.Add(callbackEntry);
            entry.State = SubscriptionState.Subscribed;
            entry.SubscribeTask = null;
            Debug.Assert(taskSource != null, nameof(taskSource) + " != null");
            taskSource.SetResult(callbackEntry.Token);
          }
        }
        catch (Exception e)
        {
          entry.State = SubscriptionState.Unsubscribed;
          entry.SubscribeTask = null;
          Debug.Assert(taskSource != null, nameof(taskSource) + " != null");
          taskSource.SetException(e);
        }

        return await taskSource.Task;
      }

      public async Task<bool> Unsubscribe(SubscriptionToken token)
      {
        string channelName;
        SubscriptionEntry entry;
        SubscriptionCallback callbackEntry;
        TaskCompletionSource<bool> taskSource;

        lock (_subscriptionMap)
        {
          (channelName, entry, callbackEntry) = GetEntryByToken(token);

          if (string.IsNullOrEmpty(channelName) || entry == null || callbackEntry == null)
          {
            _client.Logger?.Warning("Unsubscribe: Could not find token {token}", token.Token);
            return false;
          }

          switch (entry.State)
          {
            case SubscriptionState.Subscribing:
              _client.Logger?.Debug("Unsubscribe: Channel is currently subscribing. Abort unsubscribe (Channel: {Channel})", channelName);
              return false;
            case SubscriptionState.Unsubscribed:
            case SubscriptionState.Unsubscribing:
              _client.Logger?.Debug("Unsubscribe: Channel is unsubscribed or unsubscribing. Remove callback (Channel: {Channel})", channelName);
              entry.Callbacks.Remove(callbackEntry);
              return true;
            case SubscriptionState.Subscribed:
              if (entry.Callbacks.Count > 1)
              {
                _client.Logger?.Debug("Unsubscribe: There are still callbacks left. Remove callback but don't unsubscribe (Channel: {Channel})", channelName);
                entry.Callbacks.Remove(callbackEntry);
                return true;
              }

              _client.Logger?.Debug("Unsubscribe: No callbacks left. Unsubscribe and remove callback (Channel: {Channel})", channelName);
              break;
            default:
              return false;
          }

          // At this point it's only possible that the entry-State is Subscribed
          // and the callback list is empty after removing this callback.
          // Hence we unsubscribe at the server now
          entry.State = SubscriptionState.Unsubscribing;
          taskSource = new TaskCompletionSource<bool>();
          entry.UnsubscribeTask = taskSource.Task;
        }

        try
        {
          var unsubscribeResponse = await _client.Send(
            IsPrivateChannel(channelName) ? "private/unsubscribe" : "public/unsubscribe",
            new {channels = new[] {channelName}},
            new ListJsonConverter<string>()).ConfigureAwait(false);

          var response = unsubscribeResponse.ResultData;

          if (response.Count != 1 || response[0] != channelName)
          {
            entry.State = SubscriptionState.Subscribed;
            entry.UnsubscribeTask = null;
            taskSource.SetResult(false);
          }
          else
          {
            entry.Callbacks.Remove(callbackEntry);
            entry.State = SubscriptionState.Unsubscribed;
            entry.UnsubscribeTask = null;
            taskSource.SetResult(true);
          }
        }
        catch (Exception e)
        {
          entry.State = SubscriptionState.Subscribed;
          entry.UnsubscribeTask = null;
          taskSource.SetException(e);
        }

        return await taskSource.Task;
      }

      public IEnumerable<Action<Notification>> GetCallbacks(string channel)
      {
        if (_subscriptionMap.TryGetValue(channel, out var entry))
        {
          foreach (var callbackEntry in entry.Callbacks)
          {
            yield return callbackEntry.Action;
          }
        }
      }

      public void Reset()
      {
        _subscriptionMap.Clear();
      }

      private static bool IsPrivateChannel(string channel)
      {
        return channel.StartsWith("user.");
      }

      private (string channelName, SubscriptionEntry entry, SubscriptionCallback callbackEntry) GetEntryByToken(SubscriptionToken token)
      {
        lock (_subscriptionMap)
        {
          foreach (var kvp in _subscriptionMap)
          {
            foreach (var callbackEntry in kvp.Value.Callbacks)
            {
              if (callbackEntry.Token == token)
              {
                return (kvp.Key, kvp.Value, callbackEntry);
              }
            }
          }
        }

        return (null, null, null);
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
    ///     Provide <see cref="AuthParams.ClientId" /> and <see cref="AuthParams.ClientSecret" />.
    ///     Optional: Provide <see cref="AuthParams.Data" /> for more random data for signature calculation
    ///   </para>
    /// </summary>
    /// <param name="args"><see cref="AuthParams" /> object containing the necessary values</param>
    public async Task<JsonRpcResponse<AuthInfo>> PublicAuth(AuthParams args)
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

        reqParams = new {grant_type = "refresh_token", refresh_token = RefreshToken};
      }

      var response = await Send("public/auth", reqParams, new ObjectJsonConverter<AuthInfo>());

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
    public Task<JsonRpcResponse<AuthInfo>> PublicExchangeToken(string refreshToken, int subjectId)
    {
      return Send(
        "public/exchange_token",
        new {refresh_token = refreshToken, subject_id = subjectId},
        new ObjectJsonConverter<AuthInfo>());
    }

    /// <summary>
    ///   Generates token for new named session. This method can be used only with session scoped tokens.
    /// </summary>
    /// <param name="refreshToken">Refresh token</param>
    /// <param name="sessionName">New session name</param>
    public Task<JsonRpcResponse<AuthInfo>> PublicForkToken(string refreshToken, string sessionName)
    {
      return Send(
        "public/fork_token",
        new {refresh_token = refreshToken, session_name = sessionName},
        new ObjectJsonConverter<AuthInfo>());
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
      _client.SendLogoutSync("private/logout", null);
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
    public Task<JsonRpcResponse<string>> PublicSetHeartbeat(int interval)
    {
      return Send("public/set_heartbeat", new {interval}, new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   Stop sending heartbeat messages.
    /// </summary>
    public Task<JsonRpcResponse<string>> PublicDisableHeartbeat()
    {
      return Send("public/disable_heartbeat", null, new ObjectJsonConverter<string>());
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
    public Task<JsonRpcResponse<string>> PrivateEnableCancelOnDisconnect()
    {
      return PrivateEnableCancelOnDisconnect("connection");
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
    public Task<JsonRpcResponse<string>> PrivateEnableCancelOnDisconnect(string scope)
    {
      //TODO: check if private method works without access_token being sent
      return Send(
        "private/enable_cancel_on_disconnect",
        new {scope /*, access_token = AccessToken*/},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   <para>Disable Cancel On Disconnect for the connection.</para>
    ///   <para>
    ///     When change is applied for the account, then every newly opened connection will start with inactive Cancel on
    ///     Disconnect
    ///   </para>
    /// </summary>
    public Task<JsonRpcResponse<string>> PrivateDisableCancelOnDisconnect()
    {
      return PrivateDisableCancelOnDisconnect("connection");
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
    public Task<JsonRpcResponse<string>> PrivateDisableCancelOnDisconnect(string scope)
    {
      //TODO: check if private method works without access_token being sent
      return Send(
        "private/disable_cancel_on_disconnect",
        new {scope /*, access_token = AccessToken*/},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   Read current Cancel On Disconnect configuration for the account
    /// </summary>
    /// <param name="scope">
    ///   Specifies if Cancel On Disconnect change should be applied/checked for the current connection or
    ///   the account (default - <c>connection</c>)
    /// </param>
    public Task<JsonRpcResponse<CancelOnDisconnectInfo>> PrivateGetCancelOnDisconnect()
    {
      return PrivateGetCancelOnDisconnect("connection");
    }

    /// <summary>
    ///   Read current Cancel On Disconnect configuration for the account
    /// </summary>
    /// <param name="scope">
    ///   Specifies if Cancel On Disconnect change should be applied/checked for the current connection or
    ///   the account (default - <c>connection</c>)
    /// </param>
    public Task<JsonRpcResponse<CancelOnDisconnectInfo>> PrivateGetCancelOnDisconnect(string scope)
    {
      //TODO: check if private method works without access_token being sent
      return Send(
        "private/get_cancel_on_disconnect",
        new {scope /*, access_token = AccessToken*/},
        new ObjectJsonConverter<CancelOnDisconnectInfo>());
    }

    #endregion

    #region Supporting

    /// <summary>
    ///   Retrieves the current time (in milliseconds).
    ///   This API endpoint can be used to check the clock skew between your software and Deribit's systems.
    /// </summary>
    public Task<JsonRpcResponse<DateTime>> PublicGetTime()
    {
      return Send("public/get_time", null, new TimestampJsonConverter());
    }

    /// <summary>
    ///   Method used to introduce the client software connected to Deribit platform over websocket.
    ///   Provided data may have an impact on the maintained connection and will be collected for internal statistical
    ///   purposes.
    ///   In response, Deribit will also introduce itself.
    /// </summary>
    /// <param name="clientName">Client software name</param>
    /// <param name="clientVersion">Client software version</param>
    public Task<JsonRpcResponse<ServerHello>> PublicHello(string clientName, string clientVersion)
    {
      return Send(
        "public/hello",
        new {client_name = clientName, client_version = clientVersion},
        new ObjectJsonConverter<ServerHello>());
    }

    /// <summary>
    ///   Tests the connection to the API server, and returns its version.
    ///   You can use this to make sure the API is reachable, and matches the expected version.
    /// </summary>
    /// <param name="expectedResult">
    ///   The value "exception" will trigger an error response. This may be useful for testing
    ///   wrapper libraries.
    /// </param>
    public Task<JsonRpcResponse<ServerHello>> PublicTest(string expectedResult)
    {
      return Send(
        "public/test",
        new {expected_result = expectedResult},
        new ObjectJsonConverter<ServerHello>());
    }

    #endregion

    #region Account management

    /// <summary>
    ///   Retrieves user account summary
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="extended">Include additional fields</param>
    public Task<JsonRpcResponse<AccountSummary>> PrivateGetAccountSummary(string currency, bool extended)
    {
      return Send(
        "private/get_account_summary",
        new {currency, extended},
        new ObjectJsonConverter<AccountSummary>());
    }

    /// <summary>
    ///   Retrieves the language to be used for emails
    /// </summary>
    public Task<JsonRpcResponse<string>> PrivateGetEmailLanguage()
    {
      return Send(
        "private/get_email_language",
        null,
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   Changes the language to be used for emails
    /// </summary>
    /// <param name="language">
    ///   The abbreviated language name. Valid values include <c>en</c>, <c>ko</c>, <c>zh</c>, <c>ja</c>,
    ///   <c>ru</c>
    /// </param>
    public Task<JsonRpcResponse<string>> PrivateSetEmailLanguage(string language)
    {
      return Send(
        "private/set_email_language",
        new {language},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   Retrieve user position
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    public Task<JsonRpcResponse<UserPosition>> PrivateGetPosition(string instrumentName)
    {
      return Send(
        "private/get_position",
        new {instrument_name = instrumentName},
        new ObjectJsonConverter<UserPosition>());
    }

    /// <summary>
    ///   Retrieve user positions
    /// </summary>
    /// <param name="currency"><c>BTC</c> or <c>ETH</c></param>
    /// <param name="kind">Kind filter on positions: <c>future</c> or <c>option</c></param>
    public Task<JsonRpcResponse<UserPosition[]>> PrivateGetPositions(string currency, string kind = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      return Send(
        "private/get_positions",
        args,
        new ObjectJsonConverter<UserPosition[]>());
    }

    #region Announcements

    /// <inheritdoc cref="PublicGetAnnouncements(DateTime, int)" />
    public Task<JsonRpcResponse<Announcement[]>> PublicGetAnnouncements()
    {
      return Send(
        "public/get_announcements",
        null,
        new ObjectJsonConverter<Announcement[]>());
    }

    /// <inheritdoc cref="PublicGetAnnouncements(DateTime, int)" />
    public Task<JsonRpcResponse<Announcement[]>> PublicGetAnnouncements(DateTime startTime)
    {
      return Send(
        "public/get_announcements",
        new {start_timestamp = startTime.AsMilliseconds()},
        new ObjectJsonConverter<Announcement[]>());
    }

    /// <inheritdoc cref="PublicGetAnnouncements(DateTime, int)" />
    public Task<JsonRpcResponse<Announcement[]>> PublicGetAnnouncements(int count)
    {
      if (count < 1 || count > 50)
      {
        throw new ArgumentException("count must be between 1 and 50", nameof(count));
      }

      return Send(
        "public/get_announcements",
        new {count},
        new ObjectJsonConverter<Announcement[]>());
    }

    /// <summary>
    ///   <para>Retrieves announcements.</para>
    ///   <para>Default <c>startTime</c> value is current timestamp. Default <c>count</c> value is 5</para>
    /// </summary>
    /// <param name="startTime">Timestamp from which we want to retrieve announcements</param>
    /// <param name="count">Maximum count of returned announcements. Must be between 1 and 50</param>
    public Task<JsonRpcResponse<Announcement[]>> PublicGetAnnouncements(DateTime startTime, int count)
    {
      if (count < 1 || count > 50)
      {
        throw new ArgumentException("count must be between 1 and 50", nameof(count));
      }

      return Send(
        "public/get_announcements",
        new {start_timestamp = startTime.AsMilliseconds(), count},
        new ObjectJsonConverter<Announcement[]>());
    }

    /// <summary>
    ///   Retrieves announcements that have not been marked read by the user
    /// </summary>
    public Task<JsonRpcResponse<Announcement[]>> PrivateGetNewAnnouncements()
    {
      return Send(
        "private/get_new_announcements",
        null,
        new ObjectJsonConverter<Announcement[]>());
    }

    /// <summary>
    ///   Marks an announcement as read, so it will not be shown in <see cref="PrivateGetNewAnnouncements" />
    /// </summary>
    /// <param name="id">The ID of the announcement</param>
    public Task<JsonRpcResponse<string>> PrivateSetAnnouncementAsRead(long id)
    {
      return Send(
        "private/set_announcement_as_read",
        new {announcement_id = id},
        new ObjectJsonConverter<string>());
    }

    #endregion

    #region Api Key

    //TODO: Check this
    /// <summary>
    ///   Changes name for key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    /// <param name="name">Name of key (only letters, numbers and underscores allowed; maximum length is 16 characters)</param>
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateChangeApiKeyName(int id, string name)
    {
      return Send(
        "private/change_api_key_name",
        new {id, name},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <summary>
    ///   Changes scope for key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    /// <param name="maxScope"></param>
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateChangeScopeInApiKey(int id, string maxScope)
    {
      return Send(
        "private/change_scope_in_api_key",
        new {id, max_scope = maxScope},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <inheritdoc cref="PrivateCreateApiKey(string, string, bool)" />
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateCreateApiKey(string maxScope)
    {
      return Send(
        "private/create_api_key",
        new {max_scope = maxScope},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <inheritdoc cref="PrivateCreateApiKey(string, string, bool)" />
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateCreateApiKey(string maxScope, string name)
    {
      return Send(
        "private/create_api_key",
        new {name, max_scope = maxScope},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <inheritdoc cref="PrivateCreateApiKey(string, string, bool)" />
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateCreateApiKey(string maxScope, bool asDefault)
    {
      return Send(
        "private/create_api_key",
        new {@default = asDefault, max_scope = maxScope},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <summary>
    ///   Creates new api key with given scope
    /// </summary>
    /// <param name="maxScope"></param>
    /// <param name="name">Name of key (only letters, numbers and underscores allowed; maximum length is 16 characters)</param>
    /// <param name="asDefault">If <c>true</c>, new key is marked as default</param>
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateCreateApiKey(string maxScope, string name, bool asDefault)
    {
      return Send(
        "private/create_api_key",
        new {name, @default = asDefault, max_scope = maxScope},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <summary>
    ///   Disables api key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateDisableApiKey(int id)
    {
      return Send(
        "private/disable_api_key",
        new {id},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <summary>
    ///   Enables api key with given id
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateEnableApiKey(int id)
    {
      return Send(
        "private/enable_api_key",
        new {id},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    /// <summary>
    ///   Retrieves list of api keys
    /// </summary>
    public Task<JsonRpcResponse<ApiKeyInfo[]>> PrivateListApiKeys()
    {
      return Send(
        "private/list_api_keys",
        null,
        new ObjectJsonConverter<ApiKeyInfo[]>());
    }

    /// <summary>
    ///   Removes api key
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<string>> PrivateRemoveApiKey(int id)
    {
      return Send(
        "private/remove_api_key",
        new {id},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    ///   Resets secret in api key
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateResetApiKey(int id)
    {
      return Send(
        "private/reset_api_key",
        new {id},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    //TODO: Check this
    /// <summary>
    ///   Sets key with given id as default one
    /// </summary>
    /// <param name="id">Id of key</param>
    public Task<JsonRpcResponse<ApiKeyInfo>> PrivateSetApiKeyAsDefault(int id)
    {
      return Send(
        "private/set_api_key_as_default",
        new {id},
        new ObjectJsonConverter<ApiKeyInfo>());
    }

    #endregion

    #region Subaccount

    //TODO: Check this
    /// <summary>
    ///   Change the user name for a subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="name">The new user name</param>
    public Task<JsonRpcResponse<string>> PrivateChangeSubaccountName(int sid, string name)
    {
      return Send(
        "private/change_subaccount_name",
        new {sid, name},
        new ObjectJsonConverter<string>());
    }

    //TODO: check this
    /// <summary>
    ///   Create a new subaccount
    /// </summary>
    public Task<JsonRpcResponse<SubAccount>> PrivateCreateSubaccount()
    {
      return Send(
        "private/create_subaccount",
        null,
        new ObjectJsonConverter<SubAccount>());
    }

    /// <summary>
    ///   Disable two factor authentication for a subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    public Task<JsonRpcResponse<string>> PrivateDisableTfaForSubaccount(int sid)
    {
      return Send(
        "private/disable_tfa_for_subaccount",
        new {sid},
        new ObjectJsonConverter<string>());
    }

    //TODO: check this
    /// <inheritdoc cref="PrivateGetSubaccounts(bool)" />
    public Task<JsonRpcResponse<SubAccount[]>> PrivateGetSubaccounts()
    {
      return Send(
        "private/get_subaccounts",
        null,
        new ObjectJsonConverter<SubAccount[]>());
    }

    //TODO: check this
    /// <summary>
    ///   Get information about subaccounts
    /// </summary>
    public Task<JsonRpcResponse<SubAccount[]>> PrivateGetSubaccounts(bool withPortfolio)
    {
      return Send(
        "private/get_subaccounts",
        new {with_portfolio = withPortfolio},
        new ObjectJsonConverter<SubAccount[]>());
    }

    //TODO: Check this
    /// <summary>
    ///   Assign an email address to a subaccount. User will receive an email with confirmation link.
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="email">The email address for the subaccount</param>
    public Task<JsonRpcResponse<string>> PrivateSetEmailForSubaccount(int sid, string email)
    {
      return Send(
        "private/set_email_for_subaccount",
        new {sid, email},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    ///   Set the password for the subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="password">The password address for the subaccount</param>
    public Task<JsonRpcResponse<string>> PrivateSetPasswordForSubaccount(int sid, string password)
    {
      return Send(
        "private/set_password_for_subaccount",
        new {sid, password},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    ///   Enable or disable sending of notifications for the subaccount
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="state">enable (<c>true</c>) or disable (<c>false</c>) notifications</param>
    public Task<JsonRpcResponse<string>> PrivateToggleNotificationsFromSubaccount(int sid, bool state)
    {
      return Send(
        "private/toggle_notifications_from_subaccount",
        new {sid, state},
        new ObjectJsonConverter<string>());
    }

    //TODO: Check this
    /// <summary>
    ///   Enable or disable login for a subaccount. If login is disabled and a session for the subaccount exists, this session
    ///   will be terminated
    /// </summary>
    /// <param name="sid">The user id for the subaccount</param>
    /// <param name="state">enable (<c>true</c>) or disable (<c>false</c>) login</param>
    public Task<JsonRpcResponse<string>> PrivateToggleSubaccountLogin(int sid, bool state)
    {
      return Send(
        "private/toggle_subaccount_login",
        new {sid, state = state ? "enable" : "disable"},
        new ObjectJsonConverter<string>());
    }

    #endregion

    #endregion

    #region Block Trade

    //TODO: Check this
    /// <summary>
    ///   <para>Creates block trade</para>
    ///   <para>
    ///     The whole request have to be exact the same as in <c>private/verify_block_trade</c>, only role field should be
    ///     set appropriately - it basically means that both sides have to agree on the same timestamp, nonce, trades fields
    ///     and server will assure that role field is different between sides (each party accepted own role).
    ///   </para>
    ///   <para>
    ///     Using the same timestamp and nonce by both sides in <c>private/verify_block_trade</c> assures that even if
    ///     unintentionally both sides execute given block trade with valid <c>counterparty_signature</c>, the given block
    ///     trade will be executed only once.
    ///   </para>
    /// </summary>
    public Task<JsonRpcResponse<BlockTrade>> PrivateExecuteBlockTrade(BlockTradeParams @params)
    {
      var args = new ExpandoObject();
      args.TryAdd("timestamp", @params.Timestamp);
      args.TryAdd("nonce", @params.Nonce);
      args.TryAdd("role", @params.Role);
      args.TryAdd("trades", @params.Trades);
      args.TryAdd("counterparty_signature", @params.CounterpartySignature);

      if (@params.Currency != default)
      {
        args.TryAdd("currency", @params.Currency);
      }

      return Send(
        "private/execute_block_trade",
        args,
        new ObjectJsonConverter<BlockTrade>());
    }

    /// <summary>
    ///   Returns information about users block trade
    /// </summary>
    /// <param name="id">Block trade id</param>
    public Task<JsonRpcResponse<BlockTrade>> PrivateGetBlockTrade(string id)
    {
      return Send(
        "private/get_block_trade",
        new {id},
        new ObjectJsonConverter<BlockTrade>());
    }

    /// <summary>
    ///   Returns list of last users block trades
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    /// <param name="startId">The id of the newest block trade to be returned</param>
    /// <param name="endId">
    ///   The id of the oldest block trade to be returned, <paramref name="startId" /> is required with
    ///   <paramref name="endId" />
    /// </param>
    public Task<JsonRpcResponse<BlockTrade[]>> PrivateGetLastBlockTradesByCurrency(string currency,
      int count = default, string startId = default, string endId = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (startId != default)
      {
        args.TryAdd("start_id", startId);
      }

      if (endId != default)
      {
        args.TryAdd("end_id", endId);
      }

      return Send(
        "private/get_last_block_trades_by_currency",
        args,
        new ObjectJsonConverter<BlockTrade[]>());
    }

    /// <summary>
    ///   User at any time (before the private/execute_block_trade is called) can invalidate its own signature effectively
    ///   cancelling block trade
    /// </summary>
    /// <param name="signature">Signature of block trade that will be invalidated</param>
    public Task<JsonRpcResponse<string>> PrivateInvalidateBlockTradeSignature(string signature)
    {
      return Send(
        "private/invalidate_block_trade_signature",
        new {signature},
        new ObjectJsonConverter<string>());
    }

    /// <summary>
    ///   Verifies and creates block trade signature
    /// </summary>
    public Task<JsonRpcResponse<BlockTradeSignature>> PrivateVerifyBlockTrade(BlockTradeParams @params)
    {
      var args = new ExpandoObject();
      args.TryAdd("timestamp", @params.Timestamp);
      args.TryAdd("nonce", @params.Nonce);
      args.TryAdd("role", @params.Role);
      args.TryAdd("trades", @params.Trades);

      if (@params.Currency != default)
      {
        args.TryAdd("currency", @params.Currency);
      }

      return Send(
        "private/verify_block_trade",
        args,
        new ObjectJsonConverter<BlockTradeSignature>());
    }

    #endregion

    #region Trading

    /// <summary>
    ///   Places a buy order for an instrument
    /// </summary>
    public Task<JsonRpcResponse<UserOrderTrades>> PrivateBuy(BuyParams @params)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", @params.InstrumentName);
      args.TryAdd("amount", @params.Amount);

      if (@params.Type != default)
      {
        args.TryAdd("type", @params.Type);
      }

      if (@params.Label != default)
      {
        args.TryAdd("label", @params.Label);
      }

      if (@params.Price.HasValue)
      {
        args.TryAdd("price", @params.Price);
      }

      if (@params.TimeInForce != default)
      {
        args.TryAdd("time_in_force", @params.TimeInForce);
      }

      if (@params.MaxShow.HasValue)
      {
        args.TryAdd("max_show", @params.MaxShow);
      }

      if (@params.PostOnly.HasValue)
      {
        args.TryAdd("post_only", @params.PostOnly);
      }

      if (@params.RejectPostOnly.HasValue)
      {
        args.TryAdd("reject_post_only", @params.RejectPostOnly);
      }

      if (@params.ReduceOnly.HasValue)
      {
        args.TryAdd("reduce_only", @params.ReduceOnly);
      }

      if (@params.StopPrice.HasValue)
      {
        args.TryAdd("stop_price", @params.StopPrice);
      }

      if (@params.Trigger != default)
      {
        args.TryAdd("trigger", @params.Trigger);
      }

      if (@params.Advanced != default)
      {
        args.TryAdd("advanced", @params.Advanced);
      }

      return Send(
        "private/buy",
        args,
        new ObjectJsonConverter<UserOrderTrades>());
    }

    /// <summary>
    ///   Places a sell order for an instrument
    /// </summary>
    public Task<JsonRpcResponse<UserOrderTrades>> PrivateSell(SellParams @params)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", @params.InstrumentName);
      args.TryAdd("amount", @params.Amount);

      if (@params.Type != default)
      {
        args.TryAdd("type", @params.Type);
      }

      if (@params.Label != default)
      {
        args.TryAdd("label", @params.Label);
      }

      if (@params.Price.HasValue)
      {
        args.TryAdd("price", @params.Price);
      }

      if (@params.TimeInForce != default)
      {
        args.TryAdd("time_in_force", @params.TimeInForce);
      }

      if (@params.MaxShow.HasValue)
      {
        args.TryAdd("max_show", @params.MaxShow);
      }

      if (@params.PostOnly.HasValue)
      {
        args.TryAdd("post_only", @params.PostOnly);
      }

      if (@params.RejectPostOnly.HasValue)
      {
        args.TryAdd("reject_post_only", @params.RejectPostOnly);
      }

      if (@params.ReduceOnly.HasValue)
      {
        args.TryAdd("reduce_only", @params.ReduceOnly);
      }

      if (@params.StopPrice.HasValue)
      {
        args.TryAdd("stop_price", @params.StopPrice);
      }

      if (@params.Trigger != default)
      {
        args.TryAdd("trigger", @params.Trigger);
      }

      if (@params.Advanced != default)
      {
        args.TryAdd("advanced", @params.Advanced);
      }

      return Send(
        "private/sell",
        args,
        new ObjectJsonConverter<UserOrderTrades>());
    }

    /// <summary>
    ///   Change price, amount and/or other properties of an order
    /// </summary>
    public Task<JsonRpcResponse<UserOrderTrades>> PrivateEdit(EditParams @params)
    {
      var args = new ExpandoObject();
      args.TryAdd("order_id", @params.OrderId);
      args.TryAdd("amount", @params.Amount);
      args.TryAdd("price", @params.Price);

      if (@params.PostOnly.HasValue)
      {
        args.TryAdd("post_only", @params.PostOnly);
      }

      if (@params.ReduceOnly.HasValue)
      {
        args.TryAdd("reduce_only", @params.ReduceOnly);
      }

      if (@params.RejectPostOnly.HasValue)
      {
        args.TryAdd("reject_post_only", @params.RejectPostOnly);
      }

      if (@params.Advanced != default)
      {
        args.TryAdd("advanced", @params.Advanced);
      }

      if (@params.StopPrice.HasValue)
      {
        args.TryAdd("stop_price", @params.StopPrice);
      }

      return Send(
        "private/edit",
        args,
        new ObjectJsonConverter<UserOrderTrades>());
    }

    /// <summary>
    ///   Cancel an order, specified by order id
    /// </summary>
    /// <param name="orderId">The order id</param>
    public Task<JsonRpcResponse<UserOrder>> PrivateCancel(string orderId)
    {
      return Send(
        "private/cancel",
        new {order_id = orderId},
        new ObjectJsonConverter<UserOrder>());
    }

    /// <summary>
    ///   This method cancels all users orders and stop orders within all currencies and instrument kinds
    /// </summary>
    public Task<JsonRpcResponse<int>> PrivateCancelAll()
    {
      return Send(
        "private/cancel_all",
        null,
        new ObjectJsonConverter<int>());
    }

    /// <summary>
    ///   Cancels all orders by currency, optionally filtered by instrument kind and/or order type
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="kind">
    ///   <para>Instrument kind, if not provided instruments of all kinds are considered.</para>
    ///   <para>Enum: <c>future</c>, <c>option</c></para>
    /// </param>
    /// <param name="type">
    ///   <para>Order type, default - <c>all</c></para>
    ///   <para>Enum: <c>all</c>, <c>limit</c> or <c>stop</c></para>
    /// </param>
    public Task<JsonRpcResponse<int>> PrivateCancelAllByCurrency(string currency, string kind = default, string type = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (type != default)
      {
        args.TryAdd("type", type);
      }

      return Send(
        "private/cancel_all_by_currency",
        args,
        new ObjectJsonConverter<int>());
    }

    /// <summary>
    ///   Cancels all orders by instrument, optionally filtered by order type
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="type">
    ///   <para>Order type, default - <c>all</c></para>
    ///   <para>Enum: <c>all</c>, <c>limit</c>, <c>stop</c></para>
    /// </param>
    public Task<JsonRpcResponse<int>> PrivateCancelAllByInstrument(string instrumentName, string type = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (type != default)
      {
        args.TryAdd("type", type);
      }

      return Send(
        "private/cancel_all_by_instrument",
        args,
        new ObjectJsonConverter<int>());
    }

    /// <summary>
    ///   Cancels orders by label. All user's orders (stop orders too), with given label are canceled in all currencies
    /// </summary>
    /// <param name="label">User defined label for the order (maximum 64 characters)</param>
    public Task<JsonRpcResponse<int>> PrivateCancelByLabel(string label)
    {
      return Send(
        "private/cancel_by_label",
        new {label},
        new ObjectJsonConverter<int>());
    }

    /// <summary>
    ///   Makes closing position reduce only order
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="type">
    ///   <para>The order type</para>
    ///   <para>Enum: <c>limit</c>, <c>market</c></para>
    /// </param>
    /// <param name="price">Optional price for limit order</param>
    public Task<JsonRpcResponse<UserOrderTrades>> PrivateClosePosition(string instrumentName, string type, decimal price = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);
      args.TryAdd("type", type);

      if (price != default)
      {
        args.TryAdd("price", price);
      }

      return Send(
        "private/close_position",
        args,
        new ObjectJsonConverter<UserOrderTrades>());
    }

    /// <summary>
    ///   Get margins for given instrument, amount and price
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="amount">
    ///   Amount, integer for future, float for option. For perpetual and futures the amount is in USD
    ///   units, for options it is amount of corresponding crypto currency contracts, e.g., BTC or ETH
    /// </param>
    /// <param name="price">Price</param>
    public Task<JsonRpcResponse<UserMargin>> PrivateGetMargins(string instrumentName, decimal amount, decimal price)
    {
      return Send(
        "private/get_margins",
        new {instrument_name = instrumentName, amount, price},
        new ObjectJsonConverter<UserMargin>());
    }

    /// <summary>
    ///   Retrieves list of user's open orders
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="kind">
    ///   <para>Instrument kind, if not provided instruments of all kinds are considered.</para>
    ///   <para>Enum: <c>future</c>, <c>option</c></para>
    /// </param>
    /// <param name="type">
    ///   <para>Order type, default - <c>all</c></para>
    ///   <para>Enum: <c>all</c>, <c>limit</c>, <c>stop_all</c>, <c>stop_limit</c>, <c>stop_market</c></para>
    /// </param>
    public Task<JsonRpcResponse<UserOrder[]>> PrivateGetOpenOrdersByCurrency(string currency, string kind = default, string type = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (type != default)
      {
        args.TryAdd("type", type);
      }

      return Send(
        "private/get_open_orders_by_currency",
        args,
        new ObjectJsonConverter<UserOrder[]>());
    }

    /// <summary>
    ///   Retrieves list of user's open orders within given instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="type">
    ///   <para>Order type, default - <c>all</c></para>
    ///   <para>Enum: <c>all</c>, <c>limit</c>, <c>stop_all</c>, <c>stop_limit</c>, <c>stop_market</c></para>
    /// </param>
    public Task<JsonRpcResponse<UserOrder[]>> PrivateGetOpenOrdersByInstrument(string instrumentName, string type = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (type != default)
      {
        args.TryAdd("type", type);
      }

      return Send(
        "private/get_open_orders_by_instrument",
        args,
        new ObjectJsonConverter<UserOrder[]>());
    }

    /// <summary>
    ///   Retrieves history of orders that have been partially or fully filled
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="kind">
    ///   <para>Instrument kind, if not provided instruments of all kinds are considered.</para>
    ///   <para>Enum: <c>future</c>, <c>option</c></para>
    /// </param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    /// <param name="offset">The offset for pagination, default - <c>0</c></param>
    /// <param name="includeOld">Include in result orders oder than 2 days, default - <c>false</c></param>
    /// <param name="includeUnfilled">Include in result fully unfilled closed orders, default - <c>false</c></param>
    public Task<JsonRpcResponse<UserOrder[]>> PrivateGetOrderHistoryByCurrency(string currency,
      string kind = default, int count = default, int offset = default, bool? includeOld = null, bool? includeUnfilled = null)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (offset > 0)
      {
        args.TryAdd("offset", offset);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (includeUnfilled.HasValue)
      {
        args.TryAdd("include_unfilled", includeUnfilled.Value);
      }

      return Send(
        "private/get_order_history_by_currency",
        args,
        new ObjectJsonConverter<UserOrder[]>());
    }

    /// <summary>
    ///   Retrieves history of orders that have been partially or fully filled
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    /// <param name="offset">The offset for pagination, default - <c>0</c></param>
    /// <param name="includeOld">Include in result orders oder than 2 days, default - <c>false</c></param>
    /// <param name="includeUnfilled">Include in result fully unfilled closed orders, default - <c>false</c></param>
    public Task<JsonRpcResponse<UserOrder[]>> PrivateGetOrderHistoryByInstrument(string instrumentName,
      int count = default, int offset = default, bool? includeOld = null, bool? includeUnfilled = null)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (offset > 0)
      {
        args.TryAdd("offset", offset);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (includeUnfilled.HasValue)
      {
        args.TryAdd("include_unfilled", includeUnfilled.Value);
      }

      return Send(
        "private/get_order_history_by_instrument",
        args,
        new ObjectJsonConverter<UserOrder[]>());
    }

    /// <summary>
    ///   Retrieves initial margins of given orders
    /// </summary>
    /// <param name="ids">IDs of orders</param>
    public Task<JsonRpcResponse<UserOrderMargin[]>> PrivateGetOrderMarginByIDs(params string[] ids)
    {
      return Send(
        "private/get_order_margin_by_ids",
        new {ids},
        new ObjectJsonConverter<UserOrderMargin[]>());
    }

    /// <summary>
    ///   Retrieve the current state of an order
    /// </summary>
    /// <param name="orderId"></param>
    public Task<JsonRpcResponse<UserOrder>> PrivateGetOrderState(string orderId)
    {
      return Send(
        "private/get_order_state",
        new {order_id = orderId},
        new ObjectJsonConverter<UserOrder>());
    }

    /// <summary>
    ///   Retrieves detailed log of the user's stop-orders
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    /// <param name="continuation">Continuation token for pagination</param>
    public Task<JsonRpcResponse<UserStopOrderCollection>> PrivateGetStopOrderHistory(string currency,
      string instrumentName = default, int count = default, string continuation = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (instrumentName != default)
      {
        args.TryAdd("instrument_name", instrumentName);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (continuation != default)
      {
        args.TryAdd("continuation", continuation);
      }

      return Send(
        "private/get_stop_order_history",
        args,
        new ObjectJsonConverter<UserStopOrderCollection>());
    }

    /// <summary>
    ///   Retrieve the latest user trades that have occurred for instruments in a specific currency symbol
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="kind">
    ///   <para>Instrument kind, if not provided instruments of all kinds are considered.</para>
    ///   <para>Enum: <c>future</c>, <c>option</c></para>
    /// </param>
    /// <param name="startId">The ID number of the first trade to be returned</param>
    /// <param name="endId">The ID number of the last trade to be returned</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<UserTradeCollection>> PrivateGetUserTradesByCurrency(string currency,
      string kind = default, string startId = default, string endId = default, int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (startId != default)
      {
        args.TryAdd("start_id", startId);
      }

      if (endId != default)
      {
        args.TryAdd("end_id", endId);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "private/get_user_trades_by_currency",
        args,
        new ObjectJsonConverter<UserTradeCollection>());
    }

    /// <summary>
    ///   Retrieve the latest user trades that have occurred for instruments in a specific currency symbol and within given
    ///   time range
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="kind">
    ///   <para>Instrument kind, if not provided instruments of all kinds are considered.</para>
    ///   <para>Enum: <c>future</c>, <c>option</c></para>
    /// </param>
    /// <param name="startTime">The earliest timestamp to return result for</param>
    /// <param name="endTime">The most recent timestamp to return result for</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<UserTradeCollection>> PrivateGetUserTradesByCurrencyAndTime(string currency,
      string kind = default, DateTime startTime = default, DateTime endTime = default, int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (startTime != default)
      {
        args.TryAdd("start_timestamp", startTime.AsMilliseconds());
      }

      if (endTime != default)
      {
        args.TryAdd("end_timestamp", endTime.AsMilliseconds());
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "private/get_user_trades_by_currency_and_time",
        args,
        new ObjectJsonConverter<UserTradeCollection>());
    }

    /// <summary>
    ///   Retrieve the latest user trades that have occurred for a specific instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="startSeq">The sequence number of the first trade to be returned</param>
    /// <param name="endSeq">The sequence number of the last trade to be returned</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<UserTrade[]>> PrivateGetUserTradesByInstrument(string instrumentName,
      long startSeq = default, long endSeq = default, int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (startSeq != default)
      {
        args.TryAdd("start_seq", startSeq);
      }

      if (endSeq != default)
      {
        args.TryAdd("end_seq", endSeq);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "private/get_user_trades_by_instrument",
        args,
        new ObjectJsonConverter<UserTrade[]>());
    }

    /// <summary>
    ///   Retrieve the latest user trades that have occurred for a specific instrument and within given time range
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="startTime">The earliest timestamp to return result for</param>
    /// <param name="endTime">The most recent timestamp to return result for</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<UserTrade[]>> PrivateGetUserTradesByInstrumentAndTime(string instrumentName,
      DateTime startTime = default, DateTime endTime = default, int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (startTime != default)
      {
        args.TryAdd("start_timestamp", startTime.AsMilliseconds());
      }

      if (endTime != default)
      {
        args.TryAdd("end_timestamp", endTime.AsMilliseconds());
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "private/get_user_trades_by_instrument_and_time",
        args,
        new ObjectJsonConverter<UserTrade[]>());
    }

    /// <summary>
    ///   Retrieves the list of user trades that was created for given order
    /// </summary>
    /// <param name="orderId">The order id</param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<UserTrade[]>> PrivateGetUserTradesByOrder(string orderId, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("order_id", orderId);

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "private/get_user_trades_by_order",
        args,
        new ObjectJsonConverter<UserTrade[]>());
    }

    /// <summary>
    ///   Retrieves public settlement, delivery and bankruptcy events filtered by instrument name
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="type">
    ///   <para>Settlement type</para>
    ///   <para>Enum: <c>settlement</c>, <c>delivery</c>, <c>bankruptcy</c></para>
    /// </param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    public Task<JsonRpcResponse<UserSettlementCollection>> PrivateGetSettlementHistoryByInstrument(string instrumentName,
      string type = default, int count = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (type != default)
      {
        args.TryAdd("type", type);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      return Send(
        "private/get_settlement_history_by_instrument",
        args,
        new ObjectJsonConverter<UserSettlementCollection>());
    }

    /// <summary>
    ///   Retrieves settlement, delivery and bankruptcy events that have affected your account
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="type">
    ///   <para>Settlement type</para>
    ///   <para>Enum: <c>settlement</c>, <c>delivery</c>, <c>bankruptcy</c></para>
    /// </param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    public Task<JsonRpcResponse<UserSettlementCollection>> PrivateGetSettlementHistoryByCurrency(string currency, string type = default, int count = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (type != default)
      {
        args.TryAdd("type", type);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      return Send(
        "private/get_settlement_history_by_currency",
        args,
        new ObjectJsonConverter<UserSettlementCollection>());
    }

    #endregion

    #region Market data

    /// <summary>
    ///   <para>
    ///     Retrieves the summary information such as open interest, 24h volume, etc. for all instruments for the currency
    ///     (optionally filtered by kind)
    ///   </para>
    ///   <para>Valid values: <c>BTC</c>, <c>ETH</c></para>
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="kind">
    ///   Instrument kind, if not provided instruments of all kinds are considered. Valid values:
    ///   <c>future</c>, <c>option</c>
    /// </param>
    public Task<JsonRpcResponse<BookSummary[]>> PublicGetBookSummaryByCurrency(string currency, string kind = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      return Send(
        "public/get_book_summary_by_currency",
        args,
        new ObjectJsonConverter<BookSummary[]>());
    }

    /// <summary>
    ///   Retrieves the summary information such as open interest, 24h volume, etc. for a specific instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    public Task<JsonRpcResponse<BookSummary[]>> PublicGetBookSummaryByInstrument(string instrumentName)
    {
      return Send(
        "public/get_book_summary_by_instrument",
        new {instrument_name = instrumentName},
        new ObjectJsonConverter<BookSummary[]>());
    }

    /// <summary>
    ///   Retrieves contract size of provided instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    public Task<JsonRpcResponse<InstrumentContractSize>> PublicGetContractSize(string instrumentName)
    {
      return Send(
        "public/get_contract_size",
        new {instrument_name = instrumentName},
        new ObjectJsonConverter<InstrumentContractSize>());
    }

    /// <summary>
    ///   Retrieves all crypto currencies supported by the API
    /// </summary>
    public Task<JsonRpcResponse<Currency[]>> PublicGetCurrencies()
    {
      return Send(
        "public/get_currencies",
        null,
        new ObjectJsonConverter<Currency[]>());
    }

    /// <summary>
    ///   Retrieve the latest user trades that have occurred for PERPETUAL instruments in a specific currency symbol and within
    ///   given time range
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="length">Specifies time period. <c>8h</c> - 8 hours, <c>24h</c> - 24 hours, <c>1m</c> - 1 month</param>
    public Task<JsonRpcResponse<PerpetualUserTrades>> PublicGetFundingChartData(string instrumentName, string length)
    {
      return Send(
        "public/get_funding_chart_data",
        new {instrument_name = instrumentName, length},
        new ObjectJsonConverter<PerpetualUserTrades>());
    }

    /// <summary>
    ///   Retrieves hourly historical interest rate for requested instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="startTime">The earliest timestamp to return result for</param>
    /// <param name="endTime">The most recent timestamp to return result for</param>
    public Task<JsonRpcResponse<InterestRate[]>> PublicGetFundingRateHistory(string instrumentName, DateTime startTime, DateTime endTime)
    {
      return Send(
        "public/get_funding_rate_history",
        new {instrument_name = instrumentName, start_timestamp = startTime.AsMilliseconds(), end_timestamp = endTime.AsMilliseconds()},
        new ObjectJsonConverter<InterestRate[]>());
    }

    /// <summary>
    ///   Retrieves interest rate value for requested period
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="startTime">The earliest timestamp to return result for</param>
    /// <param name="endTime">The most recent timestamp to return result for</param>
    public Task<JsonRpcResponse<decimal>> PublicGetFundingRateValue(string instrumentName, DateTime startTime, DateTime endTime)
    {
      return Send(
        "public/get_funding_rate_value",
        new {instrument_name = instrumentName, start_timestamp = startTime.AsMilliseconds(), end_timestamp = endTime.AsMilliseconds()},
        new ObjectJsonConverter<decimal>());
    }

    /// <summary>
    ///   Provides information about historical volatility for given crypto currency
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    public Task<JsonRpcResponse<VolatilityItem[]>> PublicGetHistoricalVolatility(string currency)
    {
      return Send(
        "public/get_historical_volatility",
        new {currency},
        new ObjectJsonConverter<VolatilityItem[]>());
    }

    /// <summary>
    ///   Retrieves the current index price for the instruments, for the selected currency
    /// </summary>
    /// <param name="currency">The currency symbol. Valid values: <c>BTC</c>, <c>ETH</c></param>
    public Task<JsonRpcResponse<IndexPrice>> PublicGetIndex(string currency)
    {
      return Send(
        "public/get_index",
        new {currency},
        new ObjectJsonConverter<IndexPrice>());
    }

    /// <summary>
    ///   Retrieves available trading instrument. This method can be used to see which instruments are available for trading,
    ///   or which instruments have existed historically
    ///   https://docs.deribit.com/#public-get_instrument
    /// </summary>
    /// <param name="instrument_name">The instrument_name. Valid values: <c>BTC-25JUN21</c>, <c>ETH-25JUN21</c></param>
    public Task<JsonRpcResponse<Instrument>> PublicGetInstrument(string instrument_name)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrument_name);

      return Send(
        "public/get_instrument",
        args,
        new ObjectJsonConverter<Instrument>());
    }

    /// <summary>
    ///   Retrieves available trading instruments. This method can be used to see which instruments are available for trading,
    ///   or which instruments have existed historically
    /// </summary>
    /// <param name="currency">The currency symbol. Valid values: <c>BTC</c>, <c>ETH</c></param>
    /// <param name="kind">
    ///   Instrument kind, if not provided instruments of all kinds are considered. Valid values:
    ///   <c>future</c>, <c>option</c>
    /// </param>
    /// <param name="expired">Set to true to show expired instruments instead of active ones</param>
    public Task<JsonRpcResponse<Instrument[]>> PublicGetInstruments(string currency, string kind = default, bool? expired = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (expired != default)
      {
        args.TryAdd("expired", expired.Value);
      }

      return Send(
        "public/get_instruments",
        args,
        new ObjectJsonConverter<Instrument[]>());
    }

    /// <summary>
    ///   Retrieves historical settlement, delivery and bankruptcy events coming from all instruments with given currency
    /// </summary>
    /// <param name="currency">The currency symbol. Valid values: <c>BTC</c>, <c>ETH</c></param>
    /// <param name="type">Settlement type. Valid values: <c>settlement</c>, <c>delivery</c>, <c>bankruptcy</c></param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    /// <param name="continuation">Continuation token for pagination</param>
    /// <param name="searchStartTime">The latest timestamp to return result for</param>
    public Task<JsonRpcResponse<MarketSettlementCollection>> PublicGetLastSettlementsByCurrency(string currency,
      string type = default, int count = default, string continuation = default, DateTime searchStartTime = default)
    {
      var eo = new ExpandoObject();
      eo.TryAdd("currency", currency);

      if (type != default)
      {
        eo.TryAdd("type", type);
      }

      if (count > 0)
      {
        eo.TryAdd("count", count);
      }

      if (continuation != default)
      {
        eo.TryAdd("continuation", continuation);
      }

      if (searchStartTime != default)
      {
        eo.TryAdd("search_start_timestamp", searchStartTime.AsMilliseconds());
      }

      return Send(
        "public/get_last_settlements_by_currency",
        eo,
        new ObjectJsonConverter<MarketSettlementCollection>());
    }

    /// <summary>
    ///   Retrieves historical settlement, delivery and bankruptcy events filtered by instrument name
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="type">Settlement type. Valid values: <c>settlement</c>, <c>delivery</c>, <c>bankruptcy</c></param>
    /// <param name="count">Number of requested items, default - <c>20</c></param>
    /// <param name="continuation">Continuation token for pagination</param>
    /// <param name="searchStartTime">The latest timestamp to return result for</param>
    public Task<JsonRpcResponse<MarketSettlementCollection>> PublicGetLastSettlementsByInstrument(string instrumentName,
      string type = default, int count = default, string continuation = default, DateTime searchStartTime = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (type != default)
      {
        args.TryAdd("type", type);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (continuation != default)
      {
        args.TryAdd("continuation", continuation);
      }

      if (searchStartTime != default)
      {
        args.TryAdd("search_start_timestamp", searchStartTime.AsMilliseconds());
      }

      return Send(
        "public/get_last_settlements_by_instrument",
        args,
        new ObjectJsonConverter<MarketSettlementCollection>());
    }

    /// <summary>
    ///   Retrieve the latest trades that have occurred for instruments in a specific currency symbol
    /// </summary>
    /// <param name="currency">The currency symbol. Valid values: <c>BTC</c>, <c>ETH</c></param>
    /// <param name="kind">
    ///   Instrument kind, if not provided instruments of all kinds are considered. Valid values:
    ///   <c>future</c>, <c>option</c>
    /// </param>
    /// <param name="startId">The ID number of the first trade to be returned</param>
    /// <param name="endId">The ID number of the last trade to be returned</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<MarketTradeCollection>> PublicGetLastTradesByCurrency(string currency,
      string kind = default, string startId = default, string endId = default, int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (startId != default)
      {
        args.TryAdd("start_id", startId);
      }

      if (endId != default)
      {
        args.TryAdd("end_id", endId);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "public/get_last_trades_by_currency",
        args,
        new ObjectJsonConverter<MarketTradeCollection>());
    }

    /// <summary>
    ///   Retrieve the latest trades that have occurred for instruments in a specific currency symbol
    /// </summary>
    /// <param name="currency">The currency symbol. Valid values: <c>BTC</c>, <c>ETH</c></param>
    /// <param name="startTime">The earliest timestamp to return result for</param>
    /// <param name="endTime">The most recent timestamp to return result for</param>
    /// <param name="kind">
    ///   Instrument kind, if not provided instruments of all kinds are considered. Valid values:
    ///   <c>future</c>, <c>option</c>
    /// </param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<MarketTradeCollection>> PublicGetLastTradesByCurrencyAndTime(string currency, DateTime startTime, DateTime endTime,
      string kind = default, int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);
      args.TryAdd("start_timestamp", startTime.AsMilliseconds());
      args.TryAdd("end_timestamp", endTime.AsMilliseconds());

      if (kind != default)
      {
        args.TryAdd("kind", kind);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "public/get_last_trades_by_currency_and_time",
        args,
        new ObjectJsonConverter<MarketTradeCollection>());
    }

    /// <summary>
    ///   Retrieve the latest trades that have occurred for a specific instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="startSeq">The sequence number of the first trade to be returned</param>
    /// <param name="endSeq">The sequence number of the last trade to be returned</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<MarketTradeCollection>> PublicGetLastTradesByInstrument(string instrumentName,
      long startSeq = default, long endSeq = default, int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (startSeq != default)
      {
        args.TryAdd("start_seq", startSeq);
      }

      if (endSeq != default)
      {
        args.TryAdd("end_seq", endSeq);
      }

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "public/get_last_trades_by_instrument",
        args,
        new ObjectJsonConverter<MarketTradeCollection>());
    }

    /// <summary>
    ///   Retrieve the latest trades that have occured for a specific instrument and within given time range
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="startTime">The earliest timestamp to return result for</param>
    /// <param name="endTime">The most recent timestamp to return result for</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="includeOld">Include trades older than a few recent days, default - <c>false</c></param>
    /// <param name="sorting">
    ///   <para>
    ///     Direction of results sorting (<c>"default"</c> value means no sorting, results will be returned
    ///     in order in which they left the database)
    ///   </para>
    ///   <para>Enum: <c>asc</c>, <c>desc</c>, <c>default</c></para>
    /// </param>
    public Task<JsonRpcResponse<MarketTradeCollection>> PublicGetLastTradesByInstrumentAndTime(
      string instrumentName, DateTime startTime, DateTime endTime,
      int count = default, bool? includeOld = null, string sorting = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);
      args.TryAdd("start_timestamp", startTime.AsMilliseconds());
      args.TryAdd("end_timestamp", endTime.AsMilliseconds());

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (includeOld.HasValue)
      {
        args.TryAdd("include_old", includeOld.Value);
      }

      if (sorting != default)
      {
        args.TryAdd("sorting", sorting);
      }

      return Send(
        "public/get_last_trades_by_instrument_and_time",
        args,
        new ObjectJsonConverter<MarketTradeCollection>());
    }

    /// <summary>
    ///   Retrieves the order book, along with other market values for a given instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="depth">The number of entries to return for bids and asks</param>
    public Task<JsonRpcResponse<OrderBook>> PublicGetOrderBook(string instrumentName, int depth = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("instrument_name", instrumentName);

      if (depth > 0)
      {
        args.TryAdd("depth", depth);
      }

      return Send(
        "public/get_order_book",
        args,
        new ObjectJsonConverter<OrderBook>());
    }

    /// <summary>
    ///   Retrieves aggregated 24h trade volumes for different instrument types and currencies
    /// </summary>
    public Task<JsonRpcResponse<TradeVolume[]>> PublicGetTradeVolumes()
    {
      return Send(
        "public/get_trade_volumes",
        null,
        new ObjectJsonConverter<TradeVolume[]>());
    }

    /// <summary>
    ///   Publicly available market data used to generate a TradingView candle chart
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    /// <param name="startTime">The earliest timestamp to return result for</param>
    /// <param name="endTime">The most recent timestamp to return result for</param>
    /// <param name="resolution">
    ///   <para>Chart bars resolution given in full minutes or keyword <c>1D</c> (only some specific resolutions are supported)</para>
    ///   <para>
    ///     Supported resolutions: <c>1</c>, <c>3</c>, <c>5</c>, <c>10</c>, <c>15</c>, <c>30</c>, <c>60</c>, <c>120</c>,
    ///     <c>180</c>, <c>360</c>, <c>720</c>, <c>1D</c>
    ///   </para>
    /// </param>
    public Task<JsonRpcResponse<TradingViewMarketData>> PublicGetTradingViewData(string instrumentName, DateTime startTime, DateTime endTime, string resolution)
    {
      return Send(
        "public/get_tradingview_chart_data",
        new {instrument_name = instrumentName, start_timestamp = startTime.AsMilliseconds(), end_timestamp = endTime.AsMilliseconds(), resolution},
        new ObjectJsonConverter<TradingViewMarketData>());
    }

    /// <summary>
    ///   Get ticker for an instrument
    /// </summary>
    /// <param name="instrumentName">Instrument name</param>
    public Task<JsonRpcResponse<TickerData>> PublicTicker(string instrumentName)
    {
      return Send(
        "public/ticker",
        new {instrument_name = instrumentName},
        new ObjectJsonConverter<TickerData>());
    }

    #endregion

    #region Wallet

    /// <summary>
    ///   Cancel transfer
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="id">Id of transfer</param>
    /// <param name="tfa">TFA code, required when TFA is enabled for current account</param>
    public Task<JsonRpcResponse<TransferInfo>> PrivateCancelTransferById(string currency, long id, string tfa = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);
      args.TryAdd("id", id);

      if (tfa != default)
      {
        args.TryAdd("tfa", tfa);
      }

      return Send(
        "private/cancel_transfer_by_id",
        args,
        new ObjectJsonConverter<TransferInfo>());
    }

    /// <summary>
    ///   Cancels withdrawal request
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="id">The withdrawal id</param>
    public Task<JsonRpcResponse<WithdrawalInfo>> PrivateCancelWithdrawal(string currency, long id)
    {
      return Send(
        "private/cancel_withdrawal",
        new {currency, id},
        new ObjectJsonConverter<WithdrawalInfo>());
    }

    /// <summary>
    ///   Creates deposit address in currency
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    public Task<JsonRpcResponse<DepositAddress>> PrivateCreateDepositAddress(string currency)
    {
      return Send(
        "private/create_deposit_address",
        new {currency},
        new ObjectJsonConverter<DepositAddress>());
    }

    /// <summary>
    ///   Retrieve deposit address for currency
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    public Task<JsonRpcResponse<DepositAddress>> PrivateGetCurrentDepositAddress(string currency)
    {
      return Send(
        "private/get_current_deposit_address",
        new {currency},
        new ObjectJsonConverter<DepositAddress>());
    }

    /// <summary>
    ///   Retrieve the latest users deposits
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="offset">The offset for pagination, default - <c>0</c></param>
    public Task<JsonRpcResponse<DepositCollection>> PrivateGetDeposits(string currency, int count = default, int offset = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (offset > 0)
      {
        args.TryAdd("offset", offset);
      }

      return Send(
        "private/get_deposits",
        args,
        new ObjectJsonConverter<DepositCollection>());
    }

    /// <summary>
    ///   Adds new entry to address book of given type
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="offset">The offset for pagination, default - <c>0</c></param>
    public Task<JsonRpcResponse<TransferCollection>> PrivateGetTransfers(string currency, int count = default, int offset = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (offset > 0)
      {
        args.TryAdd("offset", offset);
      }

      return Send(
        "private/get_transfers",
        args,
        new ObjectJsonConverter<TransferCollection>());
    }

    /// <summary>
    ///   Retrieve the latest users withdrawals
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="count">Number of requested items, default - <c>10</c></param>
    /// <param name="offset">The offset for pagination, default - <c>0</c></param>
    public Task<JsonRpcResponse<WithdrawalCollection>> PrivateGetWithdrawals(string currency, int count = default, int offset = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);

      if (count > 0)
      {
        args.TryAdd("count", count);
      }

      if (offset > 0)
      {
        args.TryAdd("offset", offset);
      }

      return Send(
        "private/get_withdrawals",
        args,
        new ObjectJsonConverter<WithdrawalCollection>());
    }

    /// <summary>
    ///   Transfers funds to subaccount
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="amount">Amount of funds to be transferred</param>
    /// <param name="destination">Id of destination subaccount. Can be found in <c>My Account >> Subaccounts</c> tab</param>
    public Task<JsonRpcResponse<TransferInfo>> PrivateSubmitTransferToSubAccount(string currency, decimal amount, int destination)
    {
      return Send(
        "private/submit_transfer_to_subaccount",
        new {currency, amount, destination},
        new ObjectJsonConverter<TransferInfo>());
    }

    /// <summary>
    ///   Transfers funds to subaccount
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="amount">Amount of funds to be transferred</param>
    /// <param name="destination">Destination wallet's address taken from address book</param>
    /// <param name="tfa">TFA code, required when TFA is enabled for current account</param>
    public Task<JsonRpcResponse<TransferInfo>> PrivateSubmitTransferToUser(string currency, decimal amount, string destination, string tfa = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);
      args.TryAdd("amount", amount);
      args.TryAdd("destination", destination);

      if (tfa != default)
      {
        args.TryAdd("tfa", tfa);
      }

      return Send(
        "private/submit_transfer_to_user",
        args,
        new ObjectJsonConverter<TransferInfo>());
    }

    /// <summary>
    /// </summary>
    /// <param name="currency">The currency symbol</param>
    /// <param name="address">Address in current format, it must be in address book</param>
    /// <param name="amount">Amount of funds to be transferred</param>
    /// <param name="priority">
    ///   <para>Withdrawal priority, optional for BTC, default: <c>high</c></para>
    ///   <para>
    ///     Possible values: <c>insane</c>, <c>extreme_high</c>, <c>very_high</c>, <c>high</c>, <c>mid</c>, <c>low</c>,
    ///     <c>very_low</c>
    ///   </para>
    /// </param>
    /// <param name="tfa">TFA code, required when TFA is enabled for current account</param>
    public Task<JsonRpcResponse<WithdrawalInfo>> PrivateWithdraw(string currency, string address, decimal amount, string priority = default,
      string tfa = default)
    {
      var args = new ExpandoObject();
      args.TryAdd("currency", currency);
      args.TryAdd("address", address);
      args.TryAdd("amount", amount);

      if (priority != default)
      {
        args.TryAdd("priority", priority);
      }

      if (tfa != default)
      {
        args.TryAdd("tfa", tfa);
      }

      return Send(
        "private/withdraw",
        args,
        new ObjectJsonConverter<WithdrawalInfo>());
    }

    #endregion

    #region Subscriptions

    /// <summary>
    ///   Unsubscribe from a Subscription you subscribed before
    /// </summary>
    public Task<bool> Unsubscribe(SubscriptionToken token)
    {
      return _subscriptionManager.Unsubscribe(token);
    }

    /// <summary>
    ///   General announcements concerning the Deribit platform.
    /// </summary>
    public Task<SubscriptionToken> SubscribeAnnouncements(Action<AnnouncementNotification> callback)
    {
      return _subscriptionManager.Subscribe(new AnnouncementsSubscriptionParams(),
        n => callback(n.Data.ToObject<AnnouncementNotification>()));
    }

    /// <summary>
    ///   <para>Notifies about changes to the order book for a certain instrument.</para>
    ///   <para>
    ///     Notifications are sent once per specified interval, with prices grouped by (rounded to) the specified group,
    ///     showing the complete order book to the specified depth (number of price levels).
    ///   </para>
    ///   <para>
    ///     The 'asks' and the 'bids' elements in the response are both a 'list of lists'. Each element in the outer list
    ///     is a list of exactly two elements: price and amount.
    ///   </para>
    ///   <para>price is a price level (USD per BTC, rounded as specified by the 'group' parameter for the subscription).</para>
    ///   <para>
    ///     amount indicates the amount of all orders at price level. For perpetual and futures the amount is in USD units,
    ///     for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.
    ///   </para>
    /// </summary>
    public Task<SubscriptionToken> SubscribeBookGroup(BookGroupSubscriptionParams @params, Action<BookGroupNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<BookGroupNotification>()));
    }

    /// <summary>
    ///   <para>Notifies about changes to the order book for a certain instrument.</para>
    ///   <para>
    ///     The first notification will contain the whole book (bid and ask amounts for all prices). After that there will
    ///     only be information about changes to individual price levels.
    ///   </para>
    ///   <para>
    ///     The first notification will contain the amounts for all price levels (list of <c>['new', price, amount]</c>
    ///     tuples). All following notifications will contain a list of tuples with action, price level and new amount (
    ///     <c>[action, price, amount]</c>). Action can be either <c>new</c>, <c>change</c> or <c>delete</c>.
    ///   </para>
    ///   <para>
    ///     Each notification will contain a <c>change_id</c> field, and each message except for the first one will contain
    ///     a field <c>prev_change_id</c>. If <c>prev_change_id</c> is equal to the <c>change_id</c> of the previous message,
    ///     this means that no messages have been missed.
    ///   </para>
    ///   <para>
    ///     The amount for perpetual and futures is in USD units, for options it is in corresponding cryptocurrency
    ///     contracts, e.g., BTC or ETH.
    ///   </para>
    /// </summary>
    public Task<SubscriptionToken> SubscribeBookChange(BookChangeSubscriptionParams @params, Action<BookChangeNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<BookChangeNotification>()));
    }

    /// <summary>
    ///   <para>
    ///     Publicly available market data used to generate a TradingView candle chart. During single resolution period,
    ///     many events can be sent, each with updated values for the recent period.
    ///   </para>
    ///   <para>
    ///     NOTICE: When there is no trade during the requested resolution period (e.g. 1 minute), then filling sample is
    ///     generated which uses data from the last available trade candle (open and close values).
    ///   </para>
    /// </summary>
    public Task<SubscriptionToken> SubscribeChartTrades(ChartTradesSubscriptionParams @params, Action<ChartTradesNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<ChartTradesNotification>()));
    }

    /// <summary>
    ///   Provides information about current value (price) for Deribit Index
    /// </summary>
    public Task<SubscriptionToken> SubscribeDeribitPriceIndex(DeribitPriceIndexSubscriptionParams @params, Action<DeribitPriceIndexNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<DeribitPriceIndexNotification>()));
    }

    /// <summary>
    ///   Provides information about current value (price) for stock exchanges used to calculate Deribit Index
    /// </summary>
    public Task<SubscriptionToken> SubscribeDeribitPriceRanking(DeribitPriceRankingSubscriptionParams @params,
      Action<DeribitPriceRankingNotification[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<DeribitPriceRankingNotification[]>()));
    }

    /// <summary>
    ///   Returns calculated (estimated) ending price for given index
    /// </summary>
    public Task<SubscriptionToken> SubscribeEstimatedExpirationPrice(EstimatedExpirationPriceSubscriptionParams @params,
      Action<EstimatedExpirationPriceNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<EstimatedExpirationPriceNotification>()));
    }

    /// <summary>
    ///   Provides information about options markprices
    /// </summary>
    public Task<SubscriptionToken> SubscribeMarkPriceOptions(MarkPriceOptionsSubscriptionParams @params, Action<MarkPriceOptionsNotification[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<MarkPriceOptionsNotification[]>()));
    }

    /// <summary>
    ///   Provide current interest rate - but only for <c>perpetual</c> instruments. Other types won't generate any
    ///   notification.
    /// </summary>
    public Task<SubscriptionToken> SubscribePerpetualInterestRate(PerpetualInterestRateSubscriptionParams @params,
      Action<PerpetualInterestRateNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<PerpetualInterestRateNotification>()));
    }

    /// <summary>
    ///   Information about platform state
    /// </summary>
    public Task<SubscriptionToken> SubscribePlatformState(Action<PlatformStateNotification> callback)
    {
      return _subscriptionManager.Subscribe(new PlatformStateSubscriptionParams(),
        n => callback(n.Data.ToObject<PlatformStateNotification>()));
    }

    /// <summary>
    ///   Best bid/ask price and size
    /// </summary>
    public Task<SubscriptionToken> SubscribeQuote(QuoteSubscriptionParams @params, Action<QuoteNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<QuoteNotification>()));
    }

    /// <summary>
    ///   Key information about the instrument
    /// </summary>
    public Task<SubscriptionToken> SubscribeTicker(TickerSubscriptionParams @params, Action<TickerNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<TickerNotification>()));
    }

    /// <summary>
    ///   Get notifications about trades for an instrument
    /// </summary>
    public Task<SubscriptionToken> SubscribeTradesInstrument(TradesInstrumentSubscriptionParams @params, Action<TradesNotification[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<TradesNotification[]>()));
    }

    /// <summary>
    ///   Get notifications about trades in any instrument of a given kind and given currency
    /// </summary>
    public Task<SubscriptionToken> SubscribeTradesKindCurrency(TradesKindCurrencySubscriptionParams @params, Action<TradesNotification[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<TradesNotification[]>()));
    }

    /// <summary>
    ///   Get notifications about user's updates related to order, trades, etc. in an instrument.
    /// </summary>
    public Task<SubscriptionToken> SubscribeUserChangesInstrument(UserChangesInstrumentSubscriptionParams @params, Action<UserChangesNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<UserChangesNotification>()));
    }

    /// <summary>
    ///   Get notifications about changes in user's updates related to orders, trades, etc. in instruments of a given kind and
    ///   currency.
    /// </summary>
    public Task<SubscriptionToken> SubscribeUserChangesKindCurrency(UserChangesKindCurrencySubscriptionParams @params, Action<UserChangesNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<UserChangesNotification>()));
    }

    /// <summary>
    ///   Get notifications about changes in user's orders for given instrument
    /// </summary>
    public Task<SubscriptionToken> SubscribeUserOrdersInstrument(UserOrdersInstrumentSubscriptionParams @params, Action<UserOrder[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n =>
        {
          callback(@params.Interval.Equals("raw") ? new[] {n.Data.ToObject<UserOrder>()} : n.Data.ToObject<UserOrder[]>());
        });
    }

    /// <summary>
    ///   Get notifications about changes in user's orders in instrument of a given kind and currency
    /// </summary>
    public Task<SubscriptionToken> SubscribeUserOrdersKindCurrency(UserOrdersKindCurrencySubscriptionParams @params, Action<UserOrder[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n =>
        {
          callback(@params.Interval.Equals("raw") ? new[] {n.Data.ToObject<UserOrder>()} : n.Data.ToObject<UserOrder[]>());
        });
    }

    /// <summary>
    ///   Provides information about current user portfolio
    /// </summary>
    public Task<SubscriptionToken> SubscribeUserPortfolio(UserPortfolioSubscriptionParams @params, Action<UserPortfolioNotification> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<UserPortfolioNotification>()));
    }

    /// <summary>
    ///   Get notifications about user's trades in an instrument
    /// </summary>
    public Task<SubscriptionToken> SubscribeUserTradesInstrument(UserTradesInstrumentSubscriptionParams @params, Action<UserTrade[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<UserTrade[]>()));
    }

    /// <summary>
    ///   Get notifications about user's trades in any instrument of a given kind and given currency
    /// </summary>
    public Task<SubscriptionToken> SubscribeUserTradesKindCurrency(UserTradesKindCurrencySubscriptionParams @params, Action<UserTrade[]> callback)
    {
      return _subscriptionManager.Subscribe(@params,
        n => callback(n.Data.ToObject<UserTrade[]>()));
    }

    #endregion

    #endregion
  }
}
