namespace DeriSock;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<UserOrderTrades>> InternalPrivateBuy(PrivateBuyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/buy", args, new ObjectJsonConverter<UserOrderTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrderTrades>> InternalPrivateSell(PrivateSellRequest args, CancellationToken cancellationToken = default)
    => await Send("private/sell", args, new ObjectJsonConverter<UserOrderTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrderTrades>> InternalPrivateEdit(PrivateEditRequest args, CancellationToken cancellationToken = default)
    => await Send("private/edit", args, new ObjectJsonConverter<UserOrderTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrderTrades>> InternalPrivateEditByLabel(PrivateEditByLabelRequest args, CancellationToken cancellationToken = default)
    => await Send("private/edit_by_label", args, new ObjectJsonConverter<UserOrderTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrder>> InternalPrivateCancel(PrivateCancelRequest args, CancellationToken cancellationToken = default)
    => await Send("private/cancel", args, new ObjectJsonConverter<UserOrder>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<decimal>> InternalPrivateCancelAll(PrivateCancelAllRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("private/cancel_all", args, new ObjectJsonConverter<decimal>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<decimal>> InternalPrivateCancelAllByCurrency(PrivateCancelAllByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/cancel_all_by_currency", args, new ObjectJsonConverter<decimal>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<decimal>> InternalPrivateCancelAllByInstrument(PrivateCancelAllByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("private/cancel_all_by_instrument", args, new ObjectJsonConverter<decimal>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<decimal>> InternalPrivateCancelByLabel(PrivateCancelByLabelRequest args, CancellationToken cancellationToken = default)
    => await Send("private/cancel_by_label", args, new ObjectJsonConverter<decimal>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrderTrades>> InternalPrivateClosePosition(PrivateClosePositionRequest args, CancellationToken cancellationToken = default)
    => await Send("private/close_position", args, new ObjectJsonConverter<UserOrderTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<InstrumentMargin>> InternalPrivateGetMargins(PrivateGetMarginsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_margins", args, new ObjectJsonConverter<InstrumentMargin>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<MmpConfig>> InternalPrivateGetMmpConfig(PrivateGetMmpConfigRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_mmp_config", args, new ObjectJsonConverter<MmpConfig>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrder[]>> InternalPrivateGetOpenOrdersByCurrency(PrivateGetOpenOrdersByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_open_orders_by_currency", args, new ObjectJsonConverter<UserOrder[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrder[]>> InternalPrivateGetOpenOrdersByInstrument(PrivateGetOpenOrdersByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_open_orders_by_instrument", args, new ObjectJsonConverter<UserOrder[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrder[]>> InternalPrivateGetOrderHistoryByCurrency(PrivateGetOrderHistoryByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_order_history_by_currency", args, new ObjectJsonConverter<UserOrder[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrder[]>> InternalPrivateGetOrderHistoryByInstrument(PrivateGetOrderHistoryByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_order_history_by_instrument", args, new ObjectJsonConverter<UserOrder[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<OrderInitialMargin[]>> InternalPrivateGetOrderMarginByIds(PrivateGetOrderMarginByIdsRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_order_margin_by_ids", args, new ObjectJsonConverter<OrderInitialMargin[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserOrder>> InternalPrivateGetOrderState(PrivateGetOrderStateRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_order_state", args, new ObjectJsonConverter<UserOrder>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<TriggerOrderHistoryPage>> InternalPrivateGetTriggerOrderHistory(PrivateGetTriggerOrderHistoryRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_trigger_order_history", args, new ObjectJsonConverter<TriggerOrderHistoryPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTrades>> InternalPrivateGetUserTradesByCurrency(PrivateGetUserTradesByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_user_trades_by_currency", args, new ObjectJsonConverter<UserTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTrades>> InternalPrivateGetUserTradesByCurrencyAndTime(PrivateGetUserTradesByCurrencyAndTimeRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_user_trades_by_currency_and_time", args, new ObjectJsonConverter<UserTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTrades>> InternalPrivateGetUserTradesByInstrument(PrivateGetUserTradesByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_user_trades_by_instrument", args, new ObjectJsonConverter<UserTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTrades>> InternalPrivateGetUserTradesByInstrumentAndTime(PrivateGetUserTradesByInstrumentAndTimeRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_user_trades_by_instrument_and_time", args, new ObjectJsonConverter<UserTrades>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<UserTrade[]>> InternalPrivateGetUserTradesByOrder(PrivateGetUserTradesByOrderRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_user_trades_by_order", args, new ObjectJsonConverter<UserTrade[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateResetMmp(PrivateResetMmpRequest args, CancellationToken cancellationToken = default)
    => await Send("private/reset_mmp", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateSendRfq(PrivateSendRfqRequest args, CancellationToken cancellationToken = default)
    => await Send("private/send_rfq", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateSetMmpConfig(PrivateSetMmpConfigRequest args, CancellationToken cancellationToken = default)
    => await Send("private/set_mmp_config", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<SettlementPage>> InternalPrivateGetSettlementHistoryByInstrument(PrivateGetSettlementHistoryByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_settlement_history_by_instrument", args, new ObjectJsonConverter<SettlementPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<SettlementPage>> InternalPrivateGetSettlementHistoryByCurrency(PrivateGetSettlementHistoryByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("private/get_settlement_history_by_currency", args, new ObjectJsonConverter<SettlementPage>(), cancellationToken).ConfigureAwait(false);
}
