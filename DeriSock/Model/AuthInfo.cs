namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class AuthInfo
  {
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }

    ///<summary>Token lifetime in seconds</summary>
    [JsonProperty("expires_in")]
    public int ExpiresIn { get; set; }

    /// <summary>
    ///   Can be used to request a new token (with a new lifetime)
    /// </summary>
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }

    /// <summary>
    ///   Type of the access for assigned token
    /// </summary>
    [JsonProperty("scope")]
    public string Scope { get; set; }

    /// <summary>
    ///   Copied from the input (if applicable)
    /// </summary>
    [JsonProperty("state")]
    public string State { get; set; }

    /// <summary>
    ///   Authorization type, allowed value - <c>bearer</c>
    /// </summary>
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
  }
}
