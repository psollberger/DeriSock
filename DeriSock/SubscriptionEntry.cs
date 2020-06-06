namespace DeriSock
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using DeriSock.Model;

  public class SubscriptionEntry
  {
    public List<Action<Notification>> Callbacks;
    public Task<bool> CurrentAction;
    public SubscriptionState State;
  }
}
