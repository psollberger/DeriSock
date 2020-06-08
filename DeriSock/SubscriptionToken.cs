namespace DeriSock
{
  using System;

  public class SubscriptionToken
  {
    public static readonly SubscriptionToken Invalid = new SubscriptionToken(Guid.Empty);

    public Guid Token { get; }

    public SubscriptionToken(Guid token)
    {
      Token = token;
    }
  }
}
