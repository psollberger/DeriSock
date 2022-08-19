namespace DeriSock;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<UserTransfer>> InternalPrivateCancelTransferById(PrivateCancelTransferByIdRequest args, CancellationToken cancellationToken = default)
    => await Send("private/cancel_transfer_by_id", args, new ObjectJsonConverter<UserTransfer>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Withdrawal>> InternalPrivateCancelWithdrawal(PrivateCancelWithdrawalRequest args, CancellationToken cancellationToken = default)
    => await Send("private/cancel_withdrawal", args, new ObjectJsonConverter<Withdrawal>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<DepositAddress>> InternalPrivateCreateDepositAddress(PrivateCreateDepositAddressRequest args, CancellationToken cancellationToken = default)
    => await Send("private/create_deposit_address", args, new ObjectJsonConverter<DepositAddress>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<DepositAddress>> InternalPrivateGetCurrentDepositAddress(PrivateGetCurrentDepositAddressRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_current_deposit_address", args, new ObjectJsonConverter<DepositAddress>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Deposits>> InternalPrivateGetDeposits(PrivateGetDepositsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_deposits", args, new ObjectJsonConverter<Deposits>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTransfers>> InternalPrivateGetTransfers(PrivateGetTransfersRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_transfers", args, new ObjectJsonConverter<UserTransfers>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Withdrawals>> InternalPrivateGetWithdrawals(PrivateGetWithdrawalsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_withdrawals", args, new ObjectJsonConverter<Withdrawals>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTransfer>> InternalPrivateSubmitTransferToSubaccount(PrivateSubmitTransferToSubaccountRequest args, CancellationToken cancellationToken = default)
    => await Send("private/submit_transfer_to_subaccount", args, new ObjectJsonConverter<UserTransfer>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTransfer>> InternalPrivateSubmitTransferToUser(PrivateSubmitTransferToUserRequest args, CancellationToken cancellationToken = default)
    => await Send("private/submit_transfer_to_user", args, new ObjectJsonConverter<UserTransfer>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Withdrawal>> InternalPrivateWithdraw(PrivateWithdrawRequest args, CancellationToken cancellationToken = default)
    => await Send("private/withdraw", args, new ObjectJsonConverter<Withdrawal>(), cancellationToken).ConfigureAwait(false);
}
