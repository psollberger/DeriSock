namespace DeriSock.Constants;

using System;

public static class Endpoint
{
  public static readonly Uri Productive = new("wss://www.deribit.com/ws/api/v2");
  public static readonly Uri TestNet = new("wss://test.deribit.com/ws/api/v2");
}
