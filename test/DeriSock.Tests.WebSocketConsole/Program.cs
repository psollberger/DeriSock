namespace DeriSock.Tests.WebSocketConsole;

using System.Net;
using System.Runtime.InteropServices.ComTypes;

using DeriSock.Tests.WebSocketConsole.DeribitApi;

using Microsoft.VisualStudio.Threading;

using StreamJsonRpc;

public static class Program
{
  private static readonly HttpListener HttpListener = new();

  public static async Task Main(string[] args)
  {
    var cts = new CancellationTokenSource();
    HttpListener.Prefixes.Add("http://localhost:8088/");
    HttpListener.Start();
    var receiveTask = ReceiveConnectionAsync(cts.Token);

    Console.WriteLine($"Listening on: {HttpListener.Prefixes}");
    Console.ReadLine();

    cts.Cancel();
    await receiveTask;
  }

  private static async Task ReceiveConnectionAsync(CancellationToken cancellationToken)
  {
    while (!cancellationToken.IsCancellationRequested)
    {
      var context = await HttpListener.GetContextAsync().WithCancellation(cancellationToken);

      if (!context.Request.IsWebSocketRequest)
        continue;

      var webSocketContext = await context.AcceptWebSocketAsync(null);

      // See: https://github.com/microsoft/vs-streamjsonrpc/blob/main/doc/index.md
      var handler = new WebSocketMessageHandler(webSocketContext.WebSocket);
      var rpc = new JsonRpc(handler);

      var targetOptions = new JsonRpcTargetOptions
      {
        ClientRequiresNamedArguments = true,
        UseSingleObjectParameterDeserialization = true,
        DisposeOnDisconnect = true,
        MethodNameTransform = clrMethodName => clrMethodName.ToLowerInvariant()
      };

      rpc.AddLocalRpcTarget(new Supporting(), targetOptions);

      rpc.StartListening();
      await rpc.Completion;
    }
  }
}
