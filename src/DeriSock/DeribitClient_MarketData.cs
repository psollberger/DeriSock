namespace DeriSock;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<BookSummaryEntry[]>> InternalPublicGetBookSummaryByCurrency(PublicGetBookSummaryByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_book_summary_by_currency", args, new ObjectJsonConverter<BookSummaryEntry[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<BookSummaryEntry[]>> InternalPublicGetBookSummaryByInstrument(PublicGetBookSummaryByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_book_summary_by_instrument", args, new ObjectJsonConverter<BookSummaryEntry[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ContractSizeData>> InternalPublicGetContractSize(PublicGetContractSizeRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_contract_size", args, new ObjectJsonConverter<ContractSizeData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<CurrencyData[]>> InternalPublicGetCurrencies(CancellationToken cancellationToken = default)
    => await Send("public/get_currencies", null, new ObjectJsonConverter<CurrencyData[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<IndexDeliveryPrices>> InternalPublicGetDeliveryPrices(PublicGetDeliveryPricesRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_delivery_prices", args, new ObjectJsonConverter<IndexDeliveryPrices>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<FundingChartData>> InternalPublicGetFundingChartData(PublicGetFundingChartDataRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_funding_chart_data", args, new ObjectJsonConverter<FundingChartData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<FundingRateHistoryEntry[]>> InternalPublicGetFundingRateHistory(PublicGetFundingRateHistoryRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_funding_rate_history", args, new ObjectJsonConverter<FundingRateHistoryEntry[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<double>> InternalPublicGetFundingRateValue(PublicGetFundingRateValueRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_funding_rate_value", args, new ObjectJsonConverter<double>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<TimestampValueItem[]>> InternalPublicGetHistoricalVolatility(PublicGetHistoricalVolatilityRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_historical_volatility", args, new ObjectJsonConverter<TimestampValueItem[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<GetIndexResponse>> InternalPublicGetIndex(PublicGetIndexRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_index", args, new ObjectJsonConverter<GetIndexResponse>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<IndexPriceData>> InternalPublicGetIndexPrice(PublicGetIndexPriceRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_index_price", args, new ObjectJsonConverter<IndexPriceData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string[]>> InternalPublicGetIndexPriceNames(CancellationToken cancellationToken = default)
    => await Send("public/get_index_price_names", null, new ObjectJsonConverter<string[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Instrument>> InternalPublicGetInstrument(PublicGetInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_instrument", args, new ObjectJsonConverter<Instrument>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<Instrument[]>> InternalPublicGetInstruments(PublicGetInstrumentsRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_instruments", args, new ObjectJsonConverter<Instrument[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<SettlementPage>> InternalPublicGetLastSettlementsByCurrency(PublicGetLastSettlementsByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_last_settlements_by_currency", args, new ObjectJsonConverter<SettlementPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<SettlementPage>> InternalPublicGetLastSettlementsByInstrument(PublicGetLastSettlementsByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_last_settlements_by_instrument", args, new ObjectJsonConverter<SettlementPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PublicTradesPage>> InternalPublicGetLastTradesByCurrency(PublicGetLastTradesByCurrencyRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_last_trades_by_currency", args, new ObjectJsonConverter<PublicTradesPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PublicTradesPage>> InternalPublicGetLastTradesByCurrencyAndTime(PublicGetLastTradesByCurrencyAndTimeRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_last_trades_by_currency_and_time", args, new ObjectJsonConverter<PublicTradesPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PublicTradesPage>> InternalPublicGetLastTradesByInstrument(PublicGetLastTradesByInstrumentRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_last_trades_by_instrument", args, new ObjectJsonConverter<PublicTradesPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PublicTradesPage>> InternalPublicGetLastTradesByInstrumentAndTime(PublicGetLastTradesByInstrumentAndTimeRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_last_trades_by_instrument_and_time", args, new ObjectJsonConverter<PublicTradesPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<TimestampValueItem[]>> InternalPublicGetMarkPriceHistory(PublicGetMarkPriceHistoryRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_mark_price_history", args, new ObjectJsonConverter<TimestampValueItem[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<OrderBook>> InternalPublicGetOrderBook(PublicGetOrderBookRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_order_book", args, new ObjectJsonConverter<OrderBook>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<OrderBook>> InternalPublicGetOrderBookByInstrumentId(PublicGetOrderBookByInstrumentIdRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_order_book_by_instrument_id", args, new ObjectJsonConverter<OrderBook>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<RfqEntry[]>> InternalPublicGetRfqs(PublicGetRfqsRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_rfqs", args, new ObjectJsonConverter<RfqEntry[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<AggregatedTradeVolume[]>> InternalPublicGetTradeVolumes(PublicGetTradeVolumesRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("public/get_trade_volumes", args, new ObjectJsonConverter<AggregatedTradeVolume[]>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<TradingViewChartData>> InternalPublicGetTradingviewChartData(PublicGetTradingviewChartDataRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_tradingview_chart_data", args, new ObjectJsonConverter<TradingViewChartData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<VolatilityIndexPage>> InternalPublicGetVolatilityIndexData(PublicGetVolatilityIndexDataRequest args, CancellationToken cancellationToken = default)
    => await Send("public/get_volatility_index_data", args, new ObjectJsonConverter<VolatilityIndexPage>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<TickerData>> InternalPublicTicker(PublicTickerRequest args, CancellationToken cancellationToken = default)
    => await Send("public/ticker", args, new ObjectJsonConverter<TickerData>(), cancellationToken).ConfigureAwait(false);
}
