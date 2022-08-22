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
namespace DeriSock.Api
{
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using Newtonsoft.Json.Linq;
  
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial interface IAccountManagementApi
  {
    /// <summary>
    /// <para>Retrieves announcements. Default &quot;start_timestamp&quot; parameter value is current timestamp, &quot;count&quot; parameter value must be between 1 and 50, default is 5.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Announcement[]>> PublicGetAnnouncements(PublicGetAnnouncementsRequest? args = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Public version of the method calculates portfolio margin info for simulated position. For concrete user position, the private version of the method must be used. The public version of the request has special restricted rate limit (not more than once per a second for the IP).</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<JObject>> PublicGetPortfolioMargins(PublicGetPortfolioMarginsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Changes name for key with given id</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> PrivateChangeApiKeyName(PrivateChangeApiKeyNameRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Changes scope for key with given id</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> PrivateChangeScopeInApiKey(PrivateChangeScopeInApiKeyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Change the user name for a subaccount</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateChangeSubaccountName(PrivateChangeSubaccountNameRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Creates new api key with given scope</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> PrivateCreateApiKey(PrivateCreateApiKeyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Create a new subaccount</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<CreateSubAccountResponse>> PrivateCreateSubaccount(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Disables api key with given id</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> PrivateDisableApiKey(PrivateDisableApiKeyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Enables affilate program for user</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateEnableAffiliateProgram(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Enables api key with given id</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> PrivateEnableApiKey(PrivateEnableApiKeyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Lists access logs for the user</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<AccessLogEntry[]>> PrivateGetAccessLog(PrivateGetAccessLogRequest? args = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves user account summary.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<AccountSummaryData>> PrivateGetAccountSummary(PrivateGetAccountSummaryRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves user's affiliates count, payouts and link.</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<AffiliateProgramInfo>> PrivateGetAffiliateProgramInfo(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the language to be used for emails.</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateGetEmailLanguage(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves announcements that have not been marked read by the user.</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Announcement[]>> PrivateGetNewAnnouncements(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Calculates portfolio margin info for simulated position or current position of the user. This request has special restricted rate limit (not more than once per a second).</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<JObject>> PrivateGetPortfolioMargins(PrivateGetPortfolioMarginsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve user position.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserPosition>> PrivateGetPosition(PrivateGetPositionRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve user positions.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserPosition[]>> PrivateGetPositions(PrivateGetPositionsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Get information about subaccounts</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<SubAccount[]>> PrivateGetSubaccounts(PrivateGetSubaccountsRequest? args = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Get subaccounts positions</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<SubAccountDetail[]>> PrivateGetSubaccountsDetails(PrivateGetSubaccountsDetailsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest user trades that have occurred for a specific instrument and within given time range.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<TransactionLogPage>> PrivateGetTransactionLog(PrivateGetTransactionLogRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves information about locks on user account</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserLockEntry[]>> PrivateGetUserLocks(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves list of api keys</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData[]>> PrivateListApiKeys(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Removes api key</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateRemoveApiKey(PrivateRemoveApiKeyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Remove empty subaccount.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateRemoveSubaccount(PrivateRemoveSubaccountRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Resets secret in api key</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> PrivateResetApiKey(PrivateResetApiKeyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Marks an announcement as read, so it will not be shown in <c>get_new_announcements</c>.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateSetAnnouncementAsRead(PrivateSetAnnouncementAsReadRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Sets key with given id as default one for API Console</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> PrivateSetApiKeyAsDefault(PrivateSetApiKeyAsDefaultRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Assign an email address to a subaccount. User will receive an email with confirmation link.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateSetEmailForSubaccount(PrivateSetEmailForSubaccountRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Changes the language to be used for emails.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateSetEmailLanguage(PrivateSetEmailLanguageRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Set the password for the subaccount</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateSetPasswordForSubaccount(PrivateSetPasswordForSubaccountRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Enable or disable sending of notifications for the subaccount.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateToggleNotificationsFromSubaccount(PrivateToggleNotificationsFromSubaccountRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Toggle between SM and PM models</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<PortfolioMarginingToggleEntry[]>> PrivateTogglePortfolioMargining(PrivateTogglePortfolioMarginingRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Enable or disable login for a subaccount. If login is disabled and a session for the subaccount exists, this session will be terminated.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> PrivateToggleSubaccountLogin(PrivateToggleSubaccountLoginRequest args, CancellationToken cancellationToken = default(CancellationToken));
  }
}