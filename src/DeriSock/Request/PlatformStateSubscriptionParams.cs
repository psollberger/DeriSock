namespace DeriSock.Request;

public class PlatformStateSubscriptionParams : ISubscriptionChannel
{
  public string ToChannelName()
  {
    return "platform_state";
  }
}
