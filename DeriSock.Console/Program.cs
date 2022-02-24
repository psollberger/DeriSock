namespace DeriSock.Console
{
  using System;
  using System.IO;
  using System.Net.WebSockets;
  using System.Reflection;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.Constants;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using DeriSock.Request;
  using DeriSock.Utils;
  using Microsoft.Extensions.Configuration;
  using Newtonsoft.Json;
  using Serilog;
  using Serilog.Events;

  public static class Program
  {
    private static DeribitV2Client _client;
    private static readonly ManualResetEventSlim DisconnectResetEvent = new ManualResetEventSlim(false);

    public static async Task<int> Main(string[] args)
    {
      Console.CancelKeyPress += Console_CancelKeyPress;
      Console.Title = "Deribit Development Playground";

      var confRoot = new ConfigurationBuilder()
        //.SetBasePath(Directory.GetCurrentDirectory())
        //.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
        .AddUserSecrets(Assembly.GetExecutingAssembly(), false, false)
        .Build();

      var apiSettings = confRoot.GetSection("api_master");

      var clientId = apiSettings["ClientId"];
      var clientSecret = apiSettings["ClientSecret"];

      const string logFilePath = @"C:\Temp\Serilog\test-log-.txt";
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
      _client.Connected += OnConnected;
      _client.Disconnected += OnDisconnected;

      while (_client.State != WebSocketState.Open)
      {
        Log.Logger.Debug("SocketState: {State}", _client.State);
        try
        {
          await _client.Connect();

          var sig = CryptoHelper.CreateSignature(clientSecret);
          var loginRes = await _client.PublicAuth(new AuthParams {GrantType = GrantType.Signature, ClientId = clientId, Signature = sig});
          var summary = await _client.PrivateGetAccountSummary("BTC", false);

          var instruments = await _client.PublicGetInstruments("BTC");

          var sub = _client.SubscribePerpetualInterestRate(
            new PerpetualInterestRateSubscriptionParams { Interval = "100ms", InstrumentName = "BTC" },
            notification =>
            {
              Log.Logger.Information($"IR {notification.Interest}");
            });
          var subToken = await _client.SubscribeBookChange(new BookChangeSubscriptionParams {InstrumentName = "BTC-PERPETUAL", Interval = "100ms"}, o =>
          {
            // Log.Logger.Information($"HandleNotification: {o}");
          });

          var blub = 4;
        }
        catch (Exception ex)
        {
          Log.Logger.Error(ex, "Error");
          _client.PrivateLogout();
          break;
        }

        DisconnectResetEvent.Wait();

        if (_client.CloseStatus == WebSocketCloseStatus.NormalClosure)
        {
          Log.Logger.Information("Closed by client. Do not reconnect.");
          break;
        }

        if (_client.Error != null)
        {
          Log.Logger.Information("Closed by internal error. Reconnect in 5s");
          Thread.Sleep(5000);
        }
        else
        {
          Log.Logger.Information("Closed by host. Reconnect in 5s");
          Thread.Sleep(5000);
        }
      }

      Log.Logger.Information("");
      Log.Logger.Information("");
      Log.Logger.Information("");
      Log.Logger.Information("Drücksch du Taschtä für fertig ...");
      Console.ReadKey();

      return 0;
    }

    private static void OnConnected(object sender, EventArgs e)
    {
      var client = (DeribitV2Client)sender;
      Log.Logger.Information("Client is connected ({State})", client.State);
      DisconnectResetEvent.Reset();
    }

    private static void OnDisconnected(object sender, JsonRpcDisconnectEventArgs e)
    {
      var client = (DeribitV2Client)sender;
      Log.Logger.Information("Client is disconnected ({Reason}: {Description})", client.CloseStatus, client.CloseStatusDescription);
      DisconnectResetEvent.Set();
    }

    private static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
    {
      if (_client == null)
      {
        return;
      }

      if (!_client.PrivateLogout())
      {
        _client.Disconnect().GetAwaiter().GetResult();
      }

      e.Cancel = true;
    }
  }
}
