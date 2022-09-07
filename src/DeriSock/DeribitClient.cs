// ReSharper disable UnusedMember.Local
// ReSharper disable InheritdocConsiderUsage

namespace DeriSock;

using System;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Constants;
using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;
using DeriSock.Utils;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

using Serilog;

/// <summary>
///   <para>The implementation of the API methods from Deribit</para>
/// <para>The methods are organized by category and scope. For example: <see cref="Public"/> hold all public methods and <see cref="Private"/> holds all private methods.</para>
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

  private CancellationTokenSource? _processMessageStreamCts;
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
  ///   Gets if the connection is authenticated (i.e. private methods can be called)
  /// </summary>
  public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(RefreshToken);

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

    _subscriptionManager = new SubscriptionManager(this, _logger);
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

    _subscriptionManager.Clear();

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
      _processMessageStreamCts?.Cancel();
      await Task.WhenAll(_processMessageStreamTask);
    }

    await _messageSource.Disconnect(null, null, cancellationToken).ConfigureAwait(false);

    AccessToken = null;
    RefreshToken = null;

    Disconnected?.Invoke(this, new DeribitClientDisconnectedEventArgs(_messageSource.CloseStatus, _messageSource.CloseStatusDescription, _messageSource.Exception));
  }

  internal async Task<JsonRpcResponse<T>> Send<T>(string method, object? @params, IJsonConverter<T> converter, CancellationToken cancellationToken = default)
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
      Task.Run(() => OnNotification(request.Original["params"]!.ToObject<Notification<object>>()!)).ConfigureAwait(false);
    else if (string.Equals(request.Method, "heartbeat"))
      Task.Run(() => OnHeartbeat(request.Original["params"]!.ToObject<Heartbeat>()!)).ConfigureAwait(false);
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

  private void OnNotification(INotification<object> notification)
  {
    _logger?.Verbose("OnNotification: {@Notification}", notification);
    _subscriptionManager.NotifyStreams(notification);
  }

  private void EnqueueAuthRefresh(long expiresIn)
  {
    var expireTimeSpan = TimeSpan.FromSeconds(expiresIn);

    if (expireTimeSpan.TotalMilliseconds > int.MaxValue)
      return;

    _ = Task.Delay(expireTimeSpan.Subtract(TimeSpan.FromSeconds(5))).ContinueWith(
      _ =>
      {
        var result = ((IAuthenticationMethods)this).WithRefreshToken().GetAwaiter().GetResult();
        EnqueueAuthRefresh(result.Data.ExpiresIn);
      });
  }
}
