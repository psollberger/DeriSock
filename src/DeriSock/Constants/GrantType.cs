namespace DeriSock.Constants;

/// <summary>
///   Method of authentication
/// </summary>
public static class GrantType
{
  /// <summary>
  ///   Using <c>client_credentials</c> authentication
  /// </summary>
  public const string Credentials = "client_credentials";

  /// <summary>
  ///   Using <c>client_signature</c> authentication
  /// </summary>
  public const string Signature = "client_signature";

  /// <summary>
  ///   Using <c>refresh_token</c> authentication
  /// </summary>
  public const string RefreshToken = "refresh_token";
}
