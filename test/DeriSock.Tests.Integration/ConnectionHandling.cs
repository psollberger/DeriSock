namespace DeriSock.Tests.Integration;

public class ConnectionHandling : IAsyncLifetime
{
  private readonly DeribitConfiguration _config;
  private readonly DeribitClient _client;

  public ConnectionHandling()
  {
    // Arrange
    _config = AppConfig.GetDeribitConfiguration();
    _client = new DeribitClient(EndpointType.Testnet);
  }

  [Fact]
  public async Task Connect_WithInitialState_ResultsIn_IsConnected_True()
  {
    // Act
    await _client.Connect();

    // Assert
    _client.IsConnected.Should().Be(true, "the Connect method was called");
  }

  [Fact]
  public async Task Disconnect_WithOpenConnection_ResultsIn_IsConnected_False()
  {
    // Act
    await _client.Connect();
    await _client.Disconnect();

    // Assert
    _client.IsConnected.Should().Be(false, "the Disconnect method was called");
  }

  [Fact]
  public async Task ReConnect_AfterDisconnect_ResultsIn_IsConnected_True()
  {
    // Act
    await _client.Connect();
    await _client.Disconnect();
    await _client.Connect();

    // Assert
    _client.IsConnected.Should().Be(true, "the Connect method was called after a disconnect");
  }

  /// <inheritdoc />
  public Task InitializeAsync()
    => Task.CompletedTask;

  /// <inheritdoc />
  public async Task DisposeAsync()
  {
    if (_client.IsConnected)
      await _client.Disconnect();
  }
}
