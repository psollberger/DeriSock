namespace DeriSock;

using DeriSock.Api;

public partial class DeribitClient
{
  private sealed partial class PublicApiImpl
  {
    /// <inheritdoc cref="IAuthenticationApi.PublicLogin" />
    IAuthenticationMethods IPublicApi.Login()
      => _client;
  }
}
