namespace DeriSock.Request;

public class AnnouncementsSubscriptionParams : ISubscriptionChannel
{
  public string ToChannelName()
  {
    return "announcements";
  }
}
