namespace DeriSock.Utils
{
  using System.Threading;

  public class RequestIdGenerator
  {
    private readonly object _idLock = new object();
    private volatile int _lastId;

    public int Next()
    {
      lock (_idLock)
      {
        Interlocked.CompareExchange(ref _lastId, 0, int.MaxValue);
        return ++_lastId;
      }
    }
  }
}
