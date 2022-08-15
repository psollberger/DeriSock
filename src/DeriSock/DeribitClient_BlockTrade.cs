namespace DeriSock;

using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient : IBlockTradeApi
{
  /// <inheritdoc cref="IBlockTradeApi.PrivateExecuteBlockTrade" />
  public async Task<JsonRpcResponse<PrivateExecuteBlockTradeResponse>> PrivateExecuteBlockTrade(PrivateExecuteBlockTradeRequest args)
    => await Send("private/execute_block_trade", args, new ObjectJsonConverter<PrivateExecuteBlockTradeResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IBlockTradeApi.PrivateGetBlockTrade" />
  public async Task<JsonRpcResponse<PrivateGetBlockTradeResponse>> PrivateGetBlockTrade(PrivateGetBlockTradeRequest args)
    => await Send("private/get_block_trade", args, new ObjectJsonConverter<PrivateGetBlockTradeResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IBlockTradeApi.PrivateGetLastBlockTradesByCurrency" />
  public async Task<JsonRpcResponse<PrivateGetLastBlockTradesByCurrencyResponse>> PrivateGetLastBlockTradesByCurrency(PrivateGetLastBlockTradesByCurrencyRequest args)
    => await Send("private/get_last_block_trades_by_currency", args, new ObjectJsonConverter<PrivateGetLastBlockTradesByCurrencyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IBlockTradeApi.PrivateInvalidateBlockTradeSignature" />
  public async Task<JsonRpcResponse<string>> PrivateInvalidateBlockTradeSignature(PrivateInvalidateBlockTradeSignatureRequest args)
    => await Send("private/invalidate_block_trade_signature", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="IBlockTradeApi.PrivateMovePositions" />
  public async Task<JsonRpcResponse<PrivateMovePositionsResponse>> PrivateMovePositions(PrivateMovePositionsRequest args)
    => await Send("private/move_positions", args, new ObjectJsonConverter<PrivateMovePositionsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IBlockTradeApi.PrivateVerifyBlockTrade" />
  public async Task<JsonRpcResponse<PrivateVerifyBlockTradeResponse>> PrivateVerifyBlockTrade(PrivateVerifyBlockTradeRequest args)
    => await Send("private/verify_block_trade", args, new ObjectJsonConverter<PrivateVerifyBlockTradeResponse>()).ConfigureAwait(false);
}
