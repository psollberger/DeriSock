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
  using DeriSock.Request;
  using DeriSock.Utils;
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

      const string logFilePath = @"D:\Temp\Serilog\test-log-.txt";
      Directory.CreateDirectory(Path.GetDirectoryName(logFilePath));
      
      //const string outputTemplateLongLevelName = "{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} [{Level,-11:u}] {Message:lj}{NewLine}{Exception}";
      const string outputTemplateShortLevelName = "{Timestamp:yyyy-MM-dd HH:mm:ss.fffffff} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

      Log.Logger = new LoggerConfiguration()
        .MinimumLevel.Verbose()
        //.WriteTo.Async(l => l.Trace(outputTemplate: outputTemplateShortLevelName))
        .WriteTo.Async(l => l.Console(outputTemplate: outputTemplateShortLevelName, restrictedToMinimumLevel: LogEventLevel.Debug))
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

        //var loginRes = await _client.PublicAuthAsync("KxEneYNT9VsK", "S3EL63RBXOJZSN4ACV5SWF2OLO337BKL", "Playground");
        //var loginRes = await _client.PublicSignatureAuthAsync("KxEneYNT9VsK", "S3EL63RBXOJZSN4ACV5SWF2OLO337BKL", "Playground");
        
        var loginRes = await _client.PublicAuthAsync(new AuthRequestParams
        {
          GrantType = "client_signature",
          ClientId = "KxEneYNT9VsK",
          ClientSecret = "S3EL63RBXOJZSN4ACV5SWF2OLO337BKL",
          Scope = "expires:60"
        });

        await _client.PublicSetHeartbeatAsync(10);

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

      Log.Logger.Information("");
      Log.Logger.Information("");
      Log.Logger.Information("");
      Log.Logger.Information("Drücksch du Taschtä für fertig ...");
      Console.ReadKey();

      return 0;
    }

    private static void HandleBookResponse(BookResponse obj)
    {
      Log.Logger.Information($"HandleBookResponse: {obj}");
    }

    private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
      if (_client == null) return;
      if (!_client.PrivateLogout())
        _client.DisconnectAsync().GetAwaiter().GetResult();
      e.Cancel = true;
    }
  }
}
