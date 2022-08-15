namespace DeriSock;

using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient : ITradingApi
{
  /// <inheritdoc cref="ITradingApi" />
  ITradingApi ICategoriesApi.Trading()
    => this;

  /// <inheritdoc cref="ITradingApi.PrivateBuy" />
  public async Task<JsonRpcResponse<PrivateBuyResponse>> PrivateBuy(PrivateBuyRequest args)
    => await Send("private/buy", args, new ObjectJsonConverter<PrivateBuyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateSell" />
  public async Task<JsonRpcResponse<PrivateSellResponse>> PrivateSell(PrivateSellRequest args)
    => await Send("private/sell", args, new ObjectJsonConverter<PrivateSellResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateEdit" />
  public async Task<JsonRpcResponse<PrivateEditResponse>> PrivateEdit(PrivateEditRequest args)
    => await Send("private/edit", args, new ObjectJsonConverter<PrivateEditResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateEditByLabel" />
  public async Task<JsonRpcResponse<PrivateEditByLabelResponse>> PrivateEditByLabel(PrivateEditByLabelRequest args)
    => await Send("private/edit_by_label", args, new ObjectJsonConverter<PrivateEditByLabelResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateCancel" />
  public async Task<JsonRpcResponse<PrivateCancelResponse>> PrivateCancel(PrivateCancelRequest args)
    => await Send("private/cancel", args, new ObjectJsonConverter<PrivateCancelResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateCancelAll" />
  public async Task<JsonRpcResponse<decimal>> PrivateCancelAll(PrivateCancelAllRequest? args)
    => await Send("private/cancel_all", args, new ObjectJsonConverter<decimal>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateCancelAllByCurrency" />
  public async Task<JsonRpcResponse<decimal>> PrivateCancelAllByCurrency(PrivateCancelAllByCurrencyRequest args)
    => await Send("private/cancel_all_by_currency", args, new ObjectJsonConverter<decimal>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateCancelAllByInstrument" />
  public async Task<JsonRpcResponse<decimal>> PrivateCancelAllByInstrument(PrivateCancelAllByInstrumentRequest args)
    => await Send("private/cancel_all_by_instrument", args, new ObjectJsonConverter<decimal>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateCancelByLabel" />
  public async Task<JsonRpcResponse<decimal>> PrivateCancelByLabel(PrivateCancelByLabelRequest args)
    => await Send("private/cancel_by_label", args, new ObjectJsonConverter<decimal>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateClosePosition" />
  public async Task<JsonRpcResponse<PrivateClosePositionResponse>> PrivateClosePosition(PrivateClosePositionRequest args)
    => await Send("private/close_position", args, new ObjectJsonConverter<PrivateClosePositionResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetMargins" />
  public async Task<JsonRpcResponse<PrivateGetMarginsResponse>> PrivateGetMargins(PrivateGetMarginsRequest args)
    => await Send("private/get_margins", args, new ObjectJsonConverter<PrivateGetMarginsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetMmpConfig" />
  public async Task<JsonRpcResponse<PrivateGetMmpConfigResponse>> PrivateGetMmpConfig(PrivateGetMmpConfigRequest args)
    => await Send("private/get_mmp_config", args, new ObjectJsonConverter<PrivateGetMmpConfigResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetOpenOrdersByCurrency" />
  public async Task<JsonRpcResponse<PrivateGetOpenOrdersByCurrencyResponse>> PrivateGetOpenOrdersByCurrency(PrivateGetOpenOrdersByCurrencyRequest args)
    => await Send("private/get_open_orders_by_currency", args, new ObjectJsonConverter<PrivateGetOpenOrdersByCurrencyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetOpenOrdersByInstrument" />
  public async Task<JsonRpcResponse<PrivateGetOpenOrdersByInstrumentResponse>> PrivateGetOpenOrdersByInstrument(PrivateGetOpenOrdersByInstrumentRequest args)
    => await Send("private/get_open_orders_by_instrument", args, new ObjectJsonConverter<PrivateGetOpenOrdersByInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetOrderHistoryByCurrency" />
  public async Task<JsonRpcResponse<PrivateGetOrderHistoryByCurrencyResponse>> PrivateGetOrderHistoryByCurrency(PrivateGetOrderHistoryByCurrencyRequest args)
    => await Send("private/get_order_history_by_currency", args, new ObjectJsonConverter<PrivateGetOrderHistoryByCurrencyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetOrderHistoryByInstrument" />
  public async Task<JsonRpcResponse<PrivateGetOrderHistoryByInstrumentResponse>> PrivateGetOrderHistoryByInstrument(PrivateGetOrderHistoryByInstrumentRequest args)
    => await Send("private/get_order_history_by_instrument", args, new ObjectJsonConverter<PrivateGetOrderHistoryByInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetOrderMarginByIds" />
  public async Task<JsonRpcResponse<PrivateGetOrderMarginByIdsResponse>> PrivateGetOrderMarginByIds(PrivateGetOrderMarginByIdsRequest args)
    => await Send("private/get_order_margin_by_ids", args, new ObjectJsonConverter<PrivateGetOrderMarginByIdsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetOrderState" />
  public async Task<JsonRpcResponse<PrivateGetOrderStateResponse>> PrivateGetOrderState(PrivateGetOrderStateRequest args)
    => await Send("private/get_order_state", args, new ObjectJsonConverter<PrivateGetOrderStateResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetTriggerOrderHistory" />
  public async Task<JsonRpcResponse<PrivateGetTriggerOrderHistoryResponse>> PrivateGetTriggerOrderHistory(PrivateGetTriggerOrderHistoryRequest args)
    => await Send("private/get_trigger_order_history", args, new ObjectJsonConverter<PrivateGetTriggerOrderHistoryResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetUserTradesByCurrency" />
  public async Task<JsonRpcResponse<PrivateGetUserTradesByCurrencyResponse>> PrivateGetUserTradesByCurrency(PrivateGetUserTradesByCurrencyRequest args)
    => await Send("private/get_user_trades_by_currency", args, new ObjectJsonConverter<PrivateGetUserTradesByCurrencyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetUserTradesByCurrencyAndTime" />
  public async Task<JsonRpcResponse<PrivateGetUserTradesByCurrencyAndTimeResponse>> PrivateGetUserTradesByCurrencyAndTime(PrivateGetUserTradesByCurrencyAndTimeRequest args)
    => await Send("private/get_user_trades_by_currency_and_time", args, new ObjectJsonConverter<PrivateGetUserTradesByCurrencyAndTimeResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetUserTradesByInstrument" />
  public async Task<JsonRpcResponse<PrivateGetUserTradesByInstrumentResponse>> PrivateGetUserTradesByInstrument(PrivateGetUserTradesByInstrumentRequest args)
    => await Send("private/get_user_trades_by_instrument", args, new ObjectJsonConverter<PrivateGetUserTradesByInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetUserTradesByInstrumentAndTime" />
  public async Task<JsonRpcResponse<PrivateGetUserTradesByInstrumentAndTimeResponse>> PrivateGetUserTradesByInstrumentAndTime(PrivateGetUserTradesByInstrumentAndTimeRequest args)
    => await Send("private/get_user_trades_by_instrument_and_time", args, new ObjectJsonConverter<PrivateGetUserTradesByInstrumentAndTimeResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetUserTradesByOrder" />
  public async Task<JsonRpcResponse<PrivateGetUserTradesByOrderResponse>> PrivateGetUserTradesByOrder(PrivateGetUserTradesByOrderRequest args)
    => await Send("private/get_user_trades_by_order", args, new ObjectJsonConverter<PrivateGetUserTradesByOrderResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateResetMmp" />
  public async Task<JsonRpcResponse<string>> PrivateResetMmp(PrivateResetMmpRequest args)
    => await Send("private/reset_mmp", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateSendRfq" />
  public async Task<JsonRpcResponse<string>> PrivateSendRfq(PrivateSendRfqRequest args)
    => await Send("private/send_rfq", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateSetMmpConfig" />
  public async Task<JsonRpcResponse<string>> PrivateSetMmpConfig(PrivateSetMmpConfigRequest args)
    => await Send("private/set_mmp_config", args, new ObjectJsonConverter<string>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetSettlementHistoryByInstrument" />
  public async Task<JsonRpcResponse<PrivateGetSettlementHistoryByInstrumentResponse>> PrivateGetSettlementHistoryByInstrument(PrivateGetSettlementHistoryByInstrumentRequest args)
    => await Send("private/get_settlement_history_by_instrument", args, new ObjectJsonConverter<PrivateGetSettlementHistoryByInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="ITradingApi.PrivateGetSettlementHistoryByCurrency" />
  public async Task<JsonRpcResponse<PrivateGetSettlementHistoryByCurrencyResponse>> PrivateGetSettlementHistoryByCurrency(PrivateGetSettlementHistoryByCurrencyRequest args)
    => await Send("private/get_settlement_history_by_currency", args, new ObjectJsonConverter<PrivateGetSettlementHistoryByCurrencyResponse>()).ConfigureAwait(false);
}
