namespace DeriSock;

using System;

public class SubscriptionToken
{
  public static readonly SubscriptionToken Invalid = new(Guid.Empty);

  public Guid Token { get; }

  public SubscriptionToken(Guid token)
  {
    Token = token;
  }
}
