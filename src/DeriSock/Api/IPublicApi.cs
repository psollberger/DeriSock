namespace DeriSock.Api;

/// <summary></summary>
public partial interface IPublicApi
{
  /// <inheritdoc cref="IAuthenticationApi.PublicLogin" />
  public IAuthenticationMethods Login();
}
