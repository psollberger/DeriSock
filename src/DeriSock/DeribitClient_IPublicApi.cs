using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.JsonRpc;
using DeriSock.Model;

namespace DeriSock;

public partial class DeribitClient : IPublicApi
{
  IAuthenticationGrantTypes IPublicApi.Login()
    => this;

  /// <inheritdoc />
  Task<JsonRpcResponse<PublicExchangeTokenResponse>> IPublicApi.ExchangeToken(string refreshToken, int subjectId)
    => PublicExchangeToken(refreshToken, subjectId);

  /// <inheritdoc />
  Task<JsonRpcResponse<PublicForkTokenResponse>> IPublicApi.ForkToken(string refreshToken, string sessionName)
    => PublicForkToken(refreshToken, sessionName);
}