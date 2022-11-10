namespace DeriSock.DevTools.UseCases;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Model;

using JetBrains.Annotations;

using Microsoft.Extensions.Logging;

using Serilog;

[UsedImplicitly]
internal sealed class ScratchPad : IUseCase
{
  private readonly ILogger<ScratchPad> _logger;
  private readonly DeribitApiKey _apiKey;

  private int _receivedTickerCount;

  /// <summary>
  ///   Initializes a new instance of the <see cref="ScratchPad" /> class.
  /// </summary>
  public ScratchPad(ILogger<ScratchPad> logger, DeribitApiKey apiKey)
  {
    _logger = logger;
    _apiKey = apiKey;
  }

  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var client = new DeribitClient(EndpointType.Testnet, logger: Log.Logger);

    _logger.LogTrace("Connecting ...");
    await client.Connect(cancellationToken);

    var startTime = DateTimeOffset.UtcNow;

    try
    {
      _logger.LogTrace("Authenticating ...");
      await client.Authentication.PublicLogin().WithClientSignature(_apiKey.ClientId, _apiKey.ClientSecret, cancellationToken: cancellationToken);

      _logger.LogTrace("Get list of instruments ...");
      var instruments = await client.MarketData.PublicGetInstruments(new PublicGetInstrumentsRequest { Currency = CurrencySymbol.BTC }, cancellationToken);

      var tickerChannels = instruments.Data!.Select(
          instrument => new TickerChannel
          {
            InstrumentName = instrument.InstrumentName,
            Interval = NotificationInterval2._100ms
          }
        )
        .ToArray();

      _logger.LogTrace("Subscribe to ticker channels ...");
      var tickerStream = await client.Subscriptions.SubscribeTicker(tickerChannels);

      _logger.LogTrace("Listening ...");
      _ = TickerSub(tickerStream, cancellationToken);

      var updateDelay = TimeSpan.FromSeconds(10);

      while (!cancellationToken.IsCancellationRequested)
      {
        await Task.Delay(updateDelay, cancellationToken);
        _logger.LogTrace(@"Connected for '{ConnectedForTimeSpan:hh\:mm\:ss\.fff}', notifications received: {ReceivedTickerCount:D7}", DateTimeOffset.UtcNow - startTime, _receivedTickerCount);
      }
    }
    catch (TaskCanceledException)
    {
      _logger.LogTrace("Logging out ...");
      client.Authentication.PrivateLogout();
    }
  }

//  private async Task BookSub(NotificationStream<OrderBookChange> notificationStream, CancellationToken cancellationToken)
//  {
//    await foreach (var book in notificationStream.WithCancellation(cancellationToken))
//      _logger.LogTrace("{BookChannel} {DataInstrumentName}", book.Channel, book.Data.InstrumentName);
//  }

  private async Task TickerSub(NotificationStream<TickerData> notificationStream, CancellationToken cancellationToken)
  {
    await foreach (var _ in notificationStream.WithCancellation(cancellationToken))
      ++_receivedTickerCount;
  }
}
