namespace DeriSock.JsonRpc
{
  using System;
  using System.Threading.Tasks;

  public interface IJsonRpcClient
  {
    event EventHandler<JsonRpcRequest> Request;

    Uri ServerUri { get; }
    bool SocketAvailable { get; }
    bool IsConnected { get; }
    bool ClosedByError { get; }
    bool ClosedByClient { get; }
    bool ClosedByHost { get; }

    /// <summary>
    ///   Connects to the server using the <see cref="JsonRpcClient.ServerUri" /> and starts processing received messages
    /// </summary>
    Task ConnectAsync();

    /// <summary>
    ///   Stops processing of received messages and disconnects from the server
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Sends a request method to the server as fire and forget
    /// </summary>
    /// <param name="method">A string containing the name of the method to be invoked. Probably only used for private/logout</param>
    /// <param name="parameters">A structured value that hold the parameter values to be used during the invocation of the method</param>
    void SendLogout(string method, object parameters);

    /// <summary>
    ///   Sends a message to the server
    /// </summary>
    /// <param name="method">A string containing the name of the method to be invoked</param>
    /// <param name="parameters">A structured value that hold the parameter values to be used during the invocation of the method</param>
    /// <returns>A Task object</returns>
    Task<JsonRpcResponse> SendAsync(string method, object parameters);
  }
}
