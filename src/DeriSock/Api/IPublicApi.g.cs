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
  using System;

  using DeriSock.Net.JsonRpc;

  using Newtonsoft.Json.Linq;
  
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial interface IPublicApi
  {
    /// <summary>
    /// <para>Generates token for new subject id. This method can be used to switch between subaccounts.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<AuthTokenData>> ExchangeToken(PublicExchangeTokenRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Generates token for new named session. This method can be used only with session scoped tokens.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<AuthTokenData>> ForkToken(PublicForkTokenRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Signals the Websocket connection to send and request heartbeats. Heartbeats can be used to detect stale connections. When heartbeats have been set up, the API server will send <c>heartbeat</c> messages and <c>test_request</c> messages. Your software should respond to <c>test_request</c> messages by sending a <c>/api/v2/public/test</c> request. If your software fails to do so, the API server will immediately close the connection. If your account is configured to cancel on disconnect, any orders opened over the connection will be cancelled.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<string>> SetHeartbeat(PublicSetHeartbeatRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Stop sending heartbeat messages.</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<string>> DisableHeartbeat(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the current time (in milliseconds). This API endpoint can be used to check the clock skew between your software and Deribit&apos;s systems.</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<DateTime>> GetTime(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Method used to introduce the client software connected to Deribit platform over websocket. Provided data may have an impact on the maintained connection and will be collected for internal statistical purposes. In response, Deribit will also introduce itself.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<ServerVersionData>> Hello(PublicHelloRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Method used to get information about locked currencies</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<PlatformLockStatus>> Status(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Tests the connection to the API server, and returns its version. You can use this to make sure the API is reachable, and matches the expected version.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<ServerVersionData>> Test(PublicTestRequest? args = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the summary information such as open interest, 24h volume, etc. for all instruments for the currency (optionally filtered by kind).</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<BookSummaryEntry[]>> GetBookSummaryByCurrency(PublicGetBookSummaryByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the summary information such as open interest, 24h volume, etc. for a specific instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<BookSummaryEntry[]>> GetBookSummaryByInstrument(PublicGetBookSummaryByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves contract size of provided instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<ContractSizeData>> GetContractSize(PublicGetContractSizeRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves all cryptocurrencies supported by the API.</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<CurrencyData[]>> GetCurrencies(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves delivery prices for then given index</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<IndexDeliveryPrices>> GetDeliveryPrices(PublicGetDeliveryPricesRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the list of the latest PERPETUAL funding chart points within a given time period.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<FundingChartData>> GetFundingChartData(PublicGetFundingChartDataRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves hourly historical interest rate for requested PERPETUAL instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<FundingRateHistoryEntry[]>> GetFundingRateHistory(PublicGetFundingRateHistoryRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves interest rate value for requested period. Applicable only for PERPETUAL instruments.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<double>> GetFundingRateValue(PublicGetFundingRateValueRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Provides information about historical volatility for given cryptocurrency.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<DeriSock.Model.TimestampValueItem[]>> GetHistoricalVolatility(PublicGetHistoricalVolatilityRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the current index price for the instruments, for the selected currency.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<GetIndexResponse>> GetIndex(PublicGetIndexRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the current index price value for given index name.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<IndexPriceData>> GetIndexPrice(PublicGetIndexPriceRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the identifiers of all supported Price Indexes</para>
    /// </summary>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<string[]>> GetIndexPriceNames(CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves information about instrument</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<Instrument>> GetInstrument(PublicGetInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves available trading instruments. This method can be used to see which instruments are available for trading, or which instruments have recently expired.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<Instrument[]>> GetInstruments(PublicGetInstrumentsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves historical settlement, delivery and bankruptcy events coming from all instruments within given currency.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<SettlementPage>> GetLastSettlementsByCurrency(PublicGetLastSettlementsByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves historical public settlement, delivery and bankruptcy events filtered by instrument name.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<SettlementPage>> GetLastSettlementsByInstrument(PublicGetLastSettlementsByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest trades that have occurred for instruments in a specific currency symbol.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<PublicTradesPage>> GetLastTradesByCurrency(PublicGetLastTradesByCurrencyRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest trades that have occurred for instruments in a specific currency symbol and within given time range.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<PublicTradesPage>> GetLastTradesByCurrencyAndTime(PublicGetLastTradesByCurrencyAndTimeRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest trades that have occurred for a specific instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<PublicTradesPage>> GetLastTradesByInstrument(PublicGetLastTradesByInstrumentRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve the latest trades that have occurred for a specific instrument and within given time range.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<PublicTradesPage>> GetLastTradesByInstrumentAndTime(PublicGetLastTradesByInstrumentAndTimeRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Public request for 5min history of markprice values for the instrument. For now the markprice history is available only for a subset of options which take part in the volatility index calculations. All other instruments, futures and perpetuals will return empty list.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<DeriSock.Model.TimestampValueItem[]>> GetMarkPriceHistory(PublicGetMarkPriceHistoryRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the order book, along with other market values for a given instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<OrderBook>> GetOrderBook(PublicGetOrderBookRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves the order book, along with other market values for a given instrument ID.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<OrderBook>> GetOrderBookByInstrumentId(PublicGetOrderBookByInstrumentIdRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieve active RFQs for instruments in given currency.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<RfqEntry[]>> GetRfqs(PublicGetRfqsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves aggregated 24h trade volumes for different instrument types and currencies.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<AggregatedTradeVolume[]>> GetTradeVolumes(PublicGetTradeVolumesRequest? args = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Publicly available market data used to generate a TradingView candle chart.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<TradingViewChartData>> GetTradingviewChartData(PublicGetTradingviewChartDataRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Public market data request for volatility index candles.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<VolatilityIndexPage>> GetVolatilityIndexData(PublicGetVolatilityIndexDataRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Get ticker for an instrument.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<TickerData>> Ticker(PublicTickerRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves information about a combo</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<Combo>> GetComboDetails(PublicGetComboDetailsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves available combos. This method can be used to get the list of all combos, or only the list of combos in the given state.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<string[]>> GetComboIds(PublicGetComboIdsRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves information about active combos</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<Combo[]>> GetCombos(PublicGetCombosRequest args, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Retrieves announcements. Default &quot;start_timestamp&quot; parameter value is current timestamp, &quot;count&quot; parameter value must be between 1 and 50, default is 5.</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<Announcement[]>> GetAnnouncements(PublicGetAnnouncementsRequest? args = null, CancellationToken cancellationToken = default(CancellationToken));
    /// <summary>
    /// <para>Public version of the method calculates portfolio margin info for simulated position. For concrete user position, the private version of the method must be used. The public version of the request has special restricted rate limit (not more than once per a second for the IP).</para>
    /// </summary>
    /// <param name="args"></param>
    /// <param name="cancellationToken"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    System.Threading.Tasks.Task<JsonRpcResponse<JObject>> GetPortfolioMargins(PublicGetPortfolioMarginsRequest args, CancellationToken cancellationToken = default(CancellationToken));
  }
}
