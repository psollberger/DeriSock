namespace DeriSock;

using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient : IWalletApi
{
  /// <inheritdoc cref="IWalletApi" />
  IWalletApi ICategoriesApi.Wallet()
    => this;

  /// <inheritdoc cref="IWalletApi.PrivateCancelTransferById" />
  public async Task<JsonRpcResponse<PrivateCancelTransferByIdResponse>> PrivateCancelTransferById(PrivateCancelTransferByIdRequest args)
    => await Send("private/cancel_transfer_by_id", args, new ObjectJsonConverter<PrivateCancelTransferByIdResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateCancelWithdrawal" />
  public async Task<JsonRpcResponse<PrivateCancelWithdrawalResponse>> PrivateCancelWithdrawal(PrivateCancelWithdrawalRequest args)
    => await Send("private/cancel_withdrawal", args, new ObjectJsonConverter<PrivateCancelWithdrawalResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateCreateDepositAddress" />
  public async Task<JsonRpcResponse<PrivateCreateDepositAddressResponse>> PrivateCreateDepositAddress(PrivateCreateDepositAddressRequest args)
    => await Send("private/create_deposit_address", args, new ObjectJsonConverter<PrivateCreateDepositAddressResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateGetCurrentDepositAddress" />
  public async Task<JsonRpcResponse<PrivateGetCurrentDepositAddressResponse>> PrivateGetCurrentDepositAddress(PrivateGetCurrentDepositAddressRequest args)
    => await Send("private/get_current_deposit_address", args, new ObjectJsonConverter<PrivateGetCurrentDepositAddressResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateGetDeposits" />
  public async Task<JsonRpcResponse<PrivateGetDepositsResponse>> PrivateGetDeposits(PrivateGetDepositsRequest args)
    => await Send("private/get_deposits", args, new ObjectJsonConverter<PrivateGetDepositsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateGetTransfers" />
  public async Task<JsonRpcResponse<PrivateGetTransfersResponse>> PrivateGetTransfers(PrivateGetTransfersRequest args)
    => await Send("private/get_transfers", args, new ObjectJsonConverter<PrivateGetTransfersResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateGetWithdrawals" />
  public async Task<JsonRpcResponse<PrivateGetWithdrawalsResponse>> PrivateGetWithdrawals(PrivateGetWithdrawalsRequest args)
    => await Send("private/get_withdrawals", args, new ObjectJsonConverter<PrivateGetWithdrawalsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateSubmitTransferToSubaccount" />
  public async Task<JsonRpcResponse<PrivateSubmitTransferToSubaccountResponse>> PrivateSubmitTransferToSubaccount(PrivateSubmitTransferToSubaccountRequest args)
    => await Send("private/submit_transfer_to_subaccount", args, new ObjectJsonConverter<PrivateSubmitTransferToSubaccountResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateSubmitTransferToUser" />
  public async Task<JsonRpcResponse<PrivateSubmitTransferToUserResponse>> PrivateSubmitTransferToUser(PrivateSubmitTransferToUserRequest args)
    => await Send("private/submit_transfer_to_user", args, new ObjectJsonConverter<PrivateSubmitTransferToUserResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IWalletApi.PrivateWithdraw" />
  public async Task<JsonRpcResponse<PrivateWithdrawResponse>> PrivateWithdraw(PrivateWithdrawRequest args)
    => await Send("private/withdraw", args, new ObjectJsonConverter<PrivateWithdrawResponse>()).ConfigureAwait(false);
}
