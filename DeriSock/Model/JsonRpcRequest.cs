namespace DeriSock.Model
{
  public class JsonRpcRequest
  {
    public string jsonrpc;
    public int id;
    public string method;
    public object @params;
  }
}
