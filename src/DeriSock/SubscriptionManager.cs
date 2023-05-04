namespace DeriSock;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DeriSock.Converter;
using DeriSock.Model;
using Serilog;

internal class SubscriptionManager
{
  private readonly DeribitClient _client;

  // ReSharper disable once NotAccessedField.Local
  private readonly ILogger? _logger;
  private readonly IList<StreamInfo> _streamMap;

  public SubscriptionManager(DeribitClient client, ILogger? logger)
  {
    _client = client;
    _logger = logger;
    _streamMap = new List<StreamInfo>();
  }

  public void NotifyStreams(INotification<object> notification)
  {
    lock (_streamMap)
    {
      foreach (var stream in GetChannelStreams(notification.Channel))
      {
        stream.Queue.Enqueue(notification);
        if (stream is NotificationStream<object> notificationStream)
        {
          notificationStream.SignalNewItem();
        }
      }
    }
  }

  public async Task<NotificationStream<TNotification>> Subscribe<TNotification, TParams>(params TParams[] channels)
    where TNotification : class where TParams : ISubscriptionChannel
  {
    var channelNames = channels.Select(x => x.ToChannelName()).ToArray();

    var streamInfo = new StreamInfo(new NotificationStream<TNotification>(this), channelNames);

    lock (_streamMap)
      _streamMap.Add(streamInfo);

    var subscriptionScope = _client.IsAuthenticated ? "private" : "public";

    var response = await _client.Send(
      $"{subscriptionScope}/subscribe",
      new {channels = channelNames},
      new ListJsonConverter<string>()
    ).ConfigureAwait(false);

    // Updating the list of subscribed channels in the map entry
    if (response.Data is not null)
      streamInfo.Channels = response.Data.ToArray();

    return (NotificationStream<TNotification>) streamInfo.Stream;
  }

  public async Task Unsubscribe(INotificationStream stream)
  {
    StreamInfo? streamInfo;

    lock (_streamMap)
    {
      streamInfo = _streamMap.FirstOrDefault(x => ReferenceEquals(x.Stream, stream));

      if (streamInfo is null)
        return;

      _streamMap.Remove(streamInfo);
    }

    // Unsubscribing from all channels that do not have any other subscription
    var abandonedChannels = GetAbandonedChannels(streamInfo.Channels).ToArray();

    var subscriptionScope = _client.IsAuthenticated ? "private" : "public";

    if (abandonedChannels.Any())
      await _client.Send(
        $"{subscriptionScope}/unsubscribe",
        new
        {
          abandonedChannels
        },
        new ListJsonConverter<string>()
      ).ConfigureAwait(false);
  }

  private IEnumerable<INotificationStream> GetChannelStreams(string channelName)
  {
    lock (_streamMap)
      return _streamMap.Where(x => x.Channels.Contains(channelName, StringComparer.OrdinalIgnoreCase))
        .Select(x => x.Stream);
  }

  private IEnumerable<string> GetAbandonedChannels(IEnumerable<string> channels)
  {
    lock (_streamMap)
      return channels.Where(channel => _streamMap.All(streamInfo => !streamInfo.Channels.Contains(channel)));
  }

  private IEnumerable<string> GetDistinctChannels()
  {
    var channelList = new List<string>();

    lock (_streamMap)
    {
      foreach (var channels in _streamMap.Select(si => si.Channels))
        channelList.AddRange(channels);
    }

    return channelList.Distinct();
  }

  public async Task ReSubscribeAll()
  {
    lock (_streamMap)
    {
      if (_streamMap.Count < 1)
        return;
    }

    var channelNames = GetDistinctChannels().ToArray();
    var subscriptionScope = _client.IsAuthenticated ? "private" : "public";

    // Channels need to be partitioned, so that they don't exceed the 32 kB frame limit
    var nextChannelNamesIndexToProcess = 0;
    var channelNamesPartition = new List<string>(channelNames.Length);

    while (nextChannelNamesIndexToProcess < channelNames.Length)
    {
      channelNamesPartition.Clear();

      var totalChars = 0;

      while (nextChannelNamesIndexToProcess < channelNames.Length && totalChars < 30_000)
      {
        var channelName = channelNames[nextChannelNamesIndexToProcess++];
        channelNamesPartition.Add(channelName);
        totalChars += channelName.Length;
      }

      await _client.Send(
        $"{subscriptionScope}/subscribe",
        new {channels = channelNamesPartition},
        new ListJsonConverter<string>()
      ).ConfigureAwait(false);
    }
  }

  public void Clear()
  {
    lock (_streamMap)
      _streamMap.Clear();
  }

  private class StreamInfo
  {
    public INotificationStream Stream { get; }
    public string[] Channels { get; set; }

    public StreamInfo(INotificationStream stream, string[] channels)
    {
      Stream = stream;
      Channels = channels;
    }
  }
}
