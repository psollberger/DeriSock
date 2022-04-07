namespace DeriSock.JsonRpc;

using System;
using System.Threading.Tasks;

/// <inheritdoc />
/// <summary>
///   Defines the JsonRpcClient interface
/// </summary>
public interface IJsonRpcClient : IWebSocketStateInfo
{
  /// <summary>
  ///   The <see cref="Uri" /> the WebSocket connects to
  /// </summary>
  Uri ServerUri { get; }
  
  /// <summary>
  ///   Occurs when a WebSocket connection is connected
  /// </summary>
  event EventHandler Connected;

  /// <summary>
  ///   Occurs when a WebSocket connection is disconnected
  /// </summary>
  event EventHandler<JsonRpcDisconnectEventArgs> Disconnected;

  /// <summary>
  ///   Occurs when a Request on the WebSocket is received
  /// </summary>
  event EventHandler<JsonRpcRequest> RequestReceived;

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
