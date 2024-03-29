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
namespace DeriSock
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.Api;
  using DeriSock.Net.JsonRpc;
  using DeriSock.Model;
  using Newtonsoft.Json.Linq;
  
  public partial class DeribitClient
  {
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    private sealed partial class AccountManagementApiImpl : IAccountManagementApi
    {
      private readonly DeribitClient _client;
      public AccountManagementApiImpl(DeribitClient client)
      {
        _client = client;
      }
      /// <inheritdoc cref="IAccountManagementApi.PublicGetAnnouncements" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<Announcement[]>> IAccountManagementApi.PublicGetAnnouncements(PublicGetAnnouncementsRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPublicGetAnnouncements(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PublicGetPortfolioMargins" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<PortfolioMargins>> IAccountManagementApi.PublicGetPortfolioMargins(PublicGetPortfolioMarginsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPublicGetPortfolioMargins(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateChangeApiKeyName" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData>> IAccountManagementApi.PrivateChangeApiKeyName(PrivateChangeApiKeyNameRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateChangeApiKeyName(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateChangeScopeInApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData>> IAccountManagementApi.PrivateChangeScopeInApiKey(PrivateChangeScopeInApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateChangeScopeInApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateChangeSubaccountName" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateChangeSubaccountName(PrivateChangeSubaccountNameRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateChangeSubaccountName(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateCreateApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData>> IAccountManagementApi.PrivateCreateApiKey(PrivateCreateApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCreateApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateCreateSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<CreateSubAccountResponse>> IAccountManagementApi.PrivateCreateSubaccount(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCreateSubaccount(cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateDisableApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData>> IAccountManagementApi.PrivateDisableApiKey(PrivateDisableApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateDisableApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateEnableAffiliateProgram" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateEnableAffiliateProgram(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateEnableAffiliateProgram(cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateEnableApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData>> IAccountManagementApi.PrivateEnableApiKey(PrivateEnableApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateEnableApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetAccessLog" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<AccessLogEntry[]>> IAccountManagementApi.PrivateGetAccessLog(PrivateGetAccessLogRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetAccessLog(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetAccountSummary" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<AccountSummaryData>> IAccountManagementApi.PrivateGetAccountSummary(PrivateGetAccountSummaryRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetAccountSummary(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetAffiliateProgramInfo" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<AffiliateProgramInfo>> IAccountManagementApi.PrivateGetAffiliateProgramInfo(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetAffiliateProgramInfo(cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetEmailLanguage" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateGetEmailLanguage(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetEmailLanguage(cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetNewAnnouncements" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<Announcement[]>> IAccountManagementApi.PrivateGetNewAnnouncements(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetNewAnnouncements(cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetPortfolioMargins" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<PortfolioMargins>> IAccountManagementApi.PrivateGetPortfolioMargins(PrivateGetPortfolioMarginsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetPortfolioMargins(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetPosition" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserPosition>> IAccountManagementApi.PrivateGetPosition(PrivateGetPositionRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetPosition(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetPositions" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserPosition[]>> IAccountManagementApi.PrivateGetPositions(PrivateGetPositionsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetPositions(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetSubaccounts" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<SubAccount[]>> IAccountManagementApi.PrivateGetSubaccounts(PrivateGetSubaccountsRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetSubaccounts(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetSubaccountsDetails" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<SubAccountDetail[]>> IAccountManagementApi.PrivateGetSubaccountsDetails(PrivateGetSubaccountsDetailsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetSubaccountsDetails(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetTransactionLog" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<TransactionLogPage>> IAccountManagementApi.PrivateGetTransactionLog(PrivateGetTransactionLogRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetTransactionLog(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateGetUserLocks" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserLockEntry[]>> IAccountManagementApi.PrivateGetUserLocks(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetUserLocks(cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateListApiKeys" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData[]>> IAccountManagementApi.PrivateListApiKeys(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateListApiKeys(cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateRemoveApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateRemoveApiKey(PrivateRemoveApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateRemoveApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateRemoveSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateRemoveSubaccount(PrivateRemoveSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateRemoveSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateResetApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData>> IAccountManagementApi.PrivateResetApiKey(PrivateResetApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateResetApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateSetAnnouncementAsRead" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateSetAnnouncementAsRead(PrivateSetAnnouncementAsReadRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetAnnouncementAsRead(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateSetApiKeyAsDefault" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<ApiKeyData>> IAccountManagementApi.PrivateSetApiKeyAsDefault(PrivateSetApiKeyAsDefaultRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetApiKeyAsDefault(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateSetEmailForSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateSetEmailForSubaccount(PrivateSetEmailForSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetEmailForSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateSetEmailLanguage" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateSetEmailLanguage(PrivateSetEmailLanguageRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetEmailLanguage(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateSetPasswordForSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateSetPasswordForSubaccount(PrivateSetPasswordForSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetPasswordForSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateToggleNotificationsFromSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateToggleNotificationsFromSubaccount(PrivateToggleNotificationsFromSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateToggleNotificationsFromSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateTogglePortfolioMargining" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<PortfolioMarginingToggleEntry[]>> IAccountManagementApi.PrivateTogglePortfolioMargining(PrivateTogglePortfolioMarginingRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateTogglePortfolioMargining(args, cancellationToken);
      }
      /// <inheritdoc cref="IAccountManagementApi.PrivateToggleSubaccountLogin" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> IAccountManagementApi.PrivateToggleSubaccountLogin(PrivateToggleSubaccountLoginRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateToggleSubaccountLogin(args, cancellationToken);
      }
    }
  }
}
