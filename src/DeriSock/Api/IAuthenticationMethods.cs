namespace DeriSock.Api;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.JsonRpc;
using DeriSock.Model;

/// <summary>
/// <para>Retrieve an Oauth access token, to be used for authentication of &apos;private&apos; requests.</para>
/// <para>Three methods of authentication are supported:</para>
/// <para>- <c>client_credentials</c> - using the access key and access secret that can be found on the API page on the website</para>
/// <para>- <c>client_signature</c> - using the access key that can be found on the API page on the website and user generated signature. The signature is calculated using some fields provided in the request, using formula described here <a href="https://docs.deribit.com/#additional-authorization-method-signature-credentials-websocket-api">Deribit signature credentials</a></para>
/// <para>- <c>refresh_token</c> - using a refresh token that was received from an earlier invocation</para>
/// <para>The response will contain an access token, expiration period (number of seconds that the token is valid) and a refresh token that can be used to get a new set of tokens.</para>
/// </summary>
public interface IAuthenticationMethods
{
  /// <summary>
  ///   Using the access key and access secret that can be found on the API page on the website.
  /// </summary>
  /// <param name="clientId">The clients access key.</param>
  /// <param name="clientSecret">The clients access secret.</param>
  /// <param name="state">Will be passed back in the response.</param>
  /// <param name="scope">Describes type of the access for assigned token.</param>
  /// <returns></returns>
  public Task<JsonRpcResponse<AuthTokenData>> WithClientCredentials(string clientId, string clientSecret, string state = "", string scope = "", CancellationToken cancellationToken = default);

  /// <summary>
  ///   Using the access key that can be found on the API page on the website and user generated signature.
  /// </summary>
  /// <param name="clientId">The clients access key.</param>
  /// <param name="clientSecret">The clients access secret.</param>
  /// <param name="data">Optional: Contains any user specific value.</param>
  /// <param name="state">Will be passed back in the response.</param>
  /// <param name="scope">Describes type of the access for assigned token.</param>
  /// <returns></returns>
  public Task<JsonRpcResponse<AuthTokenData>> WithClientSignature(string clientId, string clientSecret, string data = "", string state = "", string scope = "", CancellationToken cancellationToken = default);

  /// <summary>
  ///   Using a refresh token that was received earlier.
  /// </summary>
  /// <param name="refreshToken">The refresh token to be used.</param>
  /// <param name="state">Will be passed back in the response.</param>
  /// <param name="scope">Describes type of the access for assigned token.</param>
  /// <returns></returns>
  public Task<JsonRpcResponse<AuthTokenData>> WithRefreshToken(string state = "", string scope = "", CancellationToken cancellationToken = default);
}
