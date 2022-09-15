namespace DeriSock;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Model;

/// <summary>
///   Represents a stream of notifications sent from the endpoint.
/// </summary>
public interface INotificationStream
{
  /// <summary>
  ///   The <see cref="ConcurrentQueue{T}" /> holding the notification data objects.
  /// </summary>
  ConcurrentQueue<INotification<object>> Queue { get; }
}

/// <inheritdoc cref="INotificationStream" />
/// <typeparam name="T">The data type of the notification being sent in this stream.</typeparam>
public class NotificationStream<T> : INotificationStream, IAsyncEnumerable<Notification<T>>, IAsyncEnumerator<Notification<T>> where T : class
{
  private readonly SubscriptionManager _manager;
  private CancellationToken _cancellationToken;
  private Notification<T>? _current;

  /// <inheritdoc />
  Notification<T> IAsyncEnumerator<Notification<T>>.Current => _current!;

  /// <inheritdoc />
  ConcurrentQueue<INotification<object>> INotificationStream.Queue { get; } = new();

  internal NotificationStream(SubscriptionManager manager)
  {
    _manager = manager;
  }

  /// <inheritdoc />
  async ValueTask<bool> IAsyncEnumerator<Notification<T>>.MoveNextAsync()
  {
    try {
      while (!_cancellationToken.IsCancellationRequested) {
        if (((INotificationStream)this).Queue.TryDequeue(out var next)) {
          _current = next.ConvertTo<T>();

          if (_cancellationToken.IsCancellationRequested)
            return false;

          return true;
        }

        await Task.Delay(1, _cancellationToken).ConfigureAwait(false);
      }

      return false;
    }
    catch (TaskCanceledException) {
      return false;
    }
  }

  /// <inheritdoc />
  public IAsyncEnumerator<Notification<T>> GetAsyncEnumerator(CancellationToken cancellationToken = default)
  {
    _cancellationToken = cancellationToken;
    return this;
  }

  /// <inheritdoc />
  public async ValueTask DisposeAsync()
  {
    await _manager.Unsubscribe(this).ConfigureAwait(false);
  }
}
