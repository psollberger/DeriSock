namespace DeriSock
{
  using Converter;
  using Model;
  using Newtonsoft.Json.Linq;
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Events;
  using Utils;

  public class WebSocket : WebSocketBase
  {
    public volatile string AccessToken;

    private readonly Dictionary<string, SubscriptionEntry> _eventsMap = new Dictionary<string, SubscriptionEntry>();

    public WebSocket(string hostname) : base(hostname)
    {
      EventResponseReceived += OnEventResponseReceived;
      MessageResponseReceived += OnMessageResponseReceived;
    }

    private void OnMessageResponseReceived(object sender, MessageResponseReceivedEventArgs e)
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

          _logger.Debug($"OnMessageResponseReceived task resolve {e.Response.id}");
          Task.Factory.StartNew(
                                () =>
                                {
                                  e.TaskInfo.Resolve(convertedResult);
                                }, TaskCreationOptions.LongRunning);
        }
      }
      catch (Exception ex)
      {
        _logger.Error($"ConsumeResponsesLoop Error during parsing task {ex}");
        Task.Factory.StartNew(
                              () =>
                              {
                                e.TaskInfo.Reject(ex);
                              }, TaskCreationOptions.LongRunning);
      }
    }

    private void OnEventResponseReceived(object sender, EventResponseReceivedEventArgs e)
    {
      lock (_eventsMap)
      {
        if (e.EventData.method == "heartbeat")
        {
          if (e.EventData.@params.type == "test_request")
          {
            _ = TestAsync("ok");
          }
          return;
        }
        if (!_eventsMap.TryGetValue(e.EventData.@params.channel, out var entry))
        {
          _logger.Warning($"Could not find event for: {e.EventData.@params}");
          return;
        }

        foreach (var callback in entry.Callbacks)
        {
          try
          {
            callback(e.EventData);
          }
          catch (Exception ex)
          {
            _logger.Information($"ReceiveMessageQueue Error during calling event callback {e.EventData.@params.channel} {ex}");
          }
        }
      }
    }

    public Task<TestResponse> TestAsync(string expectedResult)
    {
      return SendAsync("public/test", new { expected_result = expectedResult }, new ObjectJsonConverter<TestResponse>());
    }

    public Task<string> PingAsync()
    {
      return SendAsync("public/ping", new { }, new ObjectJsonConverter<string>());
    }

    public Task<List<string>> SubscribePublicAsync(string[] channels)
    {
      return SendAsync("public/subscribe", new { channels }, new ListJsonConverter<string>());
    }

    public Task<List<string>> UnsubscribePublicAsync(string[] channels)
    {
      return SendAsync("public/unsubscribe", new { channels }, new ListJsonConverter<string>());
    }

    public Task<List<string>> SubscribePrivateAsync(string[] channels)
    {
      return SendAsync("private/subscribe", new { channels, access_token = AccessToken }, new ListJsonConverter<string>());
    }

    public Task<List<string>> UnsubscribePrivateAsync(string[] channels)
    {
      return SendAsync("private/unsubscribe", new { channels, access_token = AccessToken }, new ListJsonConverter<string>());
    }

    public async Task<bool> ManagedSubscribeAsync(string channel, bool @private, Action<EventResponse> callback)
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
                //Logger.Log(LogSeverity.Notice, $"Already subsribed added to callbacks {channel}");
                if (callback != null)
                {
                  entry.Callbacks.Add(callback);
                }
                return true;
              }

            case SubscriptionState.Unsubscribing:
              //Logger.Log(LogSeverity.Notice, $"Unsubscribing return false {channel}");
              return false;

            case SubscriptionState.Unsubscribed:

              //Logger.Log(LogSeverity.Notice, $"Unsubscribed resubscribing {channel}");
              entry.State = SubscriptionState.Subscribing;
              defer = new TaskCompletionSource<bool>();
              entry.CurrentAction = defer.Task;
              break;
          }
        }
        else
        {
          //Logger.Log(LogSeverity.Notice, $"Not exists subscribing {channel}");
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
        //Logger.Log(LogSeverity.Notice, $"Empty defer wait for already subscribing {channel}");
        var currentAction = entry.CurrentAction;
        var result = currentAction != null && await currentAction;
        //Logger.Log(LogSeverity.Notice, $"Empty defer wait for already subscribing res {result} {channel}");
        lock (_eventsMap)
        {
          if (!result || entry.State != SubscriptionState.Subscribed)
          {
            return false;
          }

          //Logger.Log(LogSeverity.Notice, $"Empty defer adding callback {channel}");
          if (callback != null)
          {
            entry.Callbacks.Add(callback);
          }
          return true;
        }
      }
      try
      {
        //Logger.Log(LogSeverity.Notice, $"Subscribing {channel}");
        var response = !@private ? await SubscribePublicAsync(new[] { channel }) : await SubscribePrivateAsync(new[] { channel });
        if (response.Count != 1 || response[0] != channel)
        {
          //Logger.Log(LogSeverity.Notice, $"Invalid subscribe result {response} {channel}");
          defer.SetResult(false);
        }
        else
        {
          lock (_eventsMap)
          {
            //Logger.Log(LogSeverity.Notice, $"Successfully subscribed adding callback {channel}");
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

    public async Task<bool> ManagedUnsubscribeAsync(string channel, bool @private, Action<EventResponse> callback)
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
        var response = !@private ? await UnsubscribePublicAsync(new[] { channel }) : await UnsubscribePrivateAsync(new[] { channel });
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
  }
}
