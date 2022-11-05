namespace DeriSock.Tests.Integration;

using DeriSock.Model;

public class NullabilityValidation : IAsyncLifetime
{
  private readonly DeribitConfiguration _config;
  private readonly DeribitClient _client;

  public NullabilityValidation()
  {
    // Arrange
    _config = AppConfig.GetDeribitConfiguration();
    _client = new DeribitClient(EndpointType.Testnet);
  }

  [Fact]
  public async Task RetrievingAllInstruments_ShouldNotThrow()
  {
    // Act
    await _client.Public.GetInstruments(new PublicGetInstrumentsRequest
    {
      Currency = CurrencySymbol.BTC
    });

    await _client.Public.GetInstruments(new PublicGetInstrumentsRequest
    {
      Currency = CurrencySymbol.ETH
    });

    await _client.Public.GetInstruments(new PublicGetInstrumentsRequest
    {
      Currency = CurrencySymbol.SOL
    });

    await _client.Public.GetInstruments(new PublicGetInstrumentsRequest
    {
      Currency = CurrencySymbol.USDC
    });

    // Assert
    // should not throw exception
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
