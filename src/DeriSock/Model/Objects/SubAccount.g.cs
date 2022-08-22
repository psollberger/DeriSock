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
  
  /// <summary>
  /// </summary>
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial class SubAccount
  {
    /// <summary>
    /// <para>User email</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("email")]
    public string Email { get; set; } = string.Empty;
    /// <summary>
    /// <para>Account/Subaccount identifier</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("id")]
    public int Id { get; set; }
    /// <summary>
    /// <para><c>true</c> when password for the subaccount has been configured</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("is_password")]
    public bool IsPassword { get; set; }
    /// <summary>
    /// <para>Informs whether login to the subaccount is enabled</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("login_enabled")]
    public bool LoginEnabled { get; set; }
    /// <summary>
    /// <para>New email address that has not yet been confirmed. This field is only included if <c>with_portfolio</c> == <c>true</c>.</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("not_confirmed_email")]
    public string NotConfirmedEmail { get; set; } = string.Empty;
    /// <summary>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("portfolio")]
    public SubAccountPortfolio Portfolio { get; set; } = null!;
    /// <summary>
    /// <para>When <c>true</c> - receive all notification emails on the main email</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("receive_notifications")]
    public bool ReceiveNotifications { get; set; }
    /// <summary>
    /// <para>Names of assignments with Security Keys assigned</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("security_keys_assignments")]
    public string[] SecurityKeysAssignments { get; set; } = System.Array.Empty<System.String>();
    /// <summary>
    /// <para>Whether the Security Keys authentication is enabled</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("security_keys_enabled")]
    public bool SecurityKeysEnabled { get; set; }
    /// <summary>
    /// <para>System generated user nickname</para>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("system_name")]
    public string SystemName { get; set; } = string.Empty;
    /// <summary>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("type")]
    public string Type { get; set; } = string.Empty;
    /// <summary>
    /// </summary>
    [System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [Newtonsoft.Json.JsonPropertyAttribute("username")]
    public string Username { get; set; } = string.Empty;
  }
}