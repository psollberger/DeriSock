using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.JsonRpc;
using DeriSock.Model;

namespace DeriSock;

public partial class DeribitClient : IAuthenticationApi
{
  IAuthenticationGrantTypes IAuthenticationApi.Login()
    => this;

  /// <inheritdoc />
  Task<JsonRpcResponse<PublicExchangeTokenResponse>> IAuthenticationApi.ExchangeToken(string refreshToken, int subjectId)
    => PublicExchangeToken(refreshToken, subjectId);

  /// <inheritdoc />
  Task<JsonRpcResponse<PublicForkTokenResponse>> IAuthenticationApi.ForkToken(string refreshToken, string sessionName)
    => PublicForkToken(refreshToken, sessionName);

  /// <inheritdoc />
  bool IAuthenticationApi.Logout(bool? invalidateToken)
    => PrivateLogout(invalidateToken);
}
