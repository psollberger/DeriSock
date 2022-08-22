namespace DeriSock.Api;

// begin-snippet: example-customized-api-interface
/// <summary></summary>
public partial interface IAuthenticationApi
{
  /// <inheritdoc cref="IAuthenticationMethods" />
  public IAuthenticationMethods PublicLogin();
}
// end-snippet
