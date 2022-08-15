namespace DeriSock.Api;

using System.Threading.Tasks;

using DeriSock.JsonRpc;
using DeriSock.Model;

/// <summary>
///   Access to all public API methods
/// </summary>
public interface IPublicApi
{
  /// <inheritdoc cref="IAuthenticationApi.Login" />
  public IAuthenticationGrantTypes Login();

  /// <inheritdoc cref="IAuthenticationApi.ExchangeToken" />
  public Task<JsonRpcResponse<PublicExchangeTokenResponse>> ExchangeToken(string refreshToken, int subjectId);

  /// <inheritdoc cref="IAuthenticationApi.ForkToken" />
  public Task<JsonRpcResponse<PublicForkTokenResponse>> ForkToken(string refreshToken, string sessionName);
}
