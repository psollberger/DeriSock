// --------------------------------------------------------------------------
// <auto-generated>
//      This code was generated by a tool.
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
// </auto-generated>
// --------------------------------------------------------------------------
#pragma warning disable CS1591
#nullable enable
namespace DeriSock.Model
{
  using System;
  using DeriSock.Converter;
  
  /// <summary>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial class PublicAuthRequest
  {
    /// <summary>
    /// <para>Method of authentication</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("grant_type")]
    public GrantType? GrantType { get; set; }
    /// <summary>
    /// <para>Required for grant type <c>client_credentials</c> and <c>client_signature</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("client_id")]
    public string? ClientId { get; set; }
    /// <summary>
    /// <para>Required for grant type <c>client_credentials</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("client_secret")]
    public string? ClientSecret { get; set; }
    /// <summary>
    /// <para>Required for grant type <c>refresh_token</c></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("refresh_token")]
    public string? RefreshToken { get; set; }
    /// <summary>
    /// <para>Required for grant type <c>client_signature</c>, provides time when request has been generated (milliseconds since the UNIX epoch)</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("timestamp")]
    [Newtonsoft.Json.JsonConverter(typeof(MillisecondsTimestampConverter))]
    public DateTime? Timestamp { get; set; }
    /// <summary>
    /// <para>Required for grant type <c>client_signature</c>; it&apos;s a cryptographic signature calculated over provided fields using user <b>secret key</b>. The signature should be calculated as an HMAC (Hash-based Message Authentication Code) with <c>SHA256</c> hash algorithm</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("signature")]
    public string? Signature { get; set; }
    /// <summary>
    /// <para>Optional for grant type <c>client_signature</c>; delivers user generated initialization vector for the server token</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("nonce")]
    public string? Nonce { get; set; }
    /// <summary>
    /// <para>Optional for grant type <c>client_signature</c>; contains any user specific value</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("data")]
    public string? Data { get; set; }
    /// <summary>
    /// <para>Will be passed back in the response</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("state")]
    public string? State { get; set; }
    /// <summary>
    /// <para>Describes type of the access for assigned token, possible values: <c>connection</c>, <c>session:name</c>, <c>trade:[read, read_write, none]</c>, <c>wallet:[read, read_write, none]</c>, <c>account:[read, read_write, none]</c>, <c>expires:NUMBER</c>, <c>ip:ADDR</c>.<br/>
    ///Details are elucidated in <a href="https://docs.deribit.com/#access-scope">Access scope</a></para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("scope")]
    public string? Scope { get; set; }
  }
}
