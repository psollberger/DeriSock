namespace DeriSock;

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
    foreach (var stream in GetChannelStreams(notification.Channel))
      stream.Queue.Enqueue(notification);
  }

  public async Task<NotificationStream<TNotification>> Subscribe<TNotification, TParams>(params TParams[] channels) where TNotification : class where TParams : ISubscriptionChannel
  {
    var channelNames = channels.Select(x => x.ToChannelName()).ToArray();

    var streamInfo = new StreamInfo(new NotificationStream<TNotification>(this), channelNames);

    lock (_streamMap)
      _streamMap.Add(streamInfo);

    var subscriptionScope = _client.IsAuthenticated ? "private" : "public";

    var response = await _client.Send(
                     $"{subscriptionScope}/subscribe",
                     new { channels = channelNames },
                     new ListJsonConverter<string>()).ConfigureAwait(false);

    // Updating the list of subscribed channels in the map entry
    streamInfo.Channels = response.Data.ToArray();

    return (NotificationStream<TNotification>)streamInfo.Stream;
  }

  public async Task Unsubscribe(INotificationStream stream)
  {
    StreamInfo? streamInfo;

    lock (_streamMap) {
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
        new ListJsonConverter<string>()).ConfigureAwait(false);
  }

  private IEnumerable<INotificationStream> GetChannelStreams(string channelName)
  {
    lock (_streamMap)
      return _streamMap.Where(x => x.Channels.Contains(channelName)).Select(x => x.Stream);
  }

  private IEnumerable<string> GetAbandonedChannels(IEnumerable<string> channels)
  {
    lock (_streamMap)
      return channels.Where(channel => _streamMap.All(streamInfo => !streamInfo.Channels.Contains(channel)));
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
