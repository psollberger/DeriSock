// ReSharper disable UnusedMember.Local
// ReSharper disable InheritdocConsiderUsage

namespace DeriSock;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Constants;
using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;
using DeriSock.Request;
using DeriSock.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;
using Serilog.Events;

/// <summary>
///   <para>The implementation of the API methods from Deribit</para>
///   <para>All methods are asynchronous. Synchronous methods are suffixed with <c>Sync</c></para>
/// </summary>
public partial class DeribitClient
{
  private static readonly JsonSerializerSettings SerializationSettings = new()
  {
    NullValueHandling = NullValueHandling.Ignore
  };

  /// <summary>
  ///   Occurs after the client established a connection with the endpoint.
  /// </summary>
  public event EventHandler? Connected;

  /// <summary>
  ///   Occurs after client lost the connection with the endpoint.
  /// </summary>
  public event EventHandler<DeribitClientDisconnectedEventArgs>? Disconnected;

  private readonly ILogger? _logger;
  private readonly JsonRpcRequestTaskSourceMap _requestTaskSourceMap = new();
  private readonly SubscriptionManager _subscriptionManager;

  private readonly Uri _endpointUri;
  private readonly IJsonRpcMessageSource _messageSource;

  private CancellationTokenSource _processMessageStreamCts;
  private Task? _processMessageStreamTask;

  /// <summary>
  ///   The AccessToken received from the server after authentication
  /// </summary>
  public string? AccessToken { get; private set; }

  /// <summary>
  ///   The RefreshToken received from the server after authentication
  /// </summary>
  public string? RefreshToken { get; private set; }

  /// <summary>
  ///   Gets if the underlying message source is fully connected and ready to receive messages.
  /// </summary>
  public bool IsConnected
    => _messageSource.State switch
    {
      WebSocketState.Open => true,
      _                   => false
    };

  /// <summary>
  ///   Creates a new <see cref="DeribitClient" /> instance.
  /// </summary>
  /// <param name="endpointType">The endpoint type.</param>
  /// <param name="messageSource">The message source. <c>null</c> to use <see cref="DefaultJsonRpcMessageSource" /></param>
  /// <param name="logger">Optional implementation of the <see cref="ILogger" /> interface to enable logging capabilities.</param>
  /// <exception cref="ArgumentOutOfRangeException">Throws an exception if the <paramref name="endpointType" /> is not supported.</exception>
  public DeribitClient(EndpointType endpointType, IJsonRpcMessageSource? messageSource = null, ILogger? logger = null)
  {
    _endpointUri = endpointType switch
    {
      EndpointType.Productive => Endpoint.Productive,
      EndpointType.Testnet    => Endpoint.TestNet,
      _                       => throw new ArgumentOutOfRangeException(nameof(endpointType), endpointType, "Unsupported endpoint type")
    };

    _messageSource = messageSource ?? new DefaultJsonRpcMessageSource(logger);
    _logger = logger;

    _subscriptionManager = new SubscriptionManager(this);
  }

  /// <summary>
  ///   Connects to the endpoint.
  /// </summary>
  /// <returns>The task object representing the asynchronous operation.</returns>
  public async Task Connect(CancellationToken cancellationToken = default)
  {
    await _messageSource.Connect(_endpointUri, cancellationToken).ConfigureAwait(false);

    AccessToken = null;
    RefreshToken = null;
    _subscriptionManager.Reset();

    _processMessageStreamCts = new CancellationTokenSource();
    _processMessageStreamTask = Task.Factory.StartNew(async () => await ProcessMessageStream(_processMessageStreamCts.Token).ConfigureAwait(false), TaskCreationOptions.LongRunning);

    Connected?.Invoke(this, EventArgs.Empty);
  }

  /// <summary>
  ///   Disconnects from the server
  /// </summary>
  /// <returns>The task object representing the asynchronous operation.</returns>
  public async Task Disconnect(CancellationToken cancellationToken = default)
  {
    if (_processMessageStreamTask is not null) {
      _processMessageStreamCts.Cancel();
      await Task.WhenAll(_processMessageStreamTask);
    }

    await _messageSource.Disconnect(null, null, cancellationToken).ConfigureAwait(false);

    AccessToken = null;
    RefreshToken = null;

    Disconnected?.Invoke(this, new DeribitClientDisconnectedEventArgs(_messageSource.CloseStatus, _messageSource.CloseStatusDescription, _messageSource.Exception));
  }

