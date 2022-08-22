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
  using DeriSock.JsonRpc;
  using DeriSock.Model;
  using Newtonsoft.Json.Linq;
  
  public partial class DeribitClient
  {
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    private sealed partial class PrivateApiImpl : IPrivateApi
    {
      private readonly DeribitClient _client;
      public PrivateApiImpl(DeribitClient client)
      {
        _client = client;
      }
      /// <inheritdoc cref="IPrivateApi.Logout" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      void IPrivateApi.Logout(PrivateLogoutRequest? args)
      {
        _client.InternalPrivateLogout(args);
      }
      /// <inheritdoc cref="IPrivateApi.EnableCancelOnDisconnect" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.EnableCancelOnDisconnect(PrivateEnableCancelOnDisconnectRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateEnableCancelOnDisconnect(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.DisableCancelOnDisconnect" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.DisableCancelOnDisconnect(PrivateDisableCancelOnDisconnectRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateDisableCancelOnDisconnect(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetCancelOnDisconnect" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<CancelOnDisconnectData>> IPrivateApi.GetCancelOnDisconnect(PrivateGetCancelOnDisconnectRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetCancelOnDisconnect(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.Buy" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrderTrades>> IPrivateApi.Buy(PrivateBuyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateBuy(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.Sell" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrderTrades>> IPrivateApi.Sell(PrivateSellRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSell(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.Edit" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrderTrades>> IPrivateApi.Edit(PrivateEditRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateEdit(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.EditByLabel" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrderTrades>> IPrivateApi.EditByLabel(PrivateEditByLabelRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateEditByLabel(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.Cancel" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrder>> IPrivateApi.Cancel(PrivateCancelRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancel(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CancelAll" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<decimal>> IPrivateApi.CancelAll(PrivateCancelAllRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelAll(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CancelAllByCurrency" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<decimal>> IPrivateApi.CancelAllByCurrency(PrivateCancelAllByCurrencyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelAllByCurrency(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CancelAllByInstrument" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<decimal>> IPrivateApi.CancelAllByInstrument(PrivateCancelAllByInstrumentRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelAllByInstrument(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CancelByLabel" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<decimal>> IPrivateApi.CancelByLabel(PrivateCancelByLabelRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelByLabel(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ClosePosition" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrderTrades>> IPrivateApi.ClosePosition(PrivateClosePositionRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateClosePosition(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetMargins" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<InstrumentMargin>> IPrivateApi.GetMargins(PrivateGetMarginsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetMargins(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetMmpConfig" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<MmpConfig>> IPrivateApi.GetMmpConfig(PrivateGetMmpConfigRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetMmpConfig(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetOpenOrdersByCurrency" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrder[]>> IPrivateApi.GetOpenOrdersByCurrency(PrivateGetOpenOrdersByCurrencyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetOpenOrdersByCurrency(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetOpenOrdersByInstrument" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrder[]>> IPrivateApi.GetOpenOrdersByInstrument(PrivateGetOpenOrdersByInstrumentRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetOpenOrdersByInstrument(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetOrderHistoryByCurrency" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrder[]>> IPrivateApi.GetOrderHistoryByCurrency(PrivateGetOrderHistoryByCurrencyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetOrderHistoryByCurrency(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetOrderHistoryByInstrument" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrder[]>> IPrivateApi.GetOrderHistoryByInstrument(PrivateGetOrderHistoryByInstrumentRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetOrderHistoryByInstrument(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetOrderMarginByIds" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<OrderInitialMargin[]>> IPrivateApi.GetOrderMarginByIds(PrivateGetOrderMarginByIdsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetOrderMarginByIds(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetOrderState" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserOrder>> IPrivateApi.GetOrderState(PrivateGetOrderStateRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetOrderState(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetTriggerOrderHistory" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<TriggerOrderHistoryPage>> IPrivateApi.GetTriggerOrderHistory(PrivateGetTriggerOrderHistoryRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetTriggerOrderHistory(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetUserTradesByCurrency" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTrades>> IPrivateApi.GetUserTradesByCurrency(PrivateGetUserTradesByCurrencyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetUserTradesByCurrency(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetUserTradesByCurrencyAndTime" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTrades>> IPrivateApi.GetUserTradesByCurrencyAndTime(PrivateGetUserTradesByCurrencyAndTimeRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetUserTradesByCurrencyAndTime(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetUserTradesByInstrument" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTrades>> IPrivateApi.GetUserTradesByInstrument(PrivateGetUserTradesByInstrumentRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetUserTradesByInstrument(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetUserTradesByInstrumentAndTime" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTrades>> IPrivateApi.GetUserTradesByInstrumentAndTime(PrivateGetUserTradesByInstrumentAndTimeRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetUserTradesByInstrumentAndTime(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetUserTradesByOrder" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTrade[]>> IPrivateApi.GetUserTradesByOrder(PrivateGetUserTradesByOrderRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetUserTradesByOrder(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ResetMmp" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.ResetMmp(PrivateResetMmpRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateResetMmp(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SendRfq" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.SendRfq(PrivateSendRfqRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSendRfq(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SetMmpConfig" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.SetMmpConfig(PrivateSetMmpConfigRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetMmpConfig(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetSettlementHistoryByInstrument" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<SettlementPage>> IPrivateApi.GetSettlementHistoryByInstrument(PrivateGetSettlementHistoryByInstrumentRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetSettlementHistoryByInstrument(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetSettlementHistoryByCurrency" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<SettlementPage>> IPrivateApi.GetSettlementHistoryByCurrency(PrivateGetSettlementHistoryByCurrencyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetSettlementHistoryByCurrency(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CreateCombo" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Combo>> IPrivateApi.CreateCombo(PrivateCreateComboRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCreateCombo(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ExecuteBlockTrade" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<BlockTrade>> IPrivateApi.ExecuteBlockTrade(PrivateExecuteBlockTradeRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateExecuteBlockTrade(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetBlockTrade" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<BlockTrade>> IPrivateApi.GetBlockTrade(PrivateGetBlockTradeRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetBlockTrade(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetLastBlockTradesByCurrency" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<BlockTrade[]>> IPrivateApi.GetLastBlockTradesByCurrency(PrivateGetLastBlockTradesByCurrencyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetLastBlockTradesByCurrency(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.InvalidateBlockTradeSignature" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.InvalidateBlockTradeSignature(PrivateInvalidateBlockTradeSignatureRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateInvalidateBlockTradeSignature(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.MovePositions" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<MovePositionResponse>> IPrivateApi.MovePositions(PrivateMovePositionsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateMovePositions(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.VerifyBlockTrade" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<VerifyBlockTradeResponse>> IPrivateApi.VerifyBlockTrade(PrivateVerifyBlockTradeRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateVerifyBlockTrade(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CancelTransferById" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfer>> IPrivateApi.CancelTransferById(PrivateCancelTransferByIdRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelTransferById(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CancelWithdrawal" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Withdrawal>> IPrivateApi.CancelWithdrawal(PrivateCancelWithdrawalRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelWithdrawal(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CreateDepositAddress" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<DepositAddress>> IPrivateApi.CreateDepositAddress(PrivateCreateDepositAddressRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCreateDepositAddress(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetCurrentDepositAddress" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<DepositAddress>> IPrivateApi.GetCurrentDepositAddress(PrivateGetCurrentDepositAddressRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetCurrentDepositAddress(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetDeposits" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Deposits>> IPrivateApi.GetDeposits(PrivateGetDepositsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetDeposits(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetTransfers" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfers>> IPrivateApi.GetTransfers(PrivateGetTransfersRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetTransfers(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetWithdrawals" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Withdrawals>> IPrivateApi.GetWithdrawals(PrivateGetWithdrawalsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetWithdrawals(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SubmitTransferToSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfer>> IPrivateApi.SubmitTransferToSubaccount(PrivateSubmitTransferToSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSubmitTransferToSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SubmitTransferToUser" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfer>> IPrivateApi.SubmitTransferToUser(PrivateSubmitTransferToUserRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSubmitTransferToUser(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.Withdraw" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Withdrawal>> IPrivateApi.Withdraw(PrivateWithdrawRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateWithdraw(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ChangeApiKeyName" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> IPrivateApi.ChangeApiKeyName(PrivateChangeApiKeyNameRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateChangeApiKeyName(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ChangeScopeInApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> IPrivateApi.ChangeScopeInApiKey(PrivateChangeScopeInApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateChangeScopeInApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ChangeSubaccountName" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.ChangeSubaccountName(PrivateChangeSubaccountNameRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateChangeSubaccountName(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CreateApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> IPrivateApi.CreateApiKey(PrivateCreateApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCreateApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.CreateSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<CreateSubAccountResponse>> IPrivateApi.CreateSubaccount(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCreateSubaccount(cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.DisableApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> IPrivateApi.DisableApiKey(PrivateDisableApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateDisableApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.EnableAffiliateProgram" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.EnableAffiliateProgram(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateEnableAffiliateProgram(cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.EnableApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> IPrivateApi.EnableApiKey(PrivateEnableApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateEnableApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetAccessLog" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<AccessLogEntry[]>> IPrivateApi.GetAccessLog(PrivateGetAccessLogRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetAccessLog(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetAccountSummary" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<AccountSummaryData>> IPrivateApi.GetAccountSummary(PrivateGetAccountSummaryRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetAccountSummary(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetAffiliateProgramInfo" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<AffiliateProgramInfo>> IPrivateApi.GetAffiliateProgramInfo(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetAffiliateProgramInfo(cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetEmailLanguage" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.GetEmailLanguage(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetEmailLanguage(cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetNewAnnouncements" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Announcement[]>> IPrivateApi.GetNewAnnouncements(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetNewAnnouncements(cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetPortfolioMargins" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<JObject>> IPrivateApi.GetPortfolioMargins(PrivateGetPortfolioMarginsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetPortfolioMargins(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetPosition" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserPosition>> IPrivateApi.GetPosition(PrivateGetPositionRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetPosition(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetPositions" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserPosition[]>> IPrivateApi.GetPositions(PrivateGetPositionsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetPositions(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetSubaccounts" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<SubAccount[]>> IPrivateApi.GetSubaccounts(PrivateGetSubaccountsRequest? args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetSubaccounts(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetSubaccountsDetails" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<SubAccountDetail[]>> IPrivateApi.GetSubaccountsDetails(PrivateGetSubaccountsDetailsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetSubaccountsDetails(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetTransactionLog" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<TransactionLogPage>> IPrivateApi.GetTransactionLog(PrivateGetTransactionLogRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetTransactionLog(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.GetUserLocks" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserLockEntry[]>> IPrivateApi.GetUserLocks(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetUserLocks(cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ListApiKeys" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData[]>> IPrivateApi.ListApiKeys(CancellationToken cancellationToken)
      {
        return _client.InternalPrivateListApiKeys(cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.RemoveApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.RemoveApiKey(PrivateRemoveApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateRemoveApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.RemoveSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.RemoveSubaccount(PrivateRemoveSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateRemoveSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ResetApiKey" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> IPrivateApi.ResetApiKey(PrivateResetApiKeyRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateResetApiKey(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SetAnnouncementAsRead" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.SetAnnouncementAsRead(PrivateSetAnnouncementAsReadRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetAnnouncementAsRead(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SetApiKeyAsDefault" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<ApiKeyData>> IPrivateApi.SetApiKeyAsDefault(PrivateSetApiKeyAsDefaultRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetApiKeyAsDefault(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SetEmailForSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.SetEmailForSubaccount(PrivateSetEmailForSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetEmailForSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SetEmailLanguage" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.SetEmailLanguage(PrivateSetEmailLanguageRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetEmailLanguage(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.SetPasswordForSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.SetPasswordForSubaccount(PrivateSetPasswordForSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSetPasswordForSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ToggleNotificationsFromSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.ToggleNotificationsFromSubaccount(PrivateToggleNotificationsFromSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateToggleNotificationsFromSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.TogglePortfolioMargining" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<PortfolioMarginingToggleEntry[]>> IPrivateApi.TogglePortfolioMargining(PrivateTogglePortfolioMarginingRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateTogglePortfolioMargining(args, cancellationToken);
      }
      /// <inheritdoc cref="IPrivateApi.ToggleSubaccountLogin" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<string>> IPrivateApi.ToggleSubaccountLogin(PrivateToggleSubaccountLoginRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateToggleSubaccountLogin(args, cancellationToken);
      }
    }
  }
}