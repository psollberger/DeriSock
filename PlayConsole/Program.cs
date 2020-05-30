using System;
using System.Diagnostics;
using Microsoft.OpenApi.Models;

[assembly: DebuggerDisplay(@"\{Name = {Name} Description = {Description}}", Target = typeof(OpenApiTag))]

namespace PlayConsole
{
  using System;
  using System.IO;
  using System.Net.Http;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock;
  using DeriSock.Converter;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using Microsoft.OpenApi.Readers;
  using Newtonsoft.Json;
  using Serilog;
  using Serilog.Events;

  public static class Program
  {
    private static DeribitV2Client _client;

    public static async Task<int> Main(string[] args)
    {
      Console.CancelKeyPress += Console_CancelKeyPress;

      Console.Title = "Deribit Development Playground";

      Directory.CreateDirectory(@"D:\Temp\Serilog");
      const string logFilePath = @"D:\Temp\Serilog\test-log-.txt";

      //const string outputTemplateLongLevelName = "{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} [{Level,-11:u}] {Message:lj}{NewLine}{Exception}";
      const string outputTemplateShortLevelName = "{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

      Log.Logger = new LoggerConfiguration()
                   .MinimumLevel.Verbose()
                   //.WriteTo.Async(l => l.Trace(outputTemplate: outputTemplateShortLevelName))
                   .WriteTo.Async(l => l.Console(outputTemplate: outputTemplateShortLevelName, restrictedToMinimumLevel: LogEventLevel.Information))
                   //.WriteTo.Async(l => l.File(logFilePath, rollingInterval: RollingInterval.Day, restrictedToMinimumLevel: LogEventLevel.Verbose))
                   .Destructure.ByTransforming<JsonRpcRequest>(JsonConvert.SerializeObject)
                   .Destructure.ByTransforming<JsonRpcResponse>(JsonConvert.SerializeObject)
                   .Destructure.ByTransforming<Notification>(JsonConvert.SerializeObject)
                   .Destructure.ByTransforming<Heartbeat>(JsonConvert.SerializeObject)
                   .CreateLogger();

      _client = new DeribitV2Client(DeribitEndpointType.Testnet);

      while (!_client.IsConnected)
      {
        await _client.ConnectAsync();

        var res = await _client.PublicTest("arigato senpai");

        await _client.DisconnectAsync();

        while (_client.IsConnected)
        {
          //try
          //{
          //  var accSum = await _client.PrivateGetAccountSummaryAsync();
          //  Log.Logger.Information("Got Account Summary");
          //}
          //catch (Exception ex)
          //{
          //  var blub = 4;
          //}
          //Thread.Sleep(TimeSpan.FromSeconds(5));
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
            break;
          }
        }
      }

      return 0;
    }

    private static void HandleBookResponse(BookResponse obj)
    {
      Log.Logger.Information($"HandleBookResponse: {obj}");
    }

    private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
      if (_client == null) return;
      var _ = _client.DisconnectAsync();
      e.Cancel = true;
    }
  }
}
