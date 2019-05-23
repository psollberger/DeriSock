namespace DeriSock.Utils
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Model;

  public class SubscriptionEntry
  {
    public SubscriptionState State;
    public List<Action<EventResponse>> Callbacks;
    public Task<bool> CurrentAction;
  }
}
