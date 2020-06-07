namespace DeriSock.JsonRpc
{
  using System;
  using System.Net.WebSockets;

  public class JsonRpcDisconnectEventArgs
  {
    public WebSocketCloseStatus CloseStatus { get; }
    public string CloseStatusDescription { get; }
    public Exception Exception { get; }

    public JsonRpcDisconnectEventArgs(WebSocketCloseStatus closeStatus, string closeStatusDescription, Exception exception)
    {
      CloseStatus = closeStatus;
      CloseStatusDescription = closeStatusDescription;
      Exception = exception;
    }
  }
}
