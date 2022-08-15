namespace DeriSock.Api;

/// <summary>
///   Access to all API method categories
/// </summary>
public interface ICategoriesApi
{
  /// <inheritdoc cref="IAuthenticationApi" />
  public IAuthenticationApi Authentication();
}
