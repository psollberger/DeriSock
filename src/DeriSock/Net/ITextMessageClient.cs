namespace DeriSock.Net;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

/// <inheritdoc />
/// <summary>
///   <see cref="ITextMessageClient" /> defines the contract for a JSON-RPC message source.
/// </summary>
public interface ITextMessageClient : IAsyncDisposable
{
  /// <summary>
  ///   Gets, if the <see cref="ITextMessageClient" /> is connected with the endpoint and ready to operate.
  /// </summary>
  public bool IsConnected { get; }

  /// <summary>
  ///   Connects to the endpoint.
  /// </summary>
  /// <param name="endpoint">The endpoint to connect to.</param>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the operation should be canceled.</param>
  /// <returns>The task object representing the asynchronous operation.</returns>
  Task Connect(Uri endpoint, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Disconnects from the endpoint.
  /// </summary>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the operation should be canceled.</param>
  /// <returns>The task object representing the asynchronous operation.</returns>
  Task Disconnect(CancellationToken cancellationToken);

  /// <summary>
  ///   Sends a message to the endpoint.
  /// </summary>
  /// <param name="message">The pre-built message that will be sent to the endpoint.</param>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the operation should be canceled.</param>
  /// <returns>The task object representing the asynchronous operation.</returns>
  Task Send(string message, CancellationToken cancellationToken = default);

  /// <summary>
  ///   Gets an asynchroneous stream of messages as they arrive from the endpoint.
  /// </summary>
  /// <param name="cancellationToken">A cancellation token used to propagate notification that the operation should be canceled.</param>
  /// <returns>The asynchroneous stream of messages.</returns>
  IAsyncEnumerable<string> GetMessageStream(CancellationToken cancellationToken = default);
}
