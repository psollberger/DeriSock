namespace DeriSock.JsonRpc
{
  using System;

  public interface IJsonRpcClientFactory
  {
    IJsonRpcClient Create(Uri serverUri);
  }
}
