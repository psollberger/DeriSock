namespace DeriSock
{
  using System;
  using DeriSock.Model;

  public class SubscriptionCallback
  {
    public Action<Notification> Action { get; }
    public SubscriptionToken Token { get; }

    public SubscriptionCallback(SubscriptionToken token, Action<Notification> action)
    {
      Token = token;
      Action = action;
    }
  }
}
