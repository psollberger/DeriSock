namespace DeriSock.Api;

using System.Threading.Tasks;

using DeriSock.JsonRpc;
using DeriSock.Model;

/// <summary>
///   <para>Retrieve an Oauth access token, to be used for authentication of 'private' requests.</para>
///   <para>Three methods of authentication are supported:</para>
///   <para>
///     <list type="table">
///       <item>
///         <term>client_credentials</term>
///         <description>using the access key and access secret that can be found on the API page on the website</description>
///       </item>
///       <item>
///         <term>client_signature</term>
///         <description>using the access key that can be found on the API page on the website and user generated signature</description>
///       </item>
///       <item>
///         <term>refresh_token</term>
///         <description>using a refresh token that was received from an earlier invocation</description>
///       </item>
///     </list>
///   </para>
///   <para>
///     The response will contain an access token, expiration period (number of seconds that the token is valid) and a
///     refresh token that can be used to get a new set of tokens
///   </para>
/// </summary>
public interface IAuthenticationGrantTypes
{
  /// <summary>
  ///   Using the access key and access secret that can be found on the API page on the website.
  /// </summary>
  /// <param name="clientId">The clients access key.</param>
  /// <param name="clientSecret">The clients access secret.</param>
  /// <param name="state">Will be passed back in the response.</param>
  /// <param name="scope">Describes type of the access for assigned token.</param>
  /// <returns></returns>
  public Task<JsonRpcResponse<PublicAuthResponse>> WithClientCredentials(string clientId, string clientSecret, string state = "", string scope = "");

  /// <summary>
  ///   Using the access key that can be found on the API page on the website and user generated signature.
  /// </summary>
  /// <param name="clientId">The clients access key.</param>
  /// <param name="clientSecret">The clients access secret.</param>
  /// <param name="data">Optional: Contains any user specific value.</param>
  /// <param name="state">Will be passed back in the response.</param>
  /// <param name="scope">Describes type of the access for assigned token.</param>
  /// <returns></returns>
  public Task<JsonRpcResponse<PublicAuthResponse>> WithClientSignature(string clientId, string clientSecret, string data = "", string state = "", string scope = "");

  /// <summary>
  ///   Using a refresh token that was received earlier.
  /// </summary>
  /// <param name="refreshToken">The refresh token to be used.</param>
  /// <param name="state">Will be passed back in the response.</param>
  /// <param name="scope">Describes type of the access for assigned token.</param>
  /// <returns></returns>
  public Task<JsonRpcResponse<PublicAuthResponse>> WithRefreshToken(string state = "", string scope = "");
}
