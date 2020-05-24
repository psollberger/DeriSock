namespace DeriSock.Model
{
  public class EventResponse : JsonRpcResponse
  {
    public string jsonrpc;
    public string method;
    public EventParams @params;
  }
}
