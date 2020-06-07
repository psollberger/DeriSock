namespace DeriSock.JsonRpc
{
  using System;
  using System.Net.WebSockets;
  using System.Threading.Tasks;

  public interface IJsonRpcClient
  {
    event EventHandler Connected;
    event EventHandler<JsonRpcDisconnectEventArgs> Disconnected;
    event EventHandler<JsonRpcRequest> RequestReceived;

    Uri ServerUri { get; }

    WebSocketState State { get; }

    WebSocketCloseStatus? CloseStatus { get; }

    string CloseStatusDescription { get; }

    Exception Error { get; }

    /// <summary>
    ///   Connects to the server using the <see cref="JsonRpcClient.ServerUri" /> and starts processing received messages
    /// </summary>
    Task Connect();

    /// <summary>
    ///   Stops processing of received messages and disconnects from the server
    /// </summary>
    Task Disconnect();

    /// <summary>
    ///   Sends a request method to the server as fire and forget
    /// </summary>
    /// <param name="method">A string containing the name of the method to be invoked. Probably only used for private/logout</param>
    /// <param name="parameters">
    ///   A structured value that hold the parameter values to be used during the invocation of the
    ///   method
    /// </param>
    void SendLogoutSync(string method, object parameters);

    /// <summary>
    ///   Sends a message to the server
    /// </summary>
    /// <param name="method">A string containing the name of the method to be invoked</param>
    /// <param name="parameters">
    ///   A structured value that hold the parameter values to be used during the invocation of the
    ///   method
    /// </param>
    /// <returns>A Task object</returns>
    Task<JsonRpcResponse> Send(string method, object parameters);
  }
}
