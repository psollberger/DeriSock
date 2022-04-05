namespace DeriSock.Model;

using Newtonsoft.Json;

public class SubAccount
{
  /// <summary>
  ///   User email
  /// </summary>
  [JsonProperty("email")]
  public string Email { get; set; }

  /// <summary>
  ///   Account/Subaccount identifier
  /// </summary>
  [JsonProperty("id")]
  public int Id { get; set; }

  /// <summary>
  ///   <c>true</c> when password for the subaccount has been configured
  /// </summary>
  [JsonProperty("is_password")]
  public bool HasPassword { get; set; }

  /// <summary>
  ///   Informs whether login to the subaccount is enabled
  /// </summary>
  [JsonProperty("login_enabled")]
  public bool LoginEnabled { get; set; }

  /// <summary>
  ///   New email address that has not yet been confirmed. This field is only included if <c>with_portfolio == true</c>
  /// </summary>
  [JsonProperty("not_confirmed_email")]
  public string NotConfirmedEMail { get; set; }

  [JsonProperty("portfolio")]
  public SubAccountPortfolios Portfolio { get; set; }

  /// <summary>
  ///   When <c>true</c>, receive all notification emails on the main email
  /// </summary>
  [JsonProperty("receive_notifications")]
  public bool ReceiveNotifications { get; set; }

  /// <summary>
  ///   System generated user nickname
  /// </summary>
  [JsonProperty("system_name")]
  public string SystemName { get; set; }

  /// <summary>
  ///   Whether the two factor authentication is enabled
  /// </summary>
  [JsonProperty("tfa_enabled")]
  public bool TfaEnabled { get; set; }

  [JsonProperty("type")]
  public string Type { get; set; }

  [JsonProperty("username")]
  public string Username { get; set; }
}
