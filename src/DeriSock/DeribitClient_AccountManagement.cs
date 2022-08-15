namespace DeriSock;

using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

using Newtonsoft.Json.Linq;

public partial class DeribitClient : IAccountManagementApi
{
  /// <inheritdoc cref="IAccountManagementApi.PublicGetAnnouncements" />
  public async Task<JsonRpcResponse<PublicGetAnnouncementsResponse>> PublicGetAnnouncements(PublicGetAnnouncementsRequest? args)
    => await Send("public/get_announcements", args, new ObjectJsonConverter<PublicGetAnnouncementsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PublicGetPortfolioMargins" />
  public async Task<JsonRpcResponse<JObject>> PublicGetPortfolioMargins(PublicGetPortfolioMarginsRequest args)
    => await Send("public/get_portfolio_margins", args, new ObjectJsonConverter<JObject>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateChangeApiKeyName" />
  public async Task<JsonRpcResponse<PrivateChangeApiKeyNameResponse>> PrivateChangeApiKeyName(PrivateChangeApiKeyNameRequest args)
    => await Send("private/change_api_key_name", args, new ObjectJsonConverter<PrivateChangeApiKeyNameResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateChangeScopeInApiKey" />
  public async Task<JsonRpcResponse<PrivateChangeScopeInApiKeyResponse>> PrivateChangeScopeInApiKey(PrivateChangeScopeInApiKeyRequest args)
    => await Send("private/change_scope_in_api_key", args, new ObjectJsonConverter<PrivateChangeScopeInApiKeyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateChangeSubaccountName" />
  public async Task<JsonRpcResponse<string>> PrivateChangeSubaccountName(PrivateChangeSubaccountNameRequest args)
    => await Send("private/change_subaccount_name", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateCreateApiKey" />
  public async Task<JsonRpcResponse<PrivateCreateApiKeyResponse>> PrivateCreateApiKey(PrivateCreateApiKeyRequest args)
    => await Send("private/create_api_key", args, new ObjectJsonConverter<PrivateCreateApiKeyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateCreateSubaccount" />
  public async Task<JsonRpcResponse<PrivateCreateSubaccountResponse>> PrivateCreateSubaccount()
    => await Send("private/create_subaccount", null, new ObjectJsonConverter<PrivateCreateSubaccountResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateDisableApiKey" />
  public async Task<JsonRpcResponse<PrivateDisableApiKeyResponse>> PrivateDisableApiKey(PrivateDisableApiKeyRequest args)
    => await Send("private/disable_api_key", args, new ObjectJsonConverter<PrivateDisableApiKeyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateEnableAffiliateProgram" />
  public async Task<JsonRpcResponse<string>> PrivateEnableAffiliateProgram()
    => await Send("private/enable_affiliate_program", null, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateEnableApiKey" />
  public async Task<JsonRpcResponse<PrivateEnableApiKeyResponse>> PrivateEnableApiKey(PrivateEnableApiKeyRequest args)
    => await Send("private/enable_api_key", args, new ObjectJsonConverter<PrivateEnableApiKeyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetAccessLog" />
  public async Task<JsonRpcResponse<PrivateGetAccessLogResponse>> PrivateGetAccessLog(PrivateGetAccessLogRequest? args)
    => await Send("private/get_access_log", args, new ObjectJsonConverter<PrivateGetAccessLogResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetAccountSummary" />
  public async Task<JsonRpcResponse<PrivateGetAccountSummaryResponse>> PrivateGetAccountSummary(PrivateGetAccountSummaryRequest args)
    => await Send("private/get_account_summary", args, new ObjectJsonConverter<PrivateGetAccountSummaryResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetAffiliateProgramInfo" />
  public async Task<JsonRpcResponse<PrivateGetAffiliateProgramInfoResponse>> PrivateGetAffiliateProgramInfo()
    => await Send("private/get_affiliate_program_info", null, new ObjectJsonConverter<PrivateGetAffiliateProgramInfoResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetEmailLanguage" />
  public async Task<JsonRpcResponse<string>> PrivateGetEmailLanguage()
    => await Send("private/get_email_language", null, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetNewAnnouncements" />
  public async Task<JsonRpcResponse<PrivateGetNewAnnouncementsResponse>> PrivateGetNewAnnouncements()
    => await Send("private/get_new_announcements", null, new ObjectJsonConverter<PrivateGetNewAnnouncementsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetPortfolioMargins" />
  public async Task<JsonRpcResponse<JObject>> PrivateGetPortfolioMargins(PrivateGetPortfolioMarginsRequest args)
    => await Send("private/get_portfolio_margins", args, new ObjectJsonConverter<JObject>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetPosition" />
  public async Task<JsonRpcResponse<PrivateGetPositionResponse>> PrivateGetPosition(PrivateGetPositionRequest args)
    => await Send("private/get_position", args, new ObjectJsonConverter<PrivateGetPositionResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetPositions" />
  public async Task<JsonRpcResponse<PrivateGetPositionsResponse>> PrivateGetPositions(PrivateGetPositionsRequest args)
    => await Send("private/get_positions", args, new ObjectJsonConverter<PrivateGetPositionsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetSubaccounts" />
  public async Task<JsonRpcResponse<PrivateGetSubaccountsResponse>> PrivateGetSubaccounts(PrivateGetSubaccountsRequest? args)
    => await Send("private/get_subaccounts", args, new ObjectJsonConverter<PrivateGetSubaccountsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetSubaccountsDetails" />
  public async Task<JsonRpcResponse<PrivateGetSubaccountsDetailsResponse>> PrivateGetSubaccountsDetails(PrivateGetSubaccountsDetailsRequest args)
    => await Send("private/get_subaccounts_details", args, new ObjectJsonConverter<PrivateGetSubaccountsDetailsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateGetTransactionLog" />
  public async Task<JsonRpcResponse<PrivateGetTransactionLogResponse>> PrivateGetTransactionLog(PrivateGetTransactionLogRequest args)
    => await Send("private/get_transaction_log", args, new ObjectJsonConverter<PrivateGetTransactionLogResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateListApiKeys" />
  public async Task<JsonRpcResponse<PrivateListApiKeysResponse>> PrivateListApiKeys()
    => await Send("private/list_api_keys", null, new ObjectJsonConverter<PrivateListApiKeysResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateRemoveApiKey" />
  public async Task<JsonRpcResponse<string>> PrivateRemoveApiKey(PrivateRemoveApiKeyRequest args)
    => await Send("private/remove_api_key", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateRemoveSubaccount" />
  public async Task<JsonRpcResponse<string>> PrivateRemoveSubaccount(PrivateRemoveSubaccountRequest args)
    => await Send("private/remove_subaccount", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateResetApiKey" />
  public async Task<JsonRpcResponse<PrivateResetApiKeyResponse>> PrivateResetApiKey(PrivateResetApiKeyRequest args)
    => await Send("private/reset_api_key", args, new ObjectJsonConverter<PrivateResetApiKeyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateSetAnnouncementAsRead" />
  public async Task<JsonRpcResponse<string>> PrivateSetAnnouncementAsRead(PrivateSetAnnouncementAsReadRequest args)
    => await Send("private/set_announcement_as_read", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateSetApiKeyAsDefault" />
  public async Task<JsonRpcResponse<PrivateSetApiKeyAsDefaultResponse>> PrivateSetApiKeyAsDefault(PrivateSetApiKeyAsDefaultRequest args)
    => await Send("private/set_api_key_as_default", args, new ObjectJsonConverter<PrivateSetApiKeyAsDefaultResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateSetEmailForSubaccount" />
  public async Task<JsonRpcResponse<string>> PrivateSetEmailForSubaccount(PrivateSetEmailForSubaccountRequest args)
    => await Send("private/set_email_for_subaccount", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateSetEmailLanguage" />
  public async Task<JsonRpcResponse<string>> PrivateSetEmailLanguage(PrivateSetEmailLanguageRequest args)
    => await Send("private/set_email_language", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateSetPasswordForSubaccount" />
  public async Task<JsonRpcResponse<string>> PrivateSetPasswordForSubaccount(PrivateSetPasswordForSubaccountRequest args)
    => await Send("private/set_password_for_subaccount", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateToggleNotificationsFromSubaccount" />
  public async Task<JsonRpcResponse<string>> PrivateToggleNotificationsFromSubaccount(PrivateToggleNotificationsFromSubaccountRequest args)
    => await Send("private/toggle_notifications_from_subaccount", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IAccountManagementApi.PrivateToggleSubaccountLogin" />
  public async Task<JsonRpcResponse<string>> PrivateToggleSubaccountLogin(PrivateToggleSubaccountLoginRequest args)
    => await Send("private/toggle_subaccount_login", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);
}