  private async Task<JsonRpcResponse<T>> Send<T>(string method, object? @params, IJsonConverter<T> converter, CancellationToken cancellationToken = default)
  {
    var request = new JsonRpcRequest
    {
      Id = RequestIdGenerator.Next(),
      Method = method,
      Params = @params
    };

    _logger?.Verbose("Sending: {@Request}", request);

    var taskSource = new TaskCompletionSource<JsonRpcResponse>();
    _requestTaskSourceMap.Add(request, taskSource);

    await _messageSource.Send(JsonConvert.SerializeObject(request, Formatting.None, SerializationSettings), cancellationToken).ConfigureAwait(false);
    var response = await taskSource.Task.ConfigureAwait(false);
    return response.CreateTyped(converter.Convert(response.Result)!);
  }

  private void SendSync(string method, object? @params)
  {
    var request = new JsonRpcRequest
    {
      Id = RequestIdGenerator.Next(),
      Method = method,
      Params = @params
    };

    _logger?.Verbose("Sending synchroneous: {@Request}", request);
    _ = _messageSource.Send(JsonConvert.SerializeObject(request, Formatting.None, SerializationSettings), CancellationToken.None).ConfigureAwait(false);
  }

  private async Task ProcessMessageStream(CancellationToken cancellationToken)
  {
    try {
      await foreach (var message in _messageSource.GetMessageStream(cancellationToken).ConfigureAwait(false)) {
        if (JsonConvert.DeserializeObject(message) is not JObject jObject)
          continue;

        if (jObject.TryGetValue("method", out _))
          InternalOnRequestReceived(jObject);
        else
          InternalOnResponseReceived(message, jObject);
      }
    }
    catch (TaskCanceledException) {
      //ignore - just exit
    }

    if (_messageSource.Exception is not null)
      await Disconnect(CancellationToken.None).ConfigureAwait(false);
  }

  private void InternalOnRequestReceived(JObject requestObject)
  {
    var request = requestObject.ToObject<JsonRpcRequest>();
    Debug.Assert(request != null, nameof(request) + " != null");
    request!.Original = requestObject;

    if (string.Equals(request.Method, "subscription"))
      Task.Run(() => OnNotification(request.Original.ToObject<Notification>()!)).ConfigureAwait(false);
    else if (string.Equals(request.Method, "heartbeat"))
      Task.Run(() => OnHeartbeat(request.Original.ToObject<Heartbeat>()!)).ConfigureAwait(false);
    else
      _logger?.Warning("Unknown Server Request: {@Request}", request);
  }

  private void InternalOnResponseReceived(string message, JObject responseObject)
  {
    var response = responseObject.ToObject<JsonRpcResponse>();
    Debug.Assert(response != null, nameof(response) + " != null");
    response!.Original = message;

    if (response.Id > 0) {
      if (!_requestTaskSourceMap.TryRemove(response.Id, out var request, out var taskSource)) {
        _logger?.Error("Could not find request id {RequestId}", response.Id);
        return;
      }

      if (response.Error != null)
        taskSource!.SetException(new JsonRpcRequestException(request, response));
      else
        taskSource!.SetResult(response);
    }
  }

  private void OnHeartbeat(Heartbeat heartbeat)
  {
    _logger?.Debug("OnHeartbeat: {@Heartbeat}", heartbeat);

    if (heartbeat.Type == "test_request")
      _ = InternalPublicTest();
  }

