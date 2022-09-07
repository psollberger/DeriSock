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
  using DeriSock.Net.JsonRpc;
  using DeriSock.Model;
  
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
  public partial interface ITradingApi
  {
    /// <summary>
    /// <para>Places a buy order for an instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrderTrades>> PrivateBuy(UserOrderRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Places a sell order for an instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrderTrades>> PrivateSell(UserOrderRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Change price, amount and/or other properties of an order.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrderTrades>> PrivateEdit(PrivateEditRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Change price, amount and/or other properties of an order with given label. It works only when there is one open order with this label</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrderTrades>> PrivateEditByLabel(PrivateEditByLabelRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Cancel an order, specified by order id</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrder>> PrivateCancel(UserOrderIdRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>This method cancels all users orders and trigger orders within all currencies and instrument kinds.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<decimal>> PrivateCancelAll(PrivateCancelAllRequest? args = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Cancels all orders by currency, optionally filtered by instrument kind and/or order type.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<decimal>> PrivateCancelAllByCurrency(PrivateCancelAllByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Cancels all orders by instrument, optionally filtered by order type.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<decimal>> PrivateCancelAllByInstrument(PrivateCancelAllByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Cancels orders by label. All user&apos;s orders (trigger orders too), with given label are canceled in all currencies or in one given currency (in this case currency queue is used)</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<decimal>> PrivateCancelByLabel(PrivateCancelByLabelRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Makes closing position reduce only order .</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrderTrades>> PrivateClosePosition(PrivateClosePositionRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Get margins for given instrument, amount and price.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<InstrumentMargin>> PrivateGetMargins(PrivateGetMarginsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Get current config for MMP</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<MmpConfig>> PrivateGetMmpConfig(PrivateGetMmpConfigRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves list of user&apos;s open orders.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrder[]>> PrivateGetOpenOrdersByCurrency(PrivateGetOpenOrdersByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves list of user&apos;s open orders within given Instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrder[]>> PrivateGetOpenOrdersByInstrument(PrivateGetOpenOrdersByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves history of orders that have been partially or fully filled.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrder[]>> PrivateGetOrderHistoryByCurrency(PrivateGetOrderHistoryByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves history of orders that have been partially or fully filled.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrder[]>> PrivateGetOrderHistoryByInstrument(PrivateGetOrderHistoryByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves initial margins of given orders</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<OrderInitialMargin[]>> PrivateGetOrderMarginByIds(PrivateGetOrderMarginByIdsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the current state of an order.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserOrder>> PrivateGetOrderState(UserOrderIdRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves detailed log of the user&apos;s trigger orders.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<TriggerOrderHistoryPage>> PrivateGetTriggerOrderHistory(PrivateGetTriggerOrderHistoryRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest user trades that have occurred for instruments in a specific currency symbol.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserTrades>> PrivateGetUserTradesByCurrency(PrivateGetUserTradesByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest user trades that have occurred for instruments in a specific currency symbol and within given time range.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserTrades>> PrivateGetUserTradesByCurrencyAndTime(PrivateGetUserTradesByCurrencyAndTimeRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest user trades that have occurred for a specific instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserTrades>> PrivateGetUserTradesByInstrument(PrivateGetUserTradesByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest user trades that have occurred for a specific instrument and within given time range.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserTrades>> PrivateGetUserTradesByInstrumentAndTime(PrivateGetUserTradesByInstrumentAndTimeRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the list of user trades that was created for given order</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<UserTrade[]>> PrivateGetUserTradesByOrder(PrivateGetUserTradesByOrderRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Reset MMP</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> PrivateResetMmp(PrivateResetMmpRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Sends RFQ on a given instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> PrivateSendRfq(PrivateSendRfqRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Set config for MMP - triggers MMP reset</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<string>> PrivateSetMmpConfig(PrivateSetMmpConfigRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves public settlement, delivery and bankruptcy events filtered by instrument name</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<SettlementPage>> PrivateGetSettlementHistoryByInstrument(PrivateGetSettlementHistoryByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves settlement, delivery and bankruptcy events that have affected your account.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    System.Threading.Tasks.Task<DeriSock.Net.JsonRpc.JsonRpcResponse<SettlementPage>> PrivateGetSettlementHistoryByCurrency(PrivateGetSettlementHistoryByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
  }
}
