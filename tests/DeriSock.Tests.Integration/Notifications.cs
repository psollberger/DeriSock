namespace DeriSock.Tests.Integration;

using DeriSock.Model;

public class Notifications : IAsyncLifetime
{
  private readonly DeribitConfiguration _config;
  private readonly DeribitClient _client;

  public Notifications()
  {
    // Arrange
    _config = AppConfig.GetDeribitConfiguration();
    _client = new DeribitClient(EndpointType.Testnet);
  }

  [Fact]
  public async Task Subscription_Receives_Notifications()
  {
    // Act
    var notificationStream = await _client.Subscriptions.SubscribeBookChanges(
                               new BookChangesChannel
                               {
                                 InstrumentName = "BTC-PERPETUAL",
                                 Interval = NotificationInterval2._100ms
                               });

    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

    var receivedNotifications = new List<Notification<OrderBookChange>>();

    await foreach (var notification in notificationStream.WithCancellation(cts.Token))
      receivedNotifications.Add(notification);

    // Assert
    receivedNotifications.Should().NotBeEmpty("there has to be at least one notification received");
  }

  [Fact]
  public async Task Subscription_UserPortfolio_IsReceived_DespiteCaseChangeOfCurrency()
  {
    // Act
    var notificationStream = await _client.Subscriptions.SubscribeUserPortfolio(
                               new UserPortfolioChannel
                               {
                                 Currency = CurrencySymbol.BTC
                               });

    var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1));

    var receivedNotifications = new List<Notification<UserPortfolioNotification>>();

    await foreach (var notification in notificationStream.WithCancellation(cts.Token))
      receivedNotifications.Add(notification);

    // Assert
    receivedNotifications.Should().HaveCount(1, "only the initial portfolio should be delivered");
  }

  /// <inheritdoc />
  public async Task InitializeAsync()
  {
    await _client.Connect();
    await _client.Authentication.PublicLogin().WithClientSignature(_config.ClientId, _config.ClientSecret);
  }

  /// <inheritdoc />
  public async Task DisposeAsync()
  {
    if (_client.IsConnected)
      await _client.Disconnect();
  }
}