  private void OnNotification(Notification notification)
  {
    _logger?.Verbose("OnNotification: {@Notification}", notification);

    const int retryDelay = 5;
    const int maxRetries = 10;
    var retryCount = 0;

    Action<Notification>[] callbacks;

    do {
      callbacks = _subscriptionManager.GetCallbacks(notification.Channel);

      if (callbacks is { Length: > 0 })
        break;

      if (retryCount == 0 && (_logger?.IsEnabled(LogEventLevel.Debug) ?? false))
        _logger?.Debug("OnNotification: Could not find subscription for notification. Retrying up to {@maxRetries} times", maxRetries);

      Thread.Sleep(retryDelay);
      retryCount++;
    } while (retryCount < maxRetries);

    if (callbacks is not { Length: > 0 }) {
      if (_logger?.IsEnabled(LogEventLevel.Warning) ?? false)
        _logger?.Warning("OnNotification: Could not find subscription for notification: {@Notification}", notification);

      return;
    }

    foreach (var callback in callbacks) {
      try {
        Task.Run(() => callback(notification));
      }
      catch (Exception ex) {
        _logger?.Error(ex, "OnNotification: Error during event callback call: {@Notification}", notification);
      }
    }
  }

  private void EnqueueAuthRefresh(int expiresIn)
  {
    var expireTimeSpan = TimeSpan.FromSeconds(expiresIn);

    if (expireTimeSpan.TotalMilliseconds > int.MaxValue)
      return;

    _ = Task.Delay(expireTimeSpan.Subtract(TimeSpan.FromSeconds(5))).ContinueWith(
      _ =>
      {
        var result = ((IAuthenticationMethods)this).WithRefreshToken().GetAwaiter().GetResult();
        EnqueueAuthRefresh(result.ResultData.ExpiresIn);
      });
  }

  private class SubscriptionManager
  {
    private readonly DeribitClient _client;
    private readonly SortedDictionary<string, SubscriptionEntry> _subscriptionMap;

    public SubscriptionManager(DeribitClient client)
    {
      _client = client;
      _subscriptionMap = new SortedDictionary<string, SubscriptionEntry>();
    }

