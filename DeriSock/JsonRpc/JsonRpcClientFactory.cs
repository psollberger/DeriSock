namespace DeriSock.JsonRpc;

using System;
using Serilog;

/// <summary>
///   Used to inject a custom <see cref="IJsonRpcClientFactory" />
/// </summary>
public static class JsonRpcClientFactory
{
  private static IJsonRpcClientFactory _factory;

  /// <summary>
  ///   Inject a custom <see cref="IJsonRpcClientFactory" />
  /// </summary>
  /// <param name="factory"></param>
  public static void Register(IJsonRpcClientFactory factory)
  {
    _factory = factory;
  }

  /// <summary>
  ///   Create an <see cref="IJsonRpcClient" /> instance
  /// </summary>
  /// <param name="serverUri">The <see cref="Uri" /> to connect to</param>
  /// <param name="logger">The <see cref="ILogger" /> instance to be used for logging</param>
  /// <returns>An <see cref="IJsonRpcClient" /> instance</returns>
  public static IJsonRpcClient Create(Uri serverUri, ILogger logger)
  {
    return _factory != null ? _factory.Create(serverUri) : new JsonRpcClient(serverUri, logger);
  }
}
