namespace DeriSock.Request;

public class PlatformStateSubscriptionParams : ISubscriptionChannel
{
  public string ToChannelName()
    => "platform_state";
}