    public async Task<SubscriptionToken> Subscribe(ISubscriptionChannel channel, Action<Notification>? callback)
    {
      if (callback == null)
        return SubscriptionToken.Invalid;

      var channelName = channel.ToChannelName();
      TaskCompletionSource<SubscriptionToken>? taskSource = null;
      SubscriptionEntry entry;

      lock (_subscriptionMap) {
        if (!_subscriptionMap.TryGetValue(channelName, out entry)) {
          entry = new SubscriptionEntry();

          if (!_subscriptionMap.TryAdd(channelName, entry)) {
            _client._logger?.Error("Subscribe: Could not add internal item for channel {Channel}", channelName);
            return SubscriptionToken.Invalid;
          }

          taskSource = new TaskCompletionSource<SubscriptionToken>();
          entry.State = SubscriptionState.Subscribing;
          entry.SubscribeTask = taskSource.Task;
        }

        // Entry already exists but is completely unsubscribed
        if (entry.State == SubscriptionState.Unsubscribed) {
          taskSource = new TaskCompletionSource<SubscriptionToken>();
          entry.State = SubscriptionState.Subscribing;
          entry.SubscribeTask = taskSource.Task;
        }

        // Already subscribed - Put the callback in there and let's go
        if (entry.State == SubscriptionState.Subscribed) {
          _client._logger?.Debug("Subscribe: Subscription for channel already exists. Adding callback to list (Channel: {Channel})", channelName);
          var callbackEntry = new SubscriptionCallback(new SubscriptionToken(Guid.NewGuid()), callback);
          entry.Callbacks.Add(callbackEntry);
          return callbackEntry.Token;
        }

        // We are in the middle of unsubscribing from the channel
        if (entry.State == SubscriptionState.Unsubscribing) {
          _client._logger?.Debug("Subscribe: Channel is unsubscribing. Abort subscribe (Channel: {Channel})", channelName);
          return SubscriptionToken.Invalid;
        }
      }

      // Only one state left: Subscribing

      // We are already subscribing
      if (taskSource == null && entry.State == SubscriptionState.Subscribing) {
        _client._logger?.Debug("Subscribe: Channel is already subscribing. Waiting for the task to complete ({Channel})", channelName);

        var subscribeResult = entry.SubscribeTask != null && await entry.SubscribeTask != SubscriptionToken.Invalid;

        if (!subscribeResult && entry.State != SubscriptionState.Subscribed) {
          _client._logger?.Debug("Subscribe: Subscription has failed. Abort subscribe (Channel: {Channel})", channelName);
          return SubscriptionToken.Invalid;
        }

        _client._logger?.Debug("Subscribe: Subscription was successful. Adding callback (Channel: {Channel}", channelName);
        var callbackEntry = new SubscriptionCallback(new SubscriptionToken(Guid.NewGuid()), callback);
        entry.Callbacks.Add(callbackEntry);
        return callbackEntry.Token;
      }

      if (taskSource == null) {
        _client._logger?.Error("Subscribe: Invalid execution state. Missing TaskCompletionSource (Channel: {Channel}", channelName);
        return SubscriptionToken.Invalid;
      }

      try {
        var subscribeResponse = await _client.Send(
                                  IsPrivateChannel(channelName) ? "private/subscribe" : "public/subscribe",
                                  new
                                  {
                                    channels = new[]
                                    {
                                      channelName
                                    }
                                  },
                                  new ListJsonConverter<string>()).ConfigureAwait(false);

        var response = subscribeResponse.ResultData;

        if (response.Count != 1 || response[0] != channelName) {
          _client._logger?.Debug("Subscribe: Invalid result (Channel: {Channel}): {@Response}", channelName, response);
          entry.State = SubscriptionState.Unsubscribed;
          entry.SubscribeTask = null;
          Debug.Assert(taskSource != null, nameof(taskSource) + " != null");
          taskSource.SetResult(SubscriptionToken.Invalid);
        }
        else {
          _client._logger?.Debug("Subscribe: Successfully subscribed. Adding callback (Channel: {Channel})", channelName);

          var callbackEntry = new SubscriptionCallback(new SubscriptionToken(Guid.NewGuid()), callback);
          entry.Callbacks.Add(callbackEntry);
          entry.State = SubscriptionState.Subscribed;
          entry.SubscribeTask = null;
          Debug.Assert(taskSource != null, nameof(taskSource) + " != null");
          taskSource.SetResult(callbackEntry.Token);
        }
      }
      catch (Exception e) {
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

      lock (_subscriptionMap) {
        (channelName, entry, callbackEntry) = GetEntryByToken(token);

        if (string.IsNullOrEmpty(channelName) || entry == null || callbackEntry == null) {
          _client._logger?.Warning("Unsubscribe: Could not find token {token}", token.Token);
          return false;
        }

        switch (entry.State) {
          case SubscriptionState.Subscribing:
            _client._logger?.Debug("Unsubscribe: Channel is currently subscribing. Abort unsubscribe (Channel: {Channel})", channelName);
            return false;

          case SubscriptionState.Unsubscribed:
          case SubscriptionState.Unsubscribing:
            _client._logger?.Debug("Unsubscribe: Channel is unsubscribed or unsubscribing. Remove callback (Channel: {Channel})", channelName);
            entry.Callbacks.Remove(callbackEntry);
            return true;

          case SubscriptionState.Subscribed:
            if (entry.Callbacks.Count > 1) {
              _client._logger?.Debug("Unsubscribe: There are still callbacks left. Remove callback but don't unsubscribe (Channel: {Channel})", channelName);
              entry.Callbacks.Remove(callbackEntry);
              return true;
            }

            _client._logger?.Debug("Unsubscribe: No callbacks left. Unsubscribe and remove callback (Channel: {Channel})", channelName);
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

      try {
        var unsubscribeResponse = await _client.Send(
                                    IsPrivateChannel(channelName) ? "private/unsubscribe" : "public/unsubscribe",
                                    new
                                    {
                                      channels = new[]
                                      {
                                        channelName
                                      }
                                    },
                                    new ListJsonConverter<string>()).ConfigureAwait(false);

        var response = unsubscribeResponse.ResultData;

        if (response.Count != 1 || response[0] != channelName) {
          entry.State = SubscriptionState.Subscribed;
          entry.UnsubscribeTask = null;
          taskSource.SetResult(false);
        }
        else {
          entry.Callbacks.Remove(callbackEntry);
          entry.State = SubscriptionState.Unsubscribed;
          entry.UnsubscribeTask = null;
          taskSource.SetResult(true);
        }
      }
      catch (Exception e) {
        entry.State = SubscriptionState.Subscribed;
        entry.UnsubscribeTask = null;
        taskSource.SetException(e);
      }

      return await taskSource.Task;
    }

    public Action<Notification>[] GetCallbacks(string channel)
    {
      lock (_subscriptionMap) {
        if (_subscriptionMap.TryGetValue(channel, out var entry))
          return (from c in entry.Callbacks select c.Action).ToArray();
      }

      return null;
    }

    public void Reset()
    {
      lock (_subscriptionMap)
        _subscriptionMap.Clear();
    }

    private static bool IsPrivateChannel(string channel)
      => channel.StartsWith("user.");

    private (string channelName, SubscriptionEntry entry, SubscriptionCallback callbackEntry) GetEntryByToken(SubscriptionToken token)
    {
      lock (_subscriptionMap) {
        foreach (var kvp in _subscriptionMap) {
          foreach (var callbackEntry in kvp.Value.Callbacks) {
            if (callbackEntry.Token == token)
              return (kvp.Key, kvp.Value, callbackEntry);
          }
        }
      }

      return (null, null, null);
    }
  }

#region Subscriptions

  /// <summary>
  ///   Unsubscribe from a Subscription you subscribed before
  /// </summary>
  public Task<bool> Unsubscribe(SubscriptionToken token)
    => _subscriptionManager.Unsubscribe(token);

  /// <summary>
  ///   General announcements concerning the Deribit platform.
  /// </summary>
  public Task<SubscriptionToken> SubscribeAnnouncements(Action<AnnouncementNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      new AnnouncementsSubscriptionParams(),
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
    return _subscriptionManager.Subscribe(
      @params,
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
    return _subscriptionManager.Subscribe(
      @params,
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
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<ChartTradesNotification>()));
  }

  /// <summary>
  ///   Provides information about current value (price) for Deribit Index
  /// </summary>
  public Task<SubscriptionToken> SubscribeDeribitPriceIndex(DeribitPriceIndexSubscriptionParams @params, Action<DeribitPriceIndexNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<DeribitPriceIndexNotification>()));
  }

