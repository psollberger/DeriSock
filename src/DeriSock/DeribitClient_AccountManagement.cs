namespace DeriSock;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<Announcement[]>> InternalPublicGetAnnouncements(PublicGetAnnouncementsRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("public/get_announcements", args, new ObjectJsonConverter<Announcement[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PortfolioMargins>> InternalPublicGetPortfolioMargins(PublicGetPortfolioMarginsRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_portfolio_margins", args, new ObjectJsonConverter<PortfolioMargins>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData>> InternalPrivateChangeApiKeyName(PrivateChangeApiKeyNameRequest args, CancellationToken cancellationToken = default)
    => await Send("private/change_api_key_name", args, new ObjectJsonConverter<ApiKeyData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData>> InternalPrivateChangeScopeInApiKey(PrivateChangeScopeInApiKeyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/change_scope_in_api_key", args, new ObjectJsonConverter<ApiKeyData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateChangeSubaccountName(PrivateChangeSubaccountNameRequest args, CancellationToken cancellationToken = default)
    => await Send("private/change_subaccount_name", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData>> InternalPrivateCreateApiKey(PrivateCreateApiKeyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/create_api_key", args, new ObjectJsonConverter<ApiKeyData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<CreateSubAccountResponse>> InternalPrivateCreateSubaccount(CancellationToken cancellationToken = default)
    => await Send("private/create_subaccount", null, new ObjectJsonConverter<CreateSubAccountResponse>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData>> InternalPrivateDisableApiKey(PrivateDisableApiKeyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/disable_api_key", args, new ObjectJsonConverter<ApiKeyData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateEnableAffiliateProgram(CancellationToken cancellationToken = default)
    => await Send("private/enable_affiliate_program", null, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData>> InternalPrivateEnableApiKey(PrivateEnableApiKeyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/enable_api_key", args, new ObjectJsonConverter<ApiKeyData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<AccessLogEntry[]>> InternalPrivateGetAccessLog(PrivateGetAccessLogRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("private/get_access_log", args, new ObjectJsonConverter<AccessLogEntry[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<AccountSummaryData>> InternalPrivateGetAccountSummary(PrivateGetAccountSummaryRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_account_summary", args, new ObjectJsonConverter<AccountSummaryData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<AffiliateProgramInfo>> InternalPrivateGetAffiliateProgramInfo(CancellationToken cancellationToken = default)
    => await Send("private/get_affiliate_program_info", null, new ObjectJsonConverter<AffiliateProgramInfo>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateGetEmailLanguage(CancellationToken cancellationToken = default)
    => await Send("private/get_email_language", null, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Announcement[]>> InternalPrivateGetNewAnnouncements(CancellationToken cancellationToken = default)
    => await Send("private/get_new_announcements", null, new ObjectJsonConverter<Announcement[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PortfolioMargins>> InternalPrivateGetPortfolioMargins(PrivateGetPortfolioMarginsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_portfolio_margins", args, new ObjectJsonConverter<PortfolioMargins>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserPosition>> InternalPrivateGetPosition(PrivateGetPositionRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_position", args, new ObjectJsonConverter<UserPosition>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserPosition[]>> InternalPrivateGetPositions(PrivateGetPositionsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_positions", args, new ObjectJsonConverter<UserPosition[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<SubAccount[]>> InternalPrivateGetSubaccounts(PrivateGetSubaccountsRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("private/get_subaccounts", args, new ObjectJsonConverter<SubAccount[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<SubAccountDetail[]>> InternalPrivateGetSubaccountsDetails(PrivateGetSubaccountsDetailsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_subaccounts_details", args, new ObjectJsonConverter<SubAccountDetail[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<TransactionLogPage>> InternalPrivateGetTransactionLog(PrivateGetTransactionLogRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_transaction_log", args, new ObjectJsonConverter<TransactionLogPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserLockEntry[]>> InternalPrivateGetUserLocks(CancellationToken cancellationToken = default)
    => await Send("private/get_user_locks", null, new ObjectJsonConverter<UserLockEntry[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData[]>> InternalPrivateListApiKeys(CancellationToken cancellationToken = default)
    => await Send("private/list_api_keys", null, new ObjectJsonConverter<ApiKeyData[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateRemoveApiKey(PrivateRemoveApiKeyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/remove_api_key", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateRemoveSubaccount(PrivateRemoveSubaccountRequest args, CancellationToken cancellationToken = default)
    => await Send("private/remove_subaccount", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData>> InternalPrivateResetApiKey(PrivateResetApiKeyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/reset_api_key", args, new ObjectJsonConverter<ApiKeyData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateSetAnnouncementAsRead(PrivateSetAnnouncementAsReadRequest args, CancellationToken cancellationToken = default)
    => await Send("private/set_announcement_as_read", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ApiKeyData>> InternalPrivateSetApiKeyAsDefault(PrivateSetApiKeyAsDefaultRequest args, CancellationToken cancellationToken = default)
    => await Send("private/set_api_key_as_default", args, new ObjectJsonConverter<ApiKeyData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateSetEmailForSubaccount(PrivateSetEmailForSubaccountRequest args, CancellationToken cancellationToken = default)
    => await Send("private/set_email_for_subaccount", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateSetEmailLanguage(PrivateSetEmailLanguageRequest args, CancellationToken cancellationToken = default)
    => await Send("private/set_email_language", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateSetPasswordForSubaccount(PrivateSetPasswordForSubaccountRequest args, CancellationToken cancellationToken = default)
    => await Send("private/set_password_for_subaccount", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateToggleNotificationsFromSubaccount(PrivateToggleNotificationsFromSubaccountRequest args, CancellationToken cancellationToken = default)
    => await Send("private/toggle_notifications_from_subaccount", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PortfolioMarginingToggleEntry[]>> InternalPrivateTogglePortfolioMargining(PrivateTogglePortfolioMarginingRequest args, CancellationToken cancellationToken = default)
    => await Send("private/toggle_portfolio_margining", args, new ObjectJsonConverter<PortfolioMarginingToggleEntry[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateToggleSubaccountLogin(PrivateToggleSubaccountLoginRequest args, CancellationToken cancellationToken = default)
    => await Send("private/toggle_subaccount_login", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);
}
