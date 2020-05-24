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
                   .Destructure.ByTransforming<Request>(JsonConvert.SerializeObject)
                   .Destructure.ByTransforming<Response>(JsonConvert.SerializeObject)
                   .Destructure.ByTransforming<Notification>(JsonConvert.SerializeObject)
                   .CreateLogger();

      _client = new DeribitV2Client("test.deribit.com");

      while (!_client.IsConnected)
      {
        await _client.ConnectAsync();
        await _client.SendAsync("public/set_heartbeat", new { interval = 30 }, new ObjectJsonConverter<object>());
        await _client.SendAsync("public/test", new { expected_result = "MyTest" }, new ObjectJsonConverter<TestResponse>());

        // Register for order book changes
        if (!await _client.PublicSubscribeBookAsync("BTC-PERPETUAL", 0, 1, HandleBookResponse))
        {
          Log.Logger.Fatal("Could not subscribe to the orderbook!");
        }

        try
        {
          //var loginRes = await _client.PublicAuthAsync("KxEneYNT9VsK", "S3EL63RBXOJZSN4ACV5SWF2OLO337BKL", "Playground");
        }
        catch (Exception ex)
        {
          var bla = 4;
        }

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
