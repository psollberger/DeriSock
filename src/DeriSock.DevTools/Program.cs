namespace DeriSock.DevTools;

using System;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;

internal static class Program
{
  public static async Task Main(string[] args)
  {
    using var host = HostUtils.CreateHostBuilder(args).Build();
    var console = host.Services.GetRequiredService<IDevToolsConsole>();

    Console.WriteLine("Press return key to cancel");

    var cts = new CancellationTokenSource();

    var consoleTask = console.Run(args, cts.Token);
    var readLineTask = Task.Run(Console.ReadLine, cts.Token);

    var finishThroughReadLine = Task.WaitAny(readLineTask, consoleTask) == 0;
    cts.Cancel();

    if (finishThroughReadLine)
      await consoleTask;
  }
}
