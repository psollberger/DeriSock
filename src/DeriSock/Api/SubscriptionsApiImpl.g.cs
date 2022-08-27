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
namespace DeriSock
{
  using System;
  using System.Threading;
  using System.Threading.Tasks;
  using DeriSock.Api;
  using DeriSock.Net.JsonRpc;
  using DeriSock.Model;
  using Newtonsoft.Json.Linq;
  
  public partial class DeribitClient
  {
    [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
    private sealed partial class SubscriptionsApiImpl : ISubscriptionsApi
    {
      private readonly DeribitClient _client;
      public SubscriptionsApiImpl(DeribitClient client)
      {
        _client = client;
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeAnnouncements" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<AnnouncementChange>> ISubscriptionsApi.SubscribeAnnouncements(params AnnouncementsChannel[] channels)
      {
        return _client.InternalSubscribeAnnouncements(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeBookChangesGrouped" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<GroupedOrderBookChange>> ISubscriptionsApi.SubscribeBookChangesGrouped(params BookChangesGroupedChannel[] channels)
      {
        return _client.InternalSubscribeBookChangesGrouped(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeBookChanges" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<OrderBookChange>> ISubscriptionsApi.SubscribeBookChanges(params BookChangesChannel[] channels)
      {
        return _client.InternalSubscribeBookChanges(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeChartTrades" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<TradingViewCandleChange>> ISubscriptionsApi.SubscribeChartTrades(params ChartTradesChannel[] channels)
      {
        return _client.InternalSubscribeChartTrades(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeDeribitPriceIndex" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<DeribitPriceIndex>> ISubscriptionsApi.SubscribeDeribitPriceIndex(params DeribitPriceIndexChannel[] channels)
      {
        return _client.InternalSubscribeDeribitPriceIndex(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeDeribitPriceRanking" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<DeribitPriceRankingEntry[]>> ISubscriptionsApi.SubscribeDeribitPriceRanking(params DeribitPriceRankingChannel[] channels)
      {
        return _client.InternalSubscribeDeribitPriceRanking(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeDeribitPriceStatistics" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<DeribitPriceStatistics>> ISubscriptionsApi.SubscribeDeribitPriceStatistics(params DeribitPriceStatisticsChannel[] channels)
      {
        return _client.InternalSubscribeDeribitPriceStatistics(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeDeribitVolatilityIndex" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<DeribitVolatilityIndex>> ISubscriptionsApi.SubscribeDeribitVolatilityIndex(params DeribitVolatilityIndexChannel[] channels)
      {
        return _client.InternalSubscribeDeribitVolatilityIndex(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeEstimatedExpirationPrice" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<ExpirationPrice>> ISubscriptionsApi.SubscribeEstimatedExpirationPrice(params EstimatedExpirationPriceChannel[] channels)
      {
        return _client.InternalSubscribeEstimatedExpirationPrice(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeIncrementalTicker" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<IncrementalTickerNotification>> ISubscriptionsApi.SubscribeIncrementalTicker(params IncrementalTickerChannel[] channels)
      {
        return _client.InternalSubscribeIncrementalTicker(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeInstrumentState" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<InstrumentState>> ISubscriptionsApi.SubscribeInstrumentState(params InstrumentStateChannel[] channels)
      {
        return _client.InternalSubscribeInstrumentState(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeMarkpriceOptions" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<OptionMarkprice[]>> ISubscriptionsApi.SubscribeMarkpriceOptions(params MarkpriceOptionsChannel[] channels)
      {
        return _client.InternalSubscribeMarkpriceOptions(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribePerpetual" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<PerpetualInterestRate>> ISubscriptionsApi.SubscribePerpetual(params PerpetualChannel[] channels)
      {
        return _client.InternalSubscribePerpetual(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribePlatformState" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<PlatformStateChange>> ISubscriptionsApi.SubscribePlatformState(params PlatformStateChannel[] channels)
      {
        return _client.InternalSubscribePlatformState(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribePlatformPublicMethodsState" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<PlatformPublicMethodsStateChange>> ISubscriptionsApi.SubscribePlatformPublicMethodsState(params PlatformPublicMethodsStateChannel[] channels)
      {
        return _client.InternalSubscribePlatformPublicMethodsState(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeQuote" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<QuoteNotification>> ISubscriptionsApi.SubscribeQuote(params QuoteChannel[] channels)
      {
        return _client.InternalSubscribeQuote(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeRequestForQuote" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<RfqNotification>> ISubscriptionsApi.SubscribeRequestForQuote(params RequestForQuoteChannel[] channels)
      {
        return _client.InternalSubscribeRequestForQuote(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeTicker" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<TickerData>> ISubscriptionsApi.SubscribeTicker(params TickerChannel[] channels)
      {
        return _client.InternalSubscribeTicker(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeInstrumentTrades" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<PublicTrade>> ISubscriptionsApi.SubscribeInstrumentTrades(params InstrumentTradesChannel[] channels)
      {
        return _client.InternalSubscribeInstrumentTrades(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeKindCurrencyTrades" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<PublicTrade[]>> ISubscriptionsApi.SubscribeKindCurrencyTrades(params KindCurrencyTradesChannel[] channels)
      {
        return _client.InternalSubscribeKindCurrencyTrades(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserAccessLog" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<AccessLogEntry>> ISubscriptionsApi.SubscribeUserAccessLog(params UserAccessLogChannel[] channels)
      {
        return _client.InternalSubscribeUserAccessLog(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserInstrumentChanges" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserChange[]>> ISubscriptionsApi.SubscribeUserInstrumentChanges(params UserInstrumentChangesChannel[] channels)
      {
        return _client.InternalSubscribeUserInstrumentChanges(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserKindCurrencyChanges" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserChange[]>> ISubscriptionsApi.SubscribeUserKindCurrencyChanges(params UserKindCurrencyChangesChannel[] channels)
      {
        return _client.InternalSubscribeUserKindCurrencyChanges(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserLock" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserLockNotification>> ISubscriptionsApi.SubscribeUserLock(params UserLockChannel[] channels)
      {
        return _client.InternalSubscribeUserLock(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserMmpTrigger" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserMmpFreeze>> ISubscriptionsApi.SubscribeUserMmpTrigger(params UserMmpTriggerChannel[] channels)
      {
        return _client.InternalSubscribeUserMmpTrigger(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserOrdersInstrumentChangeRaw" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserOrder>> ISubscriptionsApi.SubscribeUserOrdersInstrumentChangeRaw(params UserOrdersInstrumentChangeRawChannel[] channels)
      {
        return _client.InternalSubscribeUserOrdersInstrumentChangeRaw(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserOrdersInstrumentChange" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserOrder[]>> ISubscriptionsApi.SubscribeUserOrdersInstrumentChange(params UserOrdersInstrumentChangeChannel[] channels)
      {
        return _client.InternalSubscribeUserOrdersInstrumentChange(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserOrdersKindCurrencyChangeRaw" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserOrder>> ISubscriptionsApi.SubscribeUserOrdersKindCurrencyChangeRaw(params UserOrdersKindCurrencyChangeRawChannel[] channels)
      {
        return _client.InternalSubscribeUserOrdersKindCurrencyChangeRaw(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserOrdersKindCurrencyChange" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserOrder[]>> ISubscriptionsApi.SubscribeUserOrdersKindCurrencyChange(params UserOrdersKindCurrencyChangeChannel[] channels)
      {
        return _client.InternalSubscribeUserOrdersKindCurrencyChange(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserPortfolio" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserPortfolioNotification>> ISubscriptionsApi.SubscribeUserPortfolio(params UserPortfolioChannel[] channels)
      {
        return _client.InternalSubscribeUserPortfolio(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserTradesInstrumentChange" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserTrade[]>> ISubscriptionsApi.SubscribeUserTradesInstrumentChange(params UserTradesInstrumentChangeChannel[] channels)
      {
        return _client.InternalSubscribeUserTradesInstrumentChange(channels);
      }
      /// <inheritdoc cref="ISubscriptionsApi.SubscribeUserTradesKindCurrencyChange" />
      /// <param name="channels"></param>
      [System.CodeDom.Compiler.GeneratedCodeAttribute("DeriSock.DevTools", "2.0.0")]
      Task<NotificationStream<UserTrade[]>> ISubscriptionsApi.SubscribeUserTradesKindCurrencyChange(params UserTradesKindCurrencyChangeChannel[] channels)
      {
        return _client.InternalSubscribeUserTradesKindCurrencyChange(channels);
      }
    }
  }
}