  /// <summary>
  ///   Provides information about current value (price) for stock exchanges used to calculate Deribit Index
  /// </summary>
  public Task<SubscriptionToken> SubscribeDeribitPriceRanking
  (
    DeribitPriceRankingSubscriptionParams @params,
    Action<DeribitPriceRankingNotification[]> callback
  )
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<DeribitPriceRankingNotification[]>()));
  }

  /// <summary>
  ///   Provides information about current value (price) for stock exchanges used to calculate Deribit Index
  /// </summary>
  public Task<SubscriptionToken> SubscribeDeribitVolatilityIndex
  (
    DeribitVolatilityIndexSubscriptionParams @params,
    Action<DeribitVolatilityIndexNotification> callback
  )
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<DeribitVolatilityIndexNotification>()));
  }

  /// <summary>
  ///   Returns calculated (estimated) ending price for given index
  /// </summary>
  public Task<SubscriptionToken> SubscribeEstimatedExpirationPrice
  (
    EstimatedExpirationPriceSubscriptionParams @params,
    Action<EstimatedExpirationPriceNotification> callback
  )
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<EstimatedExpirationPriceNotification>()));
  }

  /// <summary>
  ///   Provides information about options markprices
  /// </summary>
  public Task<SubscriptionToken> SubscribeMarkPriceOptions(MarkPriceOptionsSubscriptionParams @params, Action<MarkPriceOptionsNotification[]> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<MarkPriceOptionsNotification[]>()));
  }

  /// <summary>
  ///   Provide current interest rate - but only for <c>perpetual</c> instruments. Other types won't generate any
  ///   notification.
  /// </summary>
  public Task<SubscriptionToken> SubscribePerpetualInterestRate
  (
    PerpetualInterestRateSubscriptionParams @params,
    Action<PerpetualInterestRateNotification> callback
  )
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<PerpetualInterestRateNotification>()));
  }

  /// <summary>
  ///   Information about platform state
  /// </summary>
  public Task<SubscriptionToken> SubscribePlatformState(Action<PlatformStateNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      new PlatformStateSubscriptionParams(),
      n => callback(n.Data.ToObject<PlatformStateNotification>()));
  }

  /// <summary>
  ///   Best bid/ask price and size
  /// </summary>
  public Task<SubscriptionToken> SubscribeQuote(QuoteSubscriptionParams @params, Action<QuoteNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<QuoteNotification>()));
  }

  /// <summary>
  ///   Key information about the instrument
  /// </summary>
  public Task<SubscriptionToken> SubscribeTicker(TickerSubscriptionParams @params, Action<TickerNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<TickerNotification>()));
  }

  /// <summary>
  ///   Get notifications about trades for an instrument
  /// </summary>
  public Task<SubscriptionToken> SubscribeTradesInstrument(TradesInstrumentSubscriptionParams @params, Action<TradesNotification[]> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<TradesNotification[]>()));
  }

  /// <summary>
  ///   Get notifications about trades in any instrument of a given kind and given currency
  /// </summary>
  public Task<SubscriptionToken> SubscribeTradesKindCurrency(TradesKindCurrencySubscriptionParams @params, Action<TradesNotification[]> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<TradesNotification[]>()));
  }

  /// <summary>
  ///   Get notifications about user's updates related to order, trades, etc. in an instrument.
  /// </summary>
  public Task<SubscriptionToken> SubscribeUserChangesInstrument(UserChangesInstrumentSubscriptionParams @params, Action<UserChangesNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<UserChangesNotification>()));
  }

  /// <summary>
  ///   Get notifications about changes in user's updates related to orders, trades, etc. in instruments of a given kind and
  ///   currency.
  /// </summary>
  public Task<SubscriptionToken> SubscribeUserChangesKindCurrency(UserChangesKindCurrencySubscriptionParams @params, Action<UserChangesNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<UserChangesNotification>()));
  }

  /// <summary>
  ///   Get notifications about changes in user's orders for given instrument
  /// </summary>
  public Task<SubscriptionToken> SubscribeUserOrdersInstrument(UserOrdersInstrumentSubscriptionParams @params, Action<UserOrder[]> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n =>
      {
        callback(
          @params.Interval.Equals("raw")
            ? new[]
            {
              n.Data.ToObject<UserOrder>()
            }
            : n.Data.ToObject<UserOrder[]>());
      });
  }

  /// <summary>
  ///   Get notifications about changes in user's orders in instrument of a given kind and currency
  /// </summary>
  public Task<SubscriptionToken> SubscribeUserOrdersKindCurrency(UserOrdersKindCurrencySubscriptionParams @params, Action<UserOrder[]> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n =>
      {
        callback(
          @params.Interval.Equals("raw")
            ? new[]
            {
              n.Data.ToObject<UserOrder>()
            }
            : n.Data.ToObject<UserOrder[]>());
      });
  }

  /// <summary>
  ///   Provides information about current user portfolio
  /// </summary>
  public Task<SubscriptionToken> SubscribeUserPortfolio(UserPortfolioSubscriptionParams @params, Action<UserPortfolioNotification> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<UserPortfolioNotification>()));
  }

  /// <summary>
  ///   Get notifications about user's trades in an instrument
  /// </summary>
  public Task<SubscriptionToken> SubscribeUserTradesInstrument(UserTradesInstrumentSubscriptionParams @params, Action<UserTrade[]> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<UserTrade[]>()));
  }

  /// <summary>
  ///   Get notifications about user's trades in any instrument of a given kind and given currency
  /// </summary>
  public Task<SubscriptionToken> SubscribeUserTradesKindCurrency(UserTradesKindCurrencySubscriptionParams @params, Action<UserTrade[]> callback)
  {
    return _subscriptionManager.Subscribe(
      @params,
      n => callback(n.Data.ToObject<UserTrade[]>()));
  }

#endregion
}
