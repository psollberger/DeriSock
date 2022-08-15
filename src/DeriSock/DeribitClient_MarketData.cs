namespace DeriSock;

using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient : IMarketDataApi
{
  /// <inheritdoc cref="IMarketDataApi.PublicGetBookSummaryByCurrency" />
  public async Task<JsonRpcResponse<PublicGetBookSummaryByCurrencyResponse>> PublicGetBookSummaryByCurrency(PublicGetBookSummaryByCurrencyRequest args)
    => await Send("public/get_book_summary_by_currency", args, new ObjectJsonConverter<PublicGetBookSummaryByCurrencyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetBookSummaryByInstrument" />
  public async Task<JsonRpcResponse<PublicGetBookSummaryByInstrumentResponse>> PublicGetBookSummaryByInstrument(PublicGetBookSummaryByInstrumentRequest args)
    => await Send("public/get_book_summary_by_instrument", args, new ObjectJsonConverter<PublicGetBookSummaryByInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetContractSize" />
  public async Task<JsonRpcResponse<PublicGetContractSizeResponse>> PublicGetContractSize(PublicGetContractSizeRequest args)
    => await Send("public/get_contract_size", args, new ObjectJsonConverter<PublicGetContractSizeResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetCurrencies" />
  public async Task<JsonRpcResponse<PublicGetCurrenciesResponse>> PublicGetCurrencies()
    => await Send("public/get_currencies", null, new ObjectJsonConverter<PublicGetCurrenciesResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetDeliveryPrices" />
  public async Task<JsonRpcResponse<PublicGetDeliveryPricesResponse>> PublicGetDeliveryPrices(PublicGetDeliveryPricesRequest args)
    => await Send("public/get_delivery_prices", args, new ObjectJsonConverter<PublicGetDeliveryPricesResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetFundingChartData" />
  public async Task<JsonRpcResponse<PublicGetFundingChartDataResponse>> PublicGetFundingChartData(PublicGetFundingChartDataRequest args)
    => await Send("public/get_funding_chart_data", args, new ObjectJsonConverter<PublicGetFundingChartDataResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetFundingRateHistory" />
  public async Task<JsonRpcResponse<PublicGetFundingRateHistoryResponse>> PublicGetFundingRateHistory(PublicGetFundingRateHistoryRequest args)
    => await Send("public/get_funding_rate_history", args, new ObjectJsonConverter<PublicGetFundingRateHistoryResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetFundingRateValue" />
  public async Task<JsonRpcResponse<double>> PublicGetFundingRateValue(PublicGetFundingRateValueRequest args)
    => await Send("public/get_funding_rate_value", args, new ObjectJsonConverter<double>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetHistoricalVolatility" />
  public async Task<JsonRpcResponse<TimestampValueItem[]>> PublicGetHistoricalVolatility(PublicGetHistoricalVolatilityRequest args)
    => await Send("public/get_historical_volatility", args, new ObjectJsonConverter<TimestampValueItem[]>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetIndex" />
  public async Task<JsonRpcResponse<PublicGetIndexResponse>> PublicGetIndex(PublicGetIndexRequest args)
    => await Send("public/get_index", args, new ObjectJsonConverter<PublicGetIndexResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetIndexPrice" />
  public async Task<JsonRpcResponse<PublicGetIndexPriceResponse>> PublicGetIndexPrice(PublicGetIndexPriceRequest args)
    => await Send("public/get_index_price", args, new ObjectJsonConverter<PublicGetIndexPriceResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetIndexPriceNames" />
  public async Task<JsonRpcResponse<string[]>> PublicGetIndexPriceNames()
    => await Send("public/get_index_price_names", null, new ObjectJsonConverter<string[]>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetInstrument" />
  public async Task<JsonRpcResponse<PublicGetInstrumentResponse>> PublicGetInstrument(PublicGetInstrumentRequest args)
    => await Send("public/get_instrument", args, new ObjectJsonConverter<PublicGetInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetInstruments" />
  public async Task<JsonRpcResponse<PublicGetInstrumentsResponse>> PublicGetInstruments(PublicGetInstrumentsRequest args)
    => await Send("public/get_instruments", args, new ObjectJsonConverter<PublicGetInstrumentsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetLastSettlementsByCurrency" />
  public async Task<JsonRpcResponse<PublicGetLastSettlementsByCurrencyResponse>> PublicGetLastSettlementsByCurrency(PublicGetLastSettlementsByCurrencyRequest args)
    => await Send("public/get_last_settlements_by_currency", args, new ObjectJsonConverter<PublicGetLastSettlementsByCurrencyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetLastSettlementsByInstrument" />
  public async Task<JsonRpcResponse<PublicGetLastSettlementsByInstrumentResponse>> PublicGetLastSettlementsByInstrument(PublicGetLastSettlementsByInstrumentRequest args)
    => await Send("public/get_last_settlements_by_instrument", args, new ObjectJsonConverter<PublicGetLastSettlementsByInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetLastTradesByCurrency" />
  public async Task<JsonRpcResponse<PublicGetLastTradesByCurrencyResponse>> PublicGetLastTradesByCurrency(PublicGetLastTradesByCurrencyRequest args)
    => await Send("public/get_last_trades_by_currency", args, new ObjectJsonConverter<PublicGetLastTradesByCurrencyResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetLastTradesByCurrencyAndTime" />
  public async Task<JsonRpcResponse<PublicGetLastTradesByCurrencyAndTimeResponse>> PublicGetLastTradesByCurrencyAndTime(PublicGetLastTradesByCurrencyAndTimeRequest args)
    => await Send("public/get_last_trades_by_currency_and_time", args, new ObjectJsonConverter<PublicGetLastTradesByCurrencyAndTimeResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetLastTradesByInstrument" />
  public async Task<JsonRpcResponse<PublicGetLastTradesByInstrumentResponse>> PublicGetLastTradesByInstrument(PublicGetLastTradesByInstrumentRequest args)
    => await Send("public/get_last_trades_by_instrument", args, new ObjectJsonConverter<PublicGetLastTradesByInstrumentResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetLastTradesByInstrumentAndTime" />
  public async Task<JsonRpcResponse<PublicGetLastTradesByInstrumentAndTimeResponse>> PublicGetLastTradesByInstrumentAndTime(PublicGetLastTradesByInstrumentAndTimeRequest args)
    => await Send("public/get_last_trades_by_instrument_and_time", args, new ObjectJsonConverter<PublicGetLastTradesByInstrumentAndTimeResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetMarkPriceHistory" />
  public async Task<JsonRpcResponse<TimestampValueItem[]>> PublicGetMarkPriceHistory(PublicGetMarkPriceHistoryRequest args)
    => await Send("public/get_mark_price_history", args, new ObjectJsonConverter<TimestampValueItem[]>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetOrderBook" />
  public async Task<JsonRpcResponse<PublicGetOrderBookResponse>> PublicGetOrderBook(PublicGetOrderBookRequest args)
    => await Send("public/get_order_book", args, new ObjectJsonConverter<PublicGetOrderBookResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetOrderBookByInstrumentId" />
  public async Task<JsonRpcResponse<PublicGetOrderBookByInstrumentIdResponse>> PublicGetOrderBookByInstrumentId(PublicGetOrderBookByInstrumentIdRequest args)
    => await Send("public/get_order_book_by_instrument_id", args, new ObjectJsonConverter<PublicGetOrderBookByInstrumentIdResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetRfqs" />
  public async Task<JsonRpcResponse<PublicGetRfqsResponse>> PublicGetRfqs(PublicGetRfqsRequest args)
    => await Send("public/get_rfqs", args, new ObjectJsonConverter<PublicGetRfqsResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetTradeVolumes" />
  public async Task<JsonRpcResponse<PublicGetTradeVolumesResponse>> PublicGetTradeVolumes(PublicGetTradeVolumesRequest? args)
    => await Send("public/get_trade_volumes", args, new ObjectJsonConverter<PublicGetTradeVolumesResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetTradingviewChartData" />
  public async Task<JsonRpcResponse<PublicGetTradingviewChartDataResponse>> PublicGetTradingviewChartData(PublicGetTradingviewChartDataRequest args)
    => await Send("public/get_tradingview_chart_data", args, new ObjectJsonConverter<PublicGetTradingviewChartDataResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicGetVolatilityIndexData" />
  public async Task<JsonRpcResponse<PublicGetVolatilityIndexDataResponse>> PublicGetVolatilityIndexData(PublicGetVolatilityIndexDataRequest args)
    => await Send("public/get_volatility_index_data", args, new ObjectJsonConverter<PublicGetVolatilityIndexDataResponse>()).ConfigureAwait(false);

  /// <inheritdoc cref="IMarketDataApi.PublicTicker" />
  public async Task<JsonRpcResponse<PublicTickerResponse>> PublicTicker(PublicTickerRequest args)
    => await Send("public/ticker", args, new ObjectJsonConverter<PublicTickerResponse>()).ConfigureAwait(false);
}
