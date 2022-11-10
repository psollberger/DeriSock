namespace DeriSock.DevTools;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using Serilog;

public static class HostUtils
{
  public static IHostBuilder CreateHostBuilder(string[] args)
  {
    return Host.CreateDefaultBuilder(args)
      .ConfigureServices(
        (context, services) =>
        {
          services.AddLogging();

          services.AddSingleton(
            _ => context.Configuration.GetRequiredSection(nameof(DeribitApiKey)).Get<DeribitApiKey>()!
          );

          services.AddUseCases();

          services.AddTransient<IDevToolsConsole, DevToolsConsole>();
        }
      )
      .ConfigureLogging(
        (context, config) =>
        {
          var logger = new LoggerConfiguration().ReadFrom.Configuration(context.Configuration).CreateLogger();
          config.AddSerilog(logger);
        }
      )
      .UseConsoleLifetime();
  }
}
