namespace PlayConsole
{
  using System;
  using System.IO;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock;
  using DeriSock.Converter;
  using DeriSock.Model;
  using Newtonsoft.Json;
  using Serilog;
  using Serilog.Events;

  public static class Program
  {
    private static JsonRpcWebSocketClient _client;

    public static async Task<int> Main(string[] args)
    {
      Console.CancelKeyPress += Console_CancelKeyPress;

      Console.Title = "Deribit Development Playground";
      
      Directory.CreateDirectory(@"D:\Temp\Serilog");
      var logFilePath = @"D:\Temp\Serilog\test-log-.txt";

      var outputTemplateLongLevelName = "{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} [{Level,-11:u}] {Message:lj}{NewLine}{Exception}";
      var outputTemplateShortLevelName = "{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

      Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   .WriteTo.Async(l => l.Trace(outputTemplate: outputTemplateShortLevelName))
                   .WriteTo.Async(l => l.Console(outputTemplate: outputTemplateShortLevelName))
                   .WriteTo.Async(l => l.File(logFilePath, rollingInterval: RollingInterval.Hour, restrictedToMinimumLevel: LogEventLevel.Information))
                   .Destructure.ByTransforming<JsonRpcRequest>(JsonConvert.SerializeObject)
                   .Destructure.ByTransforming<JsonRpcResponse>(JsonConvert.SerializeObject)
                   .Destructure.ByTransforming<EventResponse>(JsonConvert.SerializeObject)
                   .CreateLogger();

      var canQuitProcess = false;

      while (!canQuitProcess)
      {
        _client = new JsonRpcWebSocketClient("wss://test.deribit.com/ws/api/v2");
        //_client = new JsonRpcWebSocketClient("wss://www.deribit.com/ws/api/v2");
        await _client.ConnectAsync();
        await _client.SendAsync("public/set_heartbeat", new { interval = 30 }, new ObjectJsonConverter<object>());
        await _client.SendAsync("public/test", new { expected_result = "MyTest" }, new ObjectJsonConverter<TestResponse>());

        while (_client.IsConnected)
        {
          Thread.Sleep(1);
        }

        //await client.DisconnectAsync();

        if (_client.ClosedByHost)
        {
          Log.Logger.Information("Closed by Host. Reconnect in 5s");
          //try to reconnect
          Thread.Sleep(5000);
          continue;
        }
        else if (_client.ClosedByError)
        {
          Log.Logger.Information("Closed by Error. Reconnect in 5s");
          //try to reconnect
          Thread.Sleep(5000);
          continue;
        }
        else
        {
          if (_client.ClosedByClient)
          {
            Log.Logger.Information("Closed by Client. Do not Reconnect.");
          }
          canQuitProcess = true;
        }
      }

      return 0;
    }

    private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
      if (_client == null) return;
      var _ = _client.DisconnectAsync();
      e.Cancel = true;
    }
  }
}
