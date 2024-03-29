namespace DeriSock;

using System;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;
using DeriSock.Utils;

public partial class DeribitClient : IAuthenticationMethods
{
  /// <inheritdoc />
  async Task<JsonRpcResponse<AuthTokenData>> IAuthenticationMethods.WithClientCredentials(string clientId, string clientSecret, string state, string scope, CancellationToken cancellationToken)
  {
    _logger?.Debug("Authenticate (client_credentials)");

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

    _authRequest = reqParams;
    _authRequestSignatureData = null;
    return await InternalPublicAuth(reqParams, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  async Task<JsonRpcResponse<AuthTokenData>> IAuthenticationMethods.WithClientSignature(string clientId, string clientSecret, string data, string state, string scope, CancellationToken cancellationToken)
  {
    _logger?.Debug("Authenticate (client_signature)");

    if (!string.IsNullOrEmpty(AccessToken))
      throw new InvalidOperationException("Already authorized");

    if (string.IsNullOrEmpty(clientId))
      throw new ArgumentNullException(nameof(clientId));

    if (string.IsNullOrEmpty(clientSecret))
      throw new ArgumentNullException(nameof(clientSecret));

    var sig = SignatureData.Create(clientSecret, data);

    var reqParams = new PublicAuthRequest
    {
      GrantType = GrantType.ClientSignature,
      ClientId = clientId,
      State = string.IsNullOrEmpty(state) ? null : state,
      Scope = string.IsNullOrEmpty(scope) ? null : scope
    };

    sig.Apply(reqParams);

    _authRequest = reqParams;
    _authRequestSignatureData = sig;
    return await InternalPublicAuth(reqParams, cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc />
  async Task<JsonRpcResponse<AuthTokenData>> IAuthenticationMethods.WithRefreshToken(string state, string scope, CancellationToken cancellationToken)
  {
    _logger?.Debug("Authenticate (refresh_token)");

    if (string.IsNullOrEmpty(RefreshToken))
      throw new ArgumentNullException(nameof(RefreshToken));

    var reqParams = new PublicAuthRequest
    {
      GrantType = GrantType.RefreshToken,
      RefreshToken = RefreshToken!,
      State = string.IsNullOrEmpty(state) ? null : state,
      Scope = string.IsNullOrEmpty(scope) ? null : scope
    };

    return await InternalPublicAuth(reqParams, cancellationToken).ConfigureAwait(false);
  }

  private async Task<JsonRpcResponse<AuthTokenData>> InternalPublicAuth(PublicAuthRequest args, CancellationToken cancellationToken = default)
  {
    var response = await Send("public/auth", args, new ObjectJsonConverter<AuthTokenData>(), cancellationToken).ConfigureAwait(false);

    var loginRes = response.Data;

    AccessToken = loginRes?.AccessToken ?? string.Empty;
    RefreshToken = loginRes?.RefreshToken ?? string.Empty;

    if (response.Error is not null) {
      _logger?.Debug("Authentication failed: {@Error}", response.Error);
      _authRequest = null;
      _authRequestSignatureData = null;
      return response;
    }

    if (string.IsNullOrEmpty(args.RefreshToken))
      EnqueueAuthRefresh(loginRes!.ExpiresIn);

    return response;
  }

  /// <inheritdoc cref="IAuthenticationApi.PublicExchangeToken" />
  private async Task<JsonRpcResponse<AuthTokenData>> InternalPublicExchangeToken(PublicExchangeTokenRequest args, CancellationToken cancellationToken = default)
  {
    _logger?.Debug("Exchanging token");
    return await Send("public/exchange_token", args, new ObjectJsonConverter<AuthTokenData>(), cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc cref="IAuthenticationApi.PublicForkToken" />
  private async Task<JsonRpcResponse<AuthTokenData>> InternalPublicForkToken(PublicForkTokenRequest args, CancellationToken cancellationToken = default)
  {
    _logger?.Debug("Forking token");
    return await Send("public/fork_token", args, new ObjectJsonConverter<AuthTokenData>(), cancellationToken).ConfigureAwait(false);
  }

  /// <inheritdoc cref="IAuthenticationApi.PrivateLogout" />
  private void InternalPrivateLogout(PrivateLogoutRequest? args)
  {
    _logger?.Debug("Logging out");

    if (string.IsNullOrEmpty(AccessToken))
      return;

    SendSync("private/logout", args);

    _authRequest = null;
    _authRequestSignatureData = null;
    AccessToken = null;
    RefreshToken = null;
  }
}
