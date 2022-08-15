namespace DeriSock.Api;

using System.Threading.Tasks;

using DeriSock.JsonRpc;
using DeriSock.Model;

/// <summary>
///   Methods used in connection with Authentication (e.g. login, logout, switch sub accounts, ...).
/// </summary>
public interface IAuthenticationApi
{
  /// <inheritdoc cref="IAuthenticationGrantTypes" />
  public IAuthenticationGrantTypes Login();

  /// <summary>
  ///   Generates token for new subject id. This method can be used to switch between subaccounts.
  /// </summary>
  /// <param name="refreshToken">Refresh token</param>
  /// <param name="subjectId">New subject id</param>
  public Task<JsonRpcResponse<PublicExchangeTokenResponse>> ExchangeToken(string refreshToken, int subjectId);

  /// <summary>
  ///   Generates token for new named session. This method can be used only with session scoped tokens.
  /// </summary>
  /// <param name="refreshToken">Refresh token</param>
  /// <param name="sessionName">New session name</param>
  public Task<JsonRpcResponse<PublicForkTokenResponse>> ForkToken(string refreshToken, string sessionName);

  /// <summary>
  ///   Gracefully close websocket connection, when COD (Cancel On Disconnect) is enabled orders are not cancelled.
  /// </summary>
  /// <param name="invalidateToken">If value is <c>true</c> all tokens created in current session are invalidated, default: <c>true</c>.</param>
  public bool Logout(bool? invalidateToken = null);
}
