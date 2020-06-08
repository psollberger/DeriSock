namespace DeriSock
{
  using System.Collections.Generic;
  using System.Threading.Tasks;

  public class SubscriptionEntry
  {
    public List<SubscriptionCallback> Callbacks { get; } = new List<SubscriptionCallback>();
    public Task<SubscriptionToken> SubscribeTask { get; set; } = null;
    public Task<bool> UnsubscribeTask { get; set; } = null;
    public SubscriptionState State { get; set; } = SubscriptionState.Unsubscribed;
  }
}
