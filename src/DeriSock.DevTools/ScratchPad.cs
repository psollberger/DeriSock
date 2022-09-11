namespace DeriSock.DevTools;

using System;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Model;

using Serilog;

public class ScratchPad
{
  public static async Task Run(CancellationToken cancellationToken)
  {
    //var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
    //var map = await ApiDocUtils.ReadRequestMapAsync(ApiDocRequestMapPath).ConfigureAwait(false);

    ////await ApiDocUtils.CreateAndWriteRequestMapAsync(apiDoc, map, ApiDocRequestMapPath).ConfigureAwait(false);
    //if (map is not null)
    //  await ApiDocUtils.WriteRequestOverridesFromMapAsync(apiDoc, map, ApiDocRequestOverridesPath).ConfigureAwait(false);

    await Task.CompletedTask;
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
