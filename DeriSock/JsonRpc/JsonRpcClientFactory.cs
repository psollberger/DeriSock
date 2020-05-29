namespace DeriSock.JsonRpc
{
  using System;

  public static class JsonRpcClientFactory
  {
    private static IJsonRpcClientFactory _factory;

    public static void Register(IJsonRpcClientFactory factory)
    {
      _factory = factory;
    }

    public static IJsonRpcClient Create(Uri serverUri)
    {
      return _factory != null ? _factory.Create(serverUri) : new JsonRpcClient(serverUri);
    }
  }
}
