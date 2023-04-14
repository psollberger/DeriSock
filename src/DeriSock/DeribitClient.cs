// ReSharper disable UnusedMember.Local
// ReSharper disable InheritdocConsiderUsage

// ReSharper disable MemberCanBePrivate.Global

namespace DeriSock;

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using DeriSock.Api;
using DeriSock.Constants;
using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net;
using DeriSock.Net.JsonRpc;
using DeriSock.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;

/// <summary>
///   <para>The implementation of the API methods from Deribit</para>
///   <para>The methods are organized by category and scope. For example: <see cref="Public" /> hold all public methods and <see cref="Private" /> holds all private methods.</para>
/// </summary>
public partial class DeribitClient : IDisposable
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
  ///   Occurs after client disconnected from the endpoint.
  /// </summary>
  public event EventHandler? Disconnected;

  /// <summary>
  ///   Occurs when the connection is lost unexpectedly.
  /// </summary>
  public event EventHandler? ConnectionLost;

  /// <summary>
  ///   Occurs when an unexpectedly lost connection is restored.
  /// </summary>
  public event EventHandler? ConnectionRestored;

  /// <summary>
  ///   Occurs when an unexpectedly lost connection.
  /// </summary>
  public event EventHandler? ConnectionFailed;

  /// <summary>
  /// Gets or sets, how much time must have been passed before trying to connect again.
  /// </summary>
  public TimeSpan ReConnectDelay { get; set; } = TimeSpan.FromSeconds(1);

  /// <summary>
  /// Gets or sets, how much the <see cref="ReConnectDelay"/> will be increased on each re-connect attempt.
  /// </summary>
  public double ReConnectDelayIncreaseFactor { get; set; } = 2.0;

  /// <summary>
  /// Gets or sets, how many times a re-connect will be tried before giving up.
  /// </summary>
  public int ReConnectMaxAttempts { get; set; } = 5;

  private readonly ILogger? _logger;
  private readonly JsonRpcRequestTaskSourceMap _requestTaskSourceMap = new();
  private readonly SubscriptionManager _subscriptionManager;

  private readonly Uri _endpointUri;
  private readonly ITextMessageClient _messageSource;
  private readonly SemaphoreSlim _sendLock = new(1, 1);
  private readonly SemaphoreSlim _reconnectLock = new(1, 1);

  private CancellationTokenSource? _processMessageStreamCts;
  private Task? _processMessageStreamTask;
  private readonly ManualResetEventSlim _processMessageTaskStartedEvent = new();

  private PublicAuthRequest? _authRequest;
  private SignatureData? _authRequestSignatureData;

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
  public bool IsConnected => _messageSource.IsConnected;

  /// <summary>
  ///   Gets if the connection is authenticated (i.e. private methods can be called)
  /// </summary>
  public bool IsAuthenticated => !string.IsNullOrEmpty(AccessToken) && !string.IsNullOrEmpty(RefreshToken);

  /// <summary>
  ///   Creates a new <see cref="DeribitClient" /> instance.
  /// </summary>
  /// <param name="endpointType">The endpoint type.</param>
  /// <param name="messageSource">The message source. <c>null</c> to use <see cref="TextMessageWebSocketClient" /></param>
  /// <param name="logger">Optional implementation of the <see cref="ILogger" /> interface to enable logging capabilities.</param>
  /// <exception cref="ArgumentOutOfRangeException">Throws an exception if the <paramref name="endpointType" /> is not supported.</exception>
  public DeribitClient(EndpointType endpointType, ITextMessageClient? messageSource = null, ILogger? logger = null)
  {
    _endpointUri = endpointType switch
    {
      EndpointType.Productive => Endpoint.Productive,
      EndpointType.Testnet => Endpoint.TestNet,
      _ => throw new ArgumentOutOfRangeException(nameof(endpointType), endpointType, "Unsupported endpoint type")
    };

    _messageSource = messageSource ?? new TextMessageWebSocketClient(logger);
    _logger = logger;
    _subscriptionManager = new SubscriptionManager(this, _logger);
  }

  /// <summary>
  ///   Connects to the endpoint.
  /// </summary>
  /// <returns>The task object representing the asynchronous operation.</returns>
  public async Task Connect(CancellationToken cancellationToken = default)
  {
    _authRequest = null;
    _authRequestSignatureData = null;
    AccessToken = null;
    RefreshToken = null;

    _subscriptionManager.Clear();

    await _messageSource.Connect(_endpointUri, cancellationToken).ConfigureAwait(false);

    _processMessageStreamCts = new CancellationTokenSource();

    _processMessageTaskStartedEvent.Reset();
    _processMessageStreamTask =
      Task.Factory.StartNew(
        async () => await ProcessMessageStream(_processMessageStreamCts.Token).ConfigureAwait(false),
        TaskCreationOptions.LongRunning);
    _processMessageTaskStartedEvent.Wait(cancellationToken);

    Connected?.Invoke(this, EventArgs.Empty);
  }


  /// <summary>
  ///   Disconnects from the server
  /// </summary>
  /// <returns>The task object representing the asynchronous operation.</returns>
  public async Task Disconnect(CancellationToken cancellationToken = default)
  {
    if (_processMessageStreamTask is not null)
    {
      _processMessageStreamCts?.Cancel();
      await Task.WhenAll(_processMessageStreamTask).ConfigureAwait(false);
    }

    await _messageSource.Disconnect(cancellationToken).ConfigureAwait(false);

    _authRequest = null;
    _authRequestSignatureData = null;
    AccessToken = null;
    RefreshToken = null;
    Disconnected?.Invoke(this, EventArgs.Empty);
    _processMessageTaskStartedEvent.Reset();
  }


  private async Task ReConnect()
  {
    if (_reconnectLock.CurrentCount < 1)
      return;

    if (!await _reconnectLock.WaitAsync(1).ConfigureAwait(false))
      return;

    try
    {
      ConnectionLost?.Invoke(this, EventArgs.Empty);

      var currentReConnectDelayFactor = 1.0;
      var reConnectSuccess = false;
      var reConnectAttempts = 0;

      while (!reConnectSuccess && ++reConnectAttempts <= ReConnectMaxAttempts)
      {
        _requestTaskSourceMap.CancelAndRemoveAll();

        if (_processMessageStreamTask is not null)
        {
          _processMessageStreamCts?.Cancel();
          await _processMessageStreamTask.ConfigureAwait(false);
        }

        AccessToken = null;
        RefreshToken = null;

        var delay = TimeSpan.FromTicks((long) (ReConnectDelay.Ticks * currentReConnectDelayFactor));
        currentReConnectDelayFactor *= ReConnectDelayIncreaseFactor;
        await Task.Delay(delay).ConfigureAwait(false);

        try
        {
          await _messageSource.Connect(_endpointUri).ConfigureAwait(false);

          _processMessageStreamCts = new CancellationTokenSource();

          _processMessageTaskStartedEvent.Reset();
          _processMessageStreamTask =
            Task.Factory.StartNew(
              async () => await ProcessMessageStream(_processMessageStreamCts.Token).ConfigureAwait(false),
              TaskCreationOptions.LongRunning);
          _processMessageTaskStartedEvent.Wait();

          await ReAuthenticateOnReConnect().ConfigureAwait(false);
          await _subscriptionManager.ReSubscribeAll().ConfigureAwait(false);

          ConnectionRestored?.Invoke(this, EventArgs.Empty);
          reConnectSuccess = true;
        }
        catch (Exception ex)
        {
          _logger?.Error("Error during reconnect attempt: {@Exception}", ex);
        }
      }

      if (!reConnectSuccess)
      {
        ConnectionFailed?.Invoke(this, EventArgs.Empty); // Raise an event
        throw new InvalidOperationException("Failed to reconnect after maximum attempts."); // Or throw an exception
      }
    }
    finally
    {
      _reconnectLock.Release();
    }
  }


  private async Task ReAuthenticateOnReConnect()
  {
    if (_authRequest is null)
      return;

    if (_authRequest.GrantType == GrantType.ClientSignature)
    {
      if (_authRequestSignatureData is null)
      {
        _authRequest = null;
        return;
      }

      _authRequestSignatureData.Refresh();
      _authRequestSignatureData.Apply(_authRequest);
    }

    await InternalPublicAuth(_authRequest).ConfigureAwait(false);
  }

  internal async Task<JsonRpcResponse<T>> Send<T>(string method, object? @params, IJsonConverter<T> converter,
    CancellationToken cancellationToken = default)
  {
    await _sendLock.WaitAsync(cancellationToken).ConfigureAwait(false);
    var requestId = RequestIdGenerator.Next();
    try
    {
      try
      {
        var request = new JsonRpcRequest
        {
          Id = requestId,
          Method = method,
          Params = @params
        };

        _logger?.Verbose("Sending: {@Request}", request);

        var taskSource = new TaskCompletionSource<JsonRpcResponse>();
        _requestTaskSourceMap.Add(request, taskSource);
        await _messageSource
          .Send(JsonConvert.SerializeObject(request, Formatting.None, SerializationSettings), cancellationToken)
          .ConfigureAwait(false);
        var response = await taskSource.Task.ConfigureAwait(false);
        return response.CreateTyped(converter.Convert(response.Result)!);
      }
      catch (SocketException sockEx) when (sockEx.SocketErrorCode == SocketError.ConnectionReset)
      {
        // In case of a connection reset during send, try re-connecting and send the request again
        await ReConnect().ConfigureAwait(false);

        if (!IsConnected)
          throw;

        requestId = RequestIdGenerator.Next();

        var request = new JsonRpcRequest
        {
          Id = requestId,
          Method = method,
          Params = @params
        };

        _logger?.Verbose("Re-Sending: {@Request}", request);

        var taskSource = new TaskCompletionSource<JsonRpcResponse>();
        _requestTaskSourceMap.Add(request, taskSource);
        await _messageSource
          .Send(JsonConvert.SerializeObject(request, Formatting.None, SerializationSettings), cancellationToken)
          .ConfigureAwait(false);
        var response = await taskSource.Task.ConfigureAwait(false);
        return response.CreateTyped(converter.Convert(response.Result)!);
      }
    }
    catch (Exception)
    {
      _requestTaskSourceMap.TryRemove(requestId, out var _, out var _);
      throw;
    }
    finally
    {
      _sendLock.Release();
    }
  }

  private async Task SendSync(string method, object? @params)
  {
    await _sendLock.WaitAsync().ConfigureAwait(false);
    try
    {
      var request = new JsonRpcRequest
      {
        Id = RequestIdGenerator.Next(),
        Method = method,
        Params = @params
      };

      _logger?.Verbose("Sending synchronous: {@Request}", request);
      await _messageSource
        .Send(JsonConvert.SerializeObject(request, Formatting.None, SerializationSettings), CancellationToken.None)
        .ConfigureAwait(false);
    }
    finally
    {
      _sendLock.Release();
    }
  }

  private async Task ProcessMessageStream(CancellationToken cancellationToken)
  {
    _processMessageTaskStartedEvent.Set();
    try
    {
      await foreach (var message in _messageSource.GetMessageStream(cancellationToken).ConfigureAwait(false))
      {
        if (JsonConvert.DeserializeObject(message) is not JObject jObject)
          continue;

        if (jObject.TryGetValue("method", out _))
          InternalOnRequestReceived(jObject);
        else
          InternalOnResponseReceived(message, jObject);
      }
    }
    catch (TaskCanceledException)
    {
      //ignore - just exit
    }
    catch (Exception ex)
    {
      _logger?.Error(ex, "An error occurred in ProcessMessageStream");

      await ReConnect().ConfigureAwait(false);

      if (!IsConnected)
        throw;
    }
  }


  private void InternalOnRequestReceived(JObject requestObject)
  {
    var request = requestObject.ToObject<JsonRpcRequest>();
    Debug.Assert(request != null, nameof(request) + " != null");
    request!.Original = requestObject;

    if (string.Equals(request.Method, "subscription"))
      Task.Run(() => OnNotification(request.Original["params"]!.ToObject<Notification<object>>()!))
        .ConfigureAwait(false);
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

    if (response.Id <= 0)
      return;

    if (!_requestTaskSourceMap.TryRemove(response.Id, out _, out var taskSource))
    {
      _logger?.Error("Could not find request id {RequestId}", response.Id);
      return;
    }

    taskSource!.SetResult(response);
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
      async _ =>
      {
        var result = await ((IAuthenticationMethods)this).WithRefreshToken().ConfigureAwait(false);

        if (result.Data is not null)
          EnqueueAuthRefresh(result.Data.ExpiresIn);
      }, TaskContinuationOptions.RunContinuationsAsynchronously);
  }



  /// <inheritdoc />
  public void Dispose()
  {
    _reconnectLock.Dispose();
    _processMessageStreamCts?.Dispose();
    _processMessageStreamTask?.Dispose();
    _processMessageTaskStartedEvent.Dispose();
  }
}
