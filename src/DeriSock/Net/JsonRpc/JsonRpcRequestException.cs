namespace DeriSock.Net.JsonRpc;

using System;

/// <summary>
///   Represents an error during JSON-RPC handling.
/// </summary>
public class JsonRpcRequestException : Exception
{
  /// <summary>
  ///   The request that was sent to the endpoint.
  /// </summary>
  public JsonRpcRequest Request { get; }

  /// <summary>
  ///   The response that was received from the endpoint.
  /// </summary>
  public JsonRpcResponse Response { get; }

  /// <summary>
  ///   The error object, if the server returned an error. Can be null.
  /// </summary>
  public JsonRpcError? Error => Response.Error;

  /// <summary>
  ///   Initializes a new instance of the <see cref="JsonRpcRequestException" /> class.
  /// </summary>
  /// <param name="request">The request object.</param>
  /// <param name="response">The response object.</param>
  public JsonRpcRequestException(JsonRpcRequest request, JsonRpcResponse response)
  {
    Request = request;
    Response = response;
  }
}
