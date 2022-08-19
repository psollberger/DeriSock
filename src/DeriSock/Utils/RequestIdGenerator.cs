namespace DeriSock.Utils;

using System.Threading;

internal static class RequestIdGenerator
{
  private static readonly object _idLock = new();
  private static volatile int _lastId;

  public static void Reset()
  {
    lock (_idLock) {
      Interlocked.Exchange(ref _lastId, 0);
    }
  }

  public static int Next()
  {
    lock (_idLock) {
      Interlocked.CompareExchange(ref _lastId, 0, int.MaxValue);
      Interlocked.Increment(ref _lastId);
      return _lastId;
    }
  }
}
