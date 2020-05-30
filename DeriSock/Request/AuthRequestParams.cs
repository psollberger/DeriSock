namespace DeriSock.Request
{
  using DeriSock.Utils;

  public class AuthRequestParams
  {
    /// <summary>
    ///   Method of authentication
    ///   <para>
    ///     <c>client_credentials</c>,
    ///     <c>client_signature</c>,
    ///     <c>refresh_token</c>
    ///   </para>
    /// </summary>
    public string GrantType { get; set; }

    /// <summary>
    ///   Required for grant type <c>client_credentials</c> and <c>client_signature</c>
    /// </summary>
    public string ClientId { get; set; }

    /// <summary>
    ///   Required for grant type <c>client_credentials</c> and <c>client_signature</c>
    /// </summary>
    public string ClientSecret { get; set; }

    /// <summary>
    ///   Optional for grant type <c>client_signature</c>; delivers user generated initialization vector for the server token
    /// </summary>
    public string Data { get; set; }

    /// <summary>
    ///   Will be passed back in the response
    /// </summary>
    public string State { get; set; }

    /// <summary>
    ///   Describes type of the access for assigned token, possible values:
    ///   <para>
    ///     <c>connection</c>,
    ///     <c>session:name</c>,
    ///     <c>trade:[red, read_write, none]</c>,
    ///     <c>wallet:[read, read_write, none]</c>,
    ///     <c>account:[read, read_write, none]</c>,
    ///     <c>expires:NUMBER</c>,
    ///     <c>ip:ADDR</c>
    ///   </para>
    ///   <para>Details are elucidated in <a href="https://docs.deribit.com/#access-scope">Access scope</a></para>
    /// </summary>
    public string Scope { get; set; }
  }
}
