namespace DeriSock.Api;

/// <summary></summary>
public partial interface IAuthenticationApi
{
  /// <inheritdoc cref="IAuthenticationMethods" />
  public IAuthenticationMethods PublicLogin();
}
