namespace DeriSock.Tests.Unit;

internal static class Helpers
{
  public static async IAsyncEnumerable<string> GetStringAsyncEnumerable(string response)
  {
    await Task.Delay(30);
    yield return response;
  }

  public static async IAsyncEnumerable<string> GetStringAsyncEnumerable(Func<Task<string>> responseFunc)
  {
    yield return await responseFunc.Invoke();
  }

  public static async IAsyncEnumerable<string> GetStringAsyncEnumerable(Func<Task<(bool, Func<string>)>> responseFunc)
  {
    var more = true;

    while (more)
    {
      (more, var result) = await responseFunc.Invoke();
      yield return result.Invoke();
    }
  }
}
