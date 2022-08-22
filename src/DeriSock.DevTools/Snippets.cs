namespace DeriSock.DevTools;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Model;

public class Snippets
{
  public async Task Readme()
  {
    // begin-snippet: readme-how-to-use
    var client = new DeribitClient(EndpointType.Testnet);
    await client.Connect();

    var response = await client.Public.GetOrderBook(
                     new PublicGetOrderBookRequest
                     {
                       InstrumentName = "BTC-PERPETUAL"
                     });

    var bestBidPrice = response.Data.BestBidPrice;

    await client.Disconnect();

    // end-snippet

    // begin-snippet: readme-auth-credentials
    await client.Authentication.PublicLogin()
      .WithClientCredentials(
        "<client id",
        "<client secret>",
        "<optional state>",
        "<optional scope>");

    // end-snippet

    // begin-snippet: readme-auth-signature
    await client.Authentication.PublicLogin()
      .WithClientSignature(
        "<client id",
        "<client secret>",
        "<optional state>",
        "<optional scope>");

    // end-snippet

    // begin-snippet: readme-auth-logout
    client.Authentication.PrivateLogout();

    // end-snippet

    // begin-snippet: readme-subscribtion-usage
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

    // Create a CancellationTokenSource to be able to stop the stream
    // (i.e. unsubscribe from the channel(s))
    var cts = new CancellationTokenSource();

    // An IAsyncEnumerable<T> stream that will listen to incoming notifications as long as
    // cts.Cancel(); is not called.
    await foreach (var notification in subscriptionStream.WithCancellation(cts.Token)) {
      //Here you can do something with the received information.
      var bookChangeId = notification.Data.ChangeId;
    }
    // end-snippet
  }
}