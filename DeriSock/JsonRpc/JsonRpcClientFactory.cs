namespace DeriSock.JsonRpc
{
  using System;
  using Serilog;

  public static class JsonRpcClientFactory
  {
    private static IJsonRpcClientFactory _factory;

    public static void Register(IJsonRpcClientFactory factory)
    {
      _factory = factory;
    }

    public static IJsonRpcClient Create(Uri serverUri, ILogger logger)
    {
      return _factory != null ? _factory.Create(serverUri) : new JsonRpcClient(serverUri, logger);
    }
  }
}
