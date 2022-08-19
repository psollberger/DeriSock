namespace DeriSock;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<BlockTrade>> InternalPrivateExecuteBlockTrade(PrivateExecuteBlockTradeRequest args, CancellationToken cancellationToken = default)
    => await Send("private/execute_block_trade", args, new ObjectJsonConverter<BlockTrade>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<BlockTrade>> InternalPrivateGetBlockTrade(PrivateGetBlockTradeRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_block_trade", args, new ObjectJsonConverter<BlockTrade>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<BlockTrade[]>> InternalPrivateGetLastBlockTradesByCurrency(PrivateGetLastBlockTradesByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_last_block_trades_by_currency", args, new ObjectJsonConverter<BlockTrade[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateInvalidateBlockTradeSignature(PrivateInvalidateBlockTradeSignatureRequest args, CancellationToken cancellationToken = default)
    => await Send("private/invalidate_block_trade_signature", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<MovePositionResponse>> InternalPrivateMovePositions(PrivateMovePositionsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/move_positions", args, new ObjectJsonConverter<MovePositionResponse>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<VerifyBlockTradeResponse>> InternalPrivateVerifyBlockTrade(PrivateVerifyBlockTradeRequest args, CancellationToken cancellationToken = default)
    => await Send("private/verify_block_trade", args, new ObjectJsonConverter<VerifyBlockTradeResponse>(), cancellationToken).ConfigureAwait(false);
}
