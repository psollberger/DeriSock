namespace DeriSock.Api;

/// <summary>
///   Access to all private API methods
/// </summary>
public interface IPrivateApi
{
  /// <inheritdoc cref="IAuthenticationApi.Logout" />
  public bool Logout(bool? invalidateToken = null);
}
