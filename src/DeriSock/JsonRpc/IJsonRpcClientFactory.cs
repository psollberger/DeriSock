namespace DeriSock.JsonRpc;

using System;

/// <summary>
///   Used to create a custom <see cref="IJsonRpcClient" /> instance
/// </summary>
public interface IJsonRpcClientFactory
{
  /// <summary>
  ///   Creates the <see cref="IJsonRpcClient" /> instance
  /// </summary>
  /// <param name="serverUri">The <see cref="Uri" /> to connect to</param>
  /// <returns>An <see cref="IJsonRpcClient" /> instance</returns>
  IJsonRpcClient Create(Uri serverUri);
}
