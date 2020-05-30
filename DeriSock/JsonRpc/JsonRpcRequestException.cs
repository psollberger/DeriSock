namespace DeriSock.JsonRpc
{
  using System;

  public class JsonRpcRequestException : Exception
  {
    public JsonRpcRequest Request { get; }
    public JsonRpcResponse Response { get; }

    public JsonRpcError Error => Response.Error;
    public int Code => Response.Error.Code;
    public override string Message => Response.Error.Message;

    public JsonRpcRequestException(JsonRpcRequest request, JsonRpcResponse response)
    {
      Request = request;
      Response = response;
    }
  }
}
