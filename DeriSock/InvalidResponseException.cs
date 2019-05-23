namespace DeriSock
{
  using System;
  using Model;

  public class InvalidResponseException : Exception
  {
    public JsonRpcResponse Response { get; }

    public JsonRpcError Error { get; }
    public override string Message { get; }
    public InvalidResponseException(JsonRpcResponse response, JsonRpcError error, string message)
    {
      Response = response;
      Error = error;
      Message = message;
    }
  }
}
