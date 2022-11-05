namespace DeriSock.Tests.Integration;

using Microsoft.Extensions.Configuration;

internal static class AppConfig
{
  public static IConfigurationRoot GetConfigurationRoot()
  {
    return new ConfigurationBuilder()
      .SetBasePath(Environment.CurrentDirectory)
      .AddJsonFile("appsettings.json", optional: true)
      .AddUserSecrets<DeribitConfiguration>(optional: true)
      .Build();
  }

  public static DeribitConfiguration GetDeribitConfiguration()
  {
    var configRoot = GetConfigurationRoot();
    return configRoot.GetSection("Deribit").Get<DeribitConfiguration>();
  }
}
