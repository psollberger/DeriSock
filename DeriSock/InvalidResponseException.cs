namespace DeriSock
{
  using System;
  using DeriSock.JsonRpc;

  public class InvalidResponseException : Exception
  {
    public Response Response { get; }
    public Error Error => Response.Error;
    public override string Message { get; }

    public InvalidResponseException(Response response, string message)
    {
      Response = response;
      Message = message;
    }
  }
}
