namespace DeriSock;

using DeriSock.Api;

// begin-snippet: example-customized-api-interface-impl
public partial class DeribitClient
{
  private sealed partial class AuthenticationApiImpl
  {
    /// <inheritdoc cref="IAuthenticationApi.PublicLogin" />
    IAuthenticationMethods IAuthenticationApi.PublicLogin()
      => _client;
  }
}
// end-snippet
