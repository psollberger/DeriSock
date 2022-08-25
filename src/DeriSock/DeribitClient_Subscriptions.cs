namespace DeriSock;

using System.Threading.Tasks;

using DeriSock.Model;

public partial class DeribitClient
{
  private async Task<NotificationStream<AnnouncementChange>> InternalSubscribeAnnouncements(params AnnouncementsChannel[] channels)
    => await _subscriptionManager.Subscribe<AnnouncementChange, AnnouncementsChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<GroupedOrderBookChange>> InternalSubscribeBookChangesGrouped(params BookChangesGroupedChannel[] channels)
    => await _subscriptionManager.Subscribe<GroupedOrderBookChange, BookChangesGroupedChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<OrderBookChange>> InternalSubscribeBookChanges(params BookChangesChannel[] channels)
    => await _subscriptionManager.Subscribe<OrderBookChange, BookChangesChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<TradingViewCandleChange>> InternalSubscribeChartTrades(params ChartTradesChannel[] channels)
    => await _subscriptionManager.Subscribe<TradingViewCandleChange, ChartTradesChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<DeribitPriceIndex>> InternalSubscribeDeribitPriceIndex(params DeribitPriceIndexChannel[] channels)
    => await _subscriptionManager.Subscribe<DeribitPriceIndex, DeribitPriceIndexChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<DeribitPriceRankingEntry[]>> InternalSubscribeDeribitPriceRanking(params DeribitPriceRankingChannel[] channels)
    => await _subscriptionManager.Subscribe<DeribitPriceRankingEntry[], DeribitPriceRankingChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<DeribitPriceStatistics>> InternalSubscribeDeribitPriceStatistics(params DeribitPriceStatisticsChannel[] channels)
    => await _subscriptionManager.Subscribe<DeribitPriceStatistics, DeribitPriceStatisticsChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<DeribitVolatilityIndex>> InternalSubscribeDeribitVolatilityIndex(params DeribitVolatilityIndexChannel[] channels)
    => await _subscriptionManager.Subscribe<DeribitVolatilityIndex, DeribitVolatilityIndexChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<ExpirationPrice>> InternalSubscribeEstimatedExpirationPrice(params EstimatedExpirationPriceChannel[] channels)
    => await _subscriptionManager.Subscribe<ExpirationPrice, EstimatedExpirationPriceChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<IncrementalTickerNotification>> InternalSubscribeIncrementalTicker(params IncrementalTickerChannel[] channels)
    => await _subscriptionManager.Subscribe<IncrementalTickerNotification, IncrementalTickerChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<InstrumentState>> InternalSubscribeInstrumentState(params InstrumentStateChannel[] channels)
    => await _subscriptionManager.Subscribe<InstrumentState, InstrumentStateChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<OptionMarkprice[]>> InternalSubscribeMarkpriceOptions(params MarkpriceOptionsChannel[] channels)
    => await _subscriptionManager.Subscribe<OptionMarkprice[], MarkpriceOptionsChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<PerpetualInterestRate>> InternalSubscribePerpetual(params PerpetualChannel[] channels)
    => await _subscriptionManager.Subscribe<PerpetualInterestRate, PerpetualChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<PlatformStateChange>> InternalSubscribePlatformState(params PlatformStateChannel[] channels)
    => await _subscriptionManager.Subscribe<PlatformStateChange, PlatformStateChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<PlatformPublicMethodsStateChange>> InternalSubscribePlatformPublicMethodsState(params PlatformPublicMethodsStateChannel[] channels)
    => await _subscriptionManager.Subscribe<PlatformPublicMethodsStateChange, PlatformPublicMethodsStateChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<QuoteNotification>> InternalSubscribeQuote(params QuoteChannel[] channels)
    => await _subscriptionManager.Subscribe<QuoteNotification, QuoteChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<RfqNotification>> InternalSubscribeRequestForQuote(params RequestForQuoteChannel[] channels)
    => await _subscriptionManager.Subscribe<RfqNotification, RequestForQuoteChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<TickerData>> InternalSubscribeTicker(params TickerChannel[] channels)
    => await _subscriptionManager.Subscribe<TickerData, TickerChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<PublicTrade>> InternalSubscribeInstrumentTrades(params InstrumentTradesChannel[] channels)
    => await _subscriptionManager.Subscribe<PublicTrade, InstrumentTradesChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<PublicTrade[]>> InternalSubscribeKindCurrencyTrades(params KindCurrencyTradesChannel[] channels)
    => await _subscriptionManager.Subscribe<PublicTrade[], KindCurrencyTradesChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<AccessLogEntry>> InternalSubscribeUserAccessLog(params UserAccessLogChannel[] channels)
    => await _subscriptionManager.Subscribe<AccessLogEntry, UserAccessLogChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserChange[]>> InternalSubscribeUserInstrumentChanges(params UserInstrumentChangesChannel[] channels)
    => await _subscriptionManager.Subscribe<UserChange[], UserInstrumentChangesChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserChange[]>> InternalSubscribeUserKindCurrencyChanges(params UserKindCurrencyChangesChannel[] channels)
    => await _subscriptionManager.Subscribe<UserChange[], UserKindCurrencyChangesChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserLockNotification>> InternalSubscribeUserLock(params UserLockChannel[] channels)
    => await _subscriptionManager.Subscribe<UserLockNotification, UserLockChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserMmpFreeze>> InternalSubscribeUserMmpTrigger(params UserMmpTriggerChannel[] channels)
    => await _subscriptionManager.Subscribe<UserMmpFreeze, UserMmpTriggerChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserOrder>> InternalSubscribeUserOrdersInstrumentChangeRaw(params UserOrdersInstrumentChangeRawChannel[] channels)
    => await _subscriptionManager.Subscribe<UserOrder, UserOrdersInstrumentChangeRawChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserOrder[]>> InternalSubscribeUserOrdersInstrumentChange(params UserOrdersInstrumentChangeChannel[] channels)
    => await _subscriptionManager.Subscribe<UserOrder[], UserOrdersInstrumentChangeChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserOrder>> InternalSubscribeUserOrdersKindCurrencyChangeRaw(params UserOrdersKindCurrencyChangeRawChannel[] channels)
    => await _subscriptionManager.Subscribe<UserOrder, UserOrdersKindCurrencyChangeRawChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserOrder[]>> InternalSubscribeUserOrdersKindCurrencyChange(params UserOrdersKindCurrencyChangeChannel[] channels)
    => await _subscriptionManager.Subscribe<UserOrder[], UserOrdersKindCurrencyChangeChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserPortfolioNotification>> InternalSubscribeUserPortfolio(params UserPortfolioChannel[] channels)
    => await _subscriptionManager.Subscribe<UserPortfolioNotification, UserPortfolioChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserTrade[]>> InternalSubscribeUserTradesInstrumentChange(params UserTradesInstrumentChangeChannel[] channels)
    => await _subscriptionManager.Subscribe<UserTrade[], UserTradesInstrumentChangeChannel>(channels).ConfigureAwait(false);

  private async Task<NotificationStream<UserTrade[]>> InternalSubscribeUserTradesKindCurrencyChange(params UserTradesKindCurrencyChangeChannel[] channels)
    => await _subscriptionManager.Subscribe<UserTrade[], UserTradesKindCurrencyChangeChannel>(channels).ConfigureAwait(false);
}
