namespace DeriSock;

using System;
using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;
using DeriSock.Utils;

public partial class DeribitClient : IAuthenticationApi, IAuthenticationMethods
{
  /// <inheritdoc cref="IAuthenticationApi" />
  IAuthenticationApi ICategoriesApi.Authentication()
    => this;

  /// <inheritdoc cref="IAuthenticationApi" />
   public IAuthenticationMethods PublicLogin()
    => this;

  /// <inheritdoc />
  async Task<JsonRpcResponse<PublicAuthResponse>> IAuthenticationMethods.WithClientCredentials(string clientId, string clientSecret, string state, string scope)
  {
    _logger.Debug("Authenticate (client_credentials)");

    if (!string.IsNullOrEmpty(AccessToken))
      throw new InvalidOperationException("Already authorized");

    if (string.IsNullOrEmpty(clientId))
      throw new ArgumentNullException(nameof(clientId));

    if (string.IsNullOrEmpty(clientSecret))
      throw new ArgumentNullException(nameof(clientSecret));

    var reqParams = new PublicAuthRequest
    {
      GrantType = GrantType.ClientCredentials,
      ClientId = clientId,
      ClientSecret = clientSecret,
      State = string.IsNullOrEmpty(state) ? null : state,
      Scope = string.IsNullOrEmpty(scope) ? null : scope
    };

    var response = await Send("public/auth", reqParams, new ObjectJsonConverter<PublicAuthResponse>()).ConfigureAwait(false);

    var loginRes = response.ResultData;

    AccessToken = loginRes.AccessToken;
    RefreshToken = loginRes.RefreshToken;

    EnqueueAuthRefresh(loginRes.ExpiresIn);

    return response;
  }

  /// <inheritdoc />
  async Task<JsonRpcResponse<PublicAuthResponse>> IAuthenticationMethods.WithClientSignature(string clientId, string clientSecret, string data, string state, string scope)
  {
    _logger.Debug("Authenticate (client_signature)");

    if (!string.IsNullOrEmpty(AccessToken))
      throw new InvalidOperationException("Already authorized");

    if (string.IsNullOrEmpty(clientId))
      throw new ArgumentNullException(nameof(clientId));

    if (string.IsNullOrEmpty(clientSecret))
      throw new ArgumentNullException(nameof(clientSecret));

    var sig = SignatureData.Create(clientId, clientSecret, data);

    var reqParams = new PublicAuthRequest
    {
      GrantType = GrantType.ClientSignature,
      ClientId = clientId,
      Timestamp = sig.Timestamp,
      Signature = sig.Signature,
      Nonce = sig.Nonce,
      Data = sig.Data,
      State = string.IsNullOrEmpty(state) ? null : state,
      Scope = string.IsNullOrEmpty(scope) ? null : scope
    };

    var response = await Send("public/auth", reqParams, new ObjectJsonConverter<PublicAuthResponse>()).ConfigureAwait(false);

    var loginRes = response.ResultData;

    AccessToken = loginRes.AccessToken;
    RefreshToken = loginRes.RefreshToken;

    EnqueueAuthRefresh(loginRes.ExpiresIn);

    return response;
  }

  /// <inheritdoc />
  async Task<JsonRpcResponse<PublicAuthResponse>> IAuthenticationMethods.WithRefreshToken(string state, string scope)
  {
    _logger.Debug("Authenticate (refresh_token)");

    if (string.IsNullOrEmpty(RefreshToken))
      throw new ArgumentNullException(nameof(RefreshToken));

    var reqParams = new PublicAuthRequest
    {
      GrantType = GrantType.RefreshToken,
      RefreshToken = RefreshToken!,
      State = string.IsNullOrEmpty(state) ? null : state,
      Scope = string.IsNullOrEmpty(scope) ? null : scope
    };

    var response = await Send("public/auth", reqParams, new ObjectJsonConverter<PublicAuthResponse>()).ConfigureAwait(false);

    var loginRes = response.ResultData;

    AccessToken = loginRes.AccessToken;
    RefreshToken = loginRes.RefreshToken;

    return response;
  }

  /// <inheritdoc cref="IAuthenticationApi.PublicExchangeToken" />
  public async Task<JsonRpcResponse<PublicExchangeTokenResponse>> PublicExchangeToken(PublicExchangeTokenRequest args)
  {
    _logger.Debug("Exchanging token");
    return await Send("public/exchange_token", args, new ObjectJsonConverter<PublicExchangeTokenResponse>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="IAuthenticationApi.PublicForkToken" />
  public async Task<JsonRpcResponse<PublicForkTokenResponse>> PublicForkToken(PublicForkTokenRequest args)
  {
    _logger.Debug("Forking token");
    return await Send("public/fork_token", args, new ObjectJsonConverter<PublicForkTokenResponse>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="IAuthenticationApi.PrivateLogout" />
  public void PrivateLogout(PrivateLogoutRequest? args)
  {
    _logger.Debug("Logging out");

    if (string.IsNullOrEmpty(AccessToken))
      return;

    //TODO: check if logout works without access_token being sent
    //_client.SendLogout("private/logout", new {access_token = AccessToken});
    _client.SendLogoutSync("private/logout", args);

    AccessToken = null;
    RefreshToken = null;
  }
}
