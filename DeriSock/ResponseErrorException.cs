namespace DeriSock
{
  using System;
  using DeriSock.JsonRpc;

  public class ResponseErrorException : Exception
  {
    public JsonRpcResponse Response { get; }
    public JsonRpcError Error => Response.Error;
    public override string Message { get; }

    public ResponseErrorException(JsonRpcResponse response, string message)
    {
      Response = response;
      Message = message;
    }
  }
}
