namespace DeriSock.Net.JsonRpc;

using System.Collections.Concurrent;
using System.Threading.Tasks;

/// <summary>
///   The <see cref="JsonRpcRequestTaskSourceMap" /> is used to keep track of sent requests and the received responses.
/// </summary>
public class JsonRpcRequestTaskSourceMap
{
  private readonly ConcurrentDictionary<int, JsonRpcRequest> _requestObjects;
  private readonly ConcurrentDictionary<int, TaskCompletionSource<JsonRpcResponse>> _taskSources;

  /// <summary>
  ///   Initializes a new instance of the <see cref="JsonRpcRequestTaskSourceMap" /> class.
  /// </summary>
  public JsonRpcRequestTaskSourceMap()
  {
    _taskSources = new ConcurrentDictionary<int, TaskCompletionSource<JsonRpcResponse>>();
    _requestObjects = new ConcurrentDictionary<int, JsonRpcRequest>();
  }

  /// <summary>
  ///   Adds a <see cref="JsonRpcRequest" /> with it's <see cref="TaskCompletionSource{TResult}" /> to the map.
  /// </summary>
  /// <param name="request">The <see cref="JsonRpcRequest" /> that gets added to the map.</param>
  /// <param name="taskSource">The <see cref="TaskCompletionSource{TResult}" /> that belongs to the <see cref="JsonRpcRequest" />.</param>
  public void Add(JsonRpcRequest request, TaskCompletionSource<JsonRpcResponse> taskSource)
  {
    _taskSources[request.Id] = taskSource;
    _requestObjects[request.Id] = request;
  }

  /// <summary>
  ///   Tries to retrieve a <see cref="JsonRpcRequest" /> by id.
  /// </summary>
  /// <param name="id">The id for which a <see cref="JsonRpcRequest" /> should be retrieved.</param>
  /// <param name="request">Contains the <see cref="JsonRpcRequest" /> object if the id was found.</param>
  /// <param name="taskSource">Contains the <see cref="TaskCompletionSource{TResult}" /> if the id was found.</param>
  /// <returns><c>true</c> if the id was found, <c>false</c> otherwise.</returns>
  public bool TryRemove(int id, out JsonRpcRequest request, out TaskCompletionSource<JsonRpcResponse>? taskSource)
  {
    taskSource = null;
    return _requestObjects.TryRemove(id, out request!) && _taskSources.TryRemove(id, out taskSource);
  }

  /// <summary>
  ///   Clears all entries from the map.
  /// </summary>
  public void Clear()
  {
    _taskSources.Clear();
    _requestObjects.Clear();
  }
}
