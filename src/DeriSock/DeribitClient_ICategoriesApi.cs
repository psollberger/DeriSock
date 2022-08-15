namespace DeriSock;

using DeriSock.Api;

public partial class DeribitClient : ICategoriesApi
{
  /// <inheritdoc cref="IAuthenticationApi" />
  public IAuthenticationApi Authentication()
    => this;
}
