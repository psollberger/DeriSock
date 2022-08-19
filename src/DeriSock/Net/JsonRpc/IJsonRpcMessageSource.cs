namespace DeriSock.Net.JsonRpc;

using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

/// <inheritdoc />
/// <summary>
///   <see cref="IJsonRpcMessageSource" /> defines the contract for a JSON-RPC message source.
/// </summary>
public interface IJsonRpcMessageSource : IAsyncDisposable
{
  /// <summary>
  ///   Gets the WebSocket state.
  /// </summary>
  public WebSocketState State { get; }

  /// <summary>
  ///   The WebSocket close status.
  /// </summary>
  public WebSocketCloseStatus? CloseStatus { get; }

  /// <summary>
  ///   The close status description.
  /// </summary>
  public string? CloseStatusDescription { get; }

  /// <summary>
  ///   In case of an error, the exception that occured.
  /// </summary>
  public Exception? Exception { get; }

  /// <summary>
  ///   Connects to the provided endpoint.
  /// </summary>
  /// <param name="endpoint">The endpoint with wich JSON-RPC messages are exchanged.</param>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the  operation should be canceled.</param>
  /// <returns>The task object representing the asynchronous operation.</returns>
  Task Connect(Uri endpoint, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Disconnects from the endpoint.
  /// </summary>
  /// <param name="closeStatus">The WebSocket close status.</param>
  /// <param name="closeStatusDescription">A description of the close status.</param>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the  operation should be canceled.</param>
  /// <returns>The task object representing the asynchronous operation.</returns>
  Task Disconnect(WebSocketCloseStatus? closeStatus, string? closeStatusDescription, CancellationToken cancellationToken);

  /// <summary>
  ///   Sends a JSON-RPC message to the underlying endpoint.
  /// </summary>
  /// <param name="message">The pre-built message that will be sent to the endpoint.</param>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the  operation should be canceled.</param>
  /// <returns>The task object representing the asynchronous operation.</returns>
  Task Send(string message, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Gets an asynchroneous stream that returns messages as they arrive from the endpoint.
  /// </summary>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the  operation should be canceled.</param>
  /// <returns>The asynchroneous stream of messages.</returns>
  IAsyncEnumerable<string> GetMessageStream(CancellationToken cancellationToken = default);
}
