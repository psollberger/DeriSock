namespace DeriSock.DevTools;

using System;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Model;

public class ScratchPad
{
  public static async Task Run()
  {
    //var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
    //var map = await ApiDocUtils.ReadRequestMapAsync(ApiDocRequestMapPath).ConfigureAwait(false);

    ////await ApiDocUtils.CreateAndWriteRequestMapAsync(apiDoc, map, ApiDocRequestMapPath).ConfigureAwait(false);
    //if (map is not null)
    //  await ApiDocUtils.WriteRequestOverridesFromMapAsync(apiDoc, map, ApiDocRequestOverridesPath).ConfigureAwait(false);

    var client = new DeribitClient(EndpointType.Testnet);
    await client.Connect();

    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

    // Subscribe to one or more channels. 
    var subscriptionStream = await client.Subscriptions.SubscribeBookChanges(
                               new BookChangesChannel
                               {
                                 InstrumentName = "BTC-PERPETUAL",
                                 Interval = NotificationInterval2._100ms
                               },
                               new BookChangesChannel
                               {
                                 InstrumentName = "ETH-PERPETUAL",
                                 Interval = NotificationInterval2._100ms
                               });

    var tickersStream = await client.Subscriptions.SubscribeTicker(
                          new TickerChannel
                          {
                            InstrumentName = "BTC-PERPETUAL",
                            Interval = NotificationInterval2._100ms
                          },
                          new TickerChannel
                          {
                            InstrumentName = "ETH-PERPETUAL",
                            Interval = NotificationInterval2._100ms
                          });

    Console.WriteLine("awaiting the subscriptions");
    await Task.WhenAll(BookSub(subscriptionStream, cts.Token), TickerSub(tickersStream, cts.Token));
    Console.WriteLine("subscriptions done");
  }

  private static async Task BookSub(NotificationStream<OrderBookChange> notificationStream, CancellationToken cancellationToken)
  {
    await foreach (var book in notificationStream.WithCancellation(cancellationToken))
      Console.WriteLine($"{book.Channel} {book.Data.InstrumentName}");
  }

  private static async Task TickerSub(NotificationStream<TickerData> notificationStream, CancellationToken cancellationToken)
  {
    await foreach (var ticker in notificationStream.WithCancellation(cancellationToken))
      Console.WriteLine($"{ticker.Channel} {ticker.Data.InstrumentName} {ticker.Data.InstrumentType}");
  }
}
