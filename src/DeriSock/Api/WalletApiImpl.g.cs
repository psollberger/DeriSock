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
    private sealed partial class WalletApiImpl : IWalletApi
    {
      private readonly DeribitClient _client;
      public WalletApiImpl(DeribitClient client)
      {
        _client = client;
      }
      /// <inheritdoc cref="IWalletApi.PrivateCancelTransferById" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfer>> IWalletApi.PrivateCancelTransferById(PrivateCancelTransferByIdRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelTransferById(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateCancelWithdrawal" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Withdrawal>> IWalletApi.PrivateCancelWithdrawal(PrivateCancelWithdrawalRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCancelWithdrawal(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateCreateDepositAddress" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<DepositAddress>> IWalletApi.PrivateCreateDepositAddress(PrivateCreateDepositAddressRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateCreateDepositAddress(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateGetCurrentDepositAddress" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<DepositAddress>> IWalletApi.PrivateGetCurrentDepositAddress(PrivateGetCurrentDepositAddressRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetCurrentDepositAddress(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateGetDeposits" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Deposits>> IWalletApi.PrivateGetDeposits(PrivateGetDepositsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetDeposits(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateGetTransfers" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfers>> IWalletApi.PrivateGetTransfers(PrivateGetTransfersRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetTransfers(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateGetWithdrawals" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Withdrawals>> IWalletApi.PrivateGetWithdrawals(PrivateGetWithdrawalsRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateGetWithdrawals(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateSubmitTransferToSubaccount" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfer>> IWalletApi.PrivateSubmitTransferToSubaccount(PrivateSubmitTransferToSubaccountRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSubmitTransferToSubaccount(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateSubmitTransferToUser" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<UserTransfer>> IWalletApi.PrivateSubmitTransferToUser(PrivateSubmitTransferToUserRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateSubmitTransferToUser(args, cancellationToken);
      }
      /// <inheritdoc cref="IWalletApi.PrivateWithdraw" />
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
      System.Threading.Tasks.Task<DeriSock.JsonRpc.JsonRpcResponse<Withdrawal>> IWalletApi.PrivateWithdraw(PrivateWithdrawRequest args, CancellationToken cancellationToken)
      {
        return _client.InternalPrivateWithdraw(args, cancellationToken);
      }
    }
  }
}