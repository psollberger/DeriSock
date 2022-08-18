namespace DeriSock;

using DeriSock.Api;

public partial class DeribitClient
{
  private sealed partial class AuthenticationApiImpl
  {
    /// <inheritdoc cref="IAuthenticationApi.PublicLogin" />
    IAuthenticationMethods IAuthenticationApi.PublicLogin()
      => _client;
  }
}
