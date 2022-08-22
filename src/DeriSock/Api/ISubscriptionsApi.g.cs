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
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.Net.JsonRpc;
  using DeriSock.Model;
  
  [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
  public partial interface ISubscriptionsApi
  {
    /// <summary>
    /// <para>General announcements concerning the Deribit platform.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<AnnouncementChange>> SubscribeAnnouncements(params AnnouncementsChannel[] channels);
    /// <summary>
    /// <para>Notifies about changes to the order book for a certain instrument.<br/>
    ///Notifications are sent once per specified interval, with prices grouped by (rounded to) the specified group, showing the complete order book to the specified depth (number of price levels).<br/>
    ///The &apos;asks&apos; and the &apos;bids&apos; elements in the response are both a &apos;list of lists&apos;. Each element in the outer list is a list of exactly two elements: price and amount.<br/>
    ///<c>price</c> is a price level (USD per BTC, rounded as specified by the &apos;group&apos; parameter for the susbcription).<br/>
    ///<c>amount</c> indicates the amount of all orders at price level. For perpetual and futures the amount is in USD units, for options it is amount of corresponding cryptocurrency contracts, e.g., BTC or ETH.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<GroupedOrderBookChange>> SubscribeBookChangesGrouped(params BookChangesGroupedChannel[] channels);
    /// <summary>
    /// <para>Notifies about changes to the order book for a certain instrument.<br/>
    ///The first notification will contain the whole book (bid and ask amounts for all prices). After that there will only be information about changes to individual price levels.<br/>
    ///The first notification will contain the amounts for all price levels (list of <c>[&apos;new&apos;, price, amount]</c> tuples). All following notifications will contain a list of tuples with action, price level and new amount (<c>[action, price, amount]</c>). Action can be either <c>new</c>, <c>change</c> or <c>delete</c>.<br/>
    ///Each notification will contain a <c>change_id</c> field, and each message except for the first one will contain a field <c>prev_change_id</c>. If <c>prev_change_id</c> is equal to the <c>change_id</c> of the previous message, this means that no messages have been missed.<br/>
    ///The amount for perpetual and futures is in USD units, for options it is in corresponding cryptocurrency contracts, e.g., BTC or ETH.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<OrderBookChange>> SubscribeBookChanges(params BookChangesChannel[] channels);
    /// <summary>
    /// <para>Publicly available market data used to generate a TradingView candle chart. During single resolution period, many events can be sent, each with updated values for the recent period.<br/>
    ///<b>NOTICE</b> When there is no trade during the requested resolution period (e.g. 1 minute), then filling sample is generated which uses data from the last available trade candle (open and close values).</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<TradingViewCandleChange>> SubscribeChartTrades(params ChartTradesChannel[] channels);
    /// <summary>
    /// <para>Provides information about current value (price) for Deribit Index</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<DeribitPriceIndex>> SubscribeDeribitPriceIndex(params DeribitPriceIndexChannel[] channels);
    /// <summary>
    /// <para>Provides information about current value (price) for stock exchanges used to calculate Deribit Index</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<DeribitPriceRankingEntry>> SubscribeDeribitPriceRanking(params DeribitPriceRankingChannel[] channels);
    /// <summary>
    /// <para>This subscription provides basic statistics about Deribit Index</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<DeribitPriceStatistics>> SubscribeDeribitPriceStatistics(params DeribitPriceStatisticsChannel[] channels);
    /// <summary>
    /// <para>Provides volatility index subscription</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<DeribitVolatilityIndex>> SubscribeDeribitVolatilityIndex(params DeribitVolatilityIndexChannel[] channels);
    /// <summary>
    /// <para>Returns calculated (estimated) ending price for given index.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<ExpirationPrice>> SubscribeEstimatedExpirationPrice(params EstimatedExpirationPriceChannel[] channels);
    /// <summary>
    /// <para>Notifies about changes in instrument ticker (key information about the instrument).<br/>
    ///The first notification will contain the whole ticker. After that there will only information about changes in the ticker.<br/>
    ///This event is send at most once per second.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<IncrementalTickerNotification>> SubscribeIncrementalTicker(params IncrementalTickerChannel[] channels);
    /// <summary>
    /// <para>Get notifications about new or terminated instruments of given kind in given currency.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<InstrumentState>> SubscribeInstrumentState(params InstrumentStateChannel[] channels);
    /// <summary>
    /// <para>Provides information about options markprices.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<OptionMarkprice>> SubscribeMarkpriceOptions(params MarkpriceOptionsChannel[] channels);
    /// <summary>
    /// <para>Provide current interest rate - but only for <b>perpetual</b> instruments. Other types won&apos;t generate any notification.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<PerpetualInterestRate>> SubscribePerpetual(params PerpetualChannel[] channels);
    /// <summary>
    /// <para>Information about platform state.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<PlatformStateChange>> SubscribePlatformState(params PlatformStateChannel[] channels);
    /// <summary>
    /// <para>Information whether unauthorized public requests are allowed</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<PlatformPublicMethodsStateChange>> SubscribePlatformPublicMethodsState(params PlatformPublicMethodsStateChannel[] channels);
    /// <summary>
    /// <para>Best bid/ask price and size.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<QuoteNotification>> SubscribeQuote(params QuoteChannel[] channels);
    /// <summary>
    /// <para>Get notifications about RFQs for instruments in given currency.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<RfqNotification>> SubscribeRequestForQuote(params RequestForQuoteChannel[] channels);
    /// <summary>
    /// <para>Key information about the instrument</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<TickerData>> SubscribeTicker(params TickerChannel[] channels);
    /// <summary>
    /// <para>Get notifications about trades for an instrument.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<PublicTrade>> SubscribeInstrumentTrades(params InstrumentTradesChannel[] channels);
    /// <summary>
    /// <para>Get notifications about trades in any instrument of a given kind and given currency.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<PublicTrade>> SubscribeKindCurrencyTrades(params KindCurrencyTradesChannel[] channels);
    /// <summary>
    /// <para>Get notifications about security events related to the account</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<AccessLogEntry>> SubscribeUserAccessLog(params UserAccessLogChannel[] channels);
    /// <summary>
    /// <para>Get notifications about user&apos;s updates related to order, trades, etc. in an instrument.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserChange>> SubscribeUserInstrumentChanges(params UserInstrumentChangesChannel[] channels);
    /// <summary>
    /// <para>Get notifications about changes in user&apos;s updates related to order, trades, etc. in instruments of a given kind and currency.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserChange>> SubscribeUserKindCurrencyChanges(params UserKindCurrencyChangesChannel[] channels);
    /// <summary>
    /// <para>Get notificiation when account is locked/unlocked</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserLockNotification>> SubscribeUserLock(params UserLockChannel[] channels);
    /// <summary>
    /// <para>Triggered when one of mmp limits is crossed</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserMmpFreeze>> SubscribeUserMmpTrigger(params UserMmpTriggerChannel[] channels);
    /// <summary>
    /// <para>Get notifications about changes in user&apos;s orders for given instrument.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserOrder>> SubscribeUserOrdersInstrumentChangeRaw(params UserOrdersInstrumentChangeRawChannel[] channels);
    /// <summary>
    /// <para>Get notifications about changes in user&apos;s orders for given instrument.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserOrder>> SubscribeUserOrdersInstrumentChange(params UserOrdersInstrumentChangeChannel[] channels);
    /// <summary>
    /// <para>Get notifications about changes in user&apos;s orders in instruments of a given kind and currency.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserOrder>> SubscribeUserOrdersKindCurrencyChangeRaw(params UserOrdersKindCurrencyChangeRawChannel[] channels);
    /// <summary>
    /// <para>Get notifications about changes in user&apos;s orders in instruments of a given kind and currency.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserOrder>> SubscribeUserOrdersKindCurrencyChange(params UserOrdersKindCurrencyChangeChannel[] channels);
    /// <summary>
    /// <para>Provides information about current user portfolio</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserPortfolioNotification>> SubscribeUserPortfolio(params UserPortfolioChannel[] channels);
    /// <summary>
    /// <para>Get notifications about user&apos;s trades in an instrument.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserTrade>> SubscribeUserTradesInstrumentChange(params UserTradesInstrumentChangeChannel[] channels);
    /// <summary>
    /// <para>Get notifications about user&apos;s trades in any instrument of a given kind and given currency.</para>
    /// </summary>
    /// <remarks>Don't forget to use this stream with <see cref="System.Threading.Tasks.TaskAsyncEnumerableExtensions.WithCancellation{T}"/>.</remarks>
    /// <param name="channels"></param>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "0.3.5")]
    Task<NotificationStream<UserTrade>> SubscribeUserTradesKindCurrencyChange(params UserTradesKindCurrencyChangeChannel[] channels);
  }
}