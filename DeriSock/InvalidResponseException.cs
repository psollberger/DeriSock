namespace DeriSock
{
  using System;
  using DeriSock.JsonRpc;

  public class InvalidResponseException : Exception
  {
    public JsonRpcResponse JsonRpcResponse { get; }
    public JsonRpcError JsonRpcError => JsonRpcResponse.JsonRpcError;
    public override string Message { get; }

    public InvalidResponseException(JsonRpcResponse jsonRpcResponse, string message)
    {
      JsonRpcResponse = jsonRpcResponse;
      Message = message;
    }
  }
}
