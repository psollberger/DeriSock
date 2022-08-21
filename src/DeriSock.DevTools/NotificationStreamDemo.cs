namespace DeriSock.DevTools;

using System;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Model;

public class NotificationStreamDemo
{
  private static readonly Stats stats = new();
  private static DateTime lastDraw;
  private static (int Left, int Top) CursorPosition = (-1, -1);

  public static async Task Run(CancellationToken cancellationToken)
  {
    try {
      var client = new DeribitClient(EndpointType.Testnet);
      await client.Connect(CancellationToken.None);

      var stream = await client.Subscriptions.SubscribeBookChanges(
                     new BookChangesChannel
                     {
                       InstrumentName = "BTC-PERPETUAL",
                       Interval = NotificationInterval2._100ms
                     },
                     new BookChangesChannel
                     {
                       InstrumentName = "ETH-PERPETUAL",
                       Interval = NotificationInterval2._100ms
                     },
                     new BookChangesChannel
                     {
                       InstrumentName = "BTC-28OCT22-25000-P",
                       Interval = NotificationInterval2._100ms
                     },
                     new BookChangesChannel
                     {
                       InstrumentName = "BTC-2SEP22-26000-C",
                       Interval = NotificationInterval2._100ms
                     });

      await foreach (var item in stream.WithCancellation(cancellationToken)) {
        switch (item.Channel) {
          case "book.BTC-PERPETUAL.100ms":
            stats.CountBtcPerp++;
            break;

          case "book.ETH-PERPETUAL.100ms":
            stats.CountEthPerp++;
            break;

          case "book.BTC-28OCT22-25000-P.100ms":
            stats.CountPut++;
            break;

          case "book.BTC-2SEP22-26000-C.100ms":
            stats.CountCall++;
            break;
        }

        Redraw();
      }

      await client.Disconnect(CancellationToken.None);
    }
    catch (Exception ex) {
      Console.WriteLine(ex);
      throw;
    }
  }

  public static void Redraw()
  {
    if (lastDraw.AddSeconds(1) >= DateTime.Now)
      return;

    lastDraw = DateTime.Now;

    if (CursorPosition == (-1, -1))
      CursorPosition = Console.GetCursorPosition();

    Console.SetCursorPosition(CursorPosition.Left, CursorPosition.Top);

    Console.WriteLine($"BTC-PERPETUAL        : {stats.CountBtcPerp}");
    Console.WriteLine($"ETH-PERPETUAL        : {stats.CountEthPerp}");
    Console.WriteLine($"BTC-28OCT22-25000-P  : {stats.CountPut}");
    Console.WriteLine($"BTC-2SEP22-26000-C   : {stats.CountCall}");
  }

  private class Stats
  {
    public int CountBtcPerp;
    public int CountEthPerp;
    public int CountPut;
    public int CountCall;
  }
}
