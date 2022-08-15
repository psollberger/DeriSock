namespace DeriSock.Tests.Integration
{
  using System.Net.WebSockets;

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
    public async Task Connect_WithInitialState_ResultsIn_WebSocketStateOpen()
    {
      // Act
      await _client.Connect();

      // Assert
      _client.State.Should().Be(WebSocketState.Open);
    }

    [Fact]
    public async Task Disconnect_WithOpenConnection_ResultsIn_WebSocketStateClosed()
    {
      // Act
      await _client.Connect();
      await _client.Disconnect();

      // Assert
      _client.State.Should().Be(WebSocketState.Closed);
    }

    [Fact]
    public async Task ReConnect_AfterDisconnect_ResultsIn_WebSocketStateOpen()
    {
      // Act
      await _client.Connect();
      await _client.Disconnect();
      await _client.Connect();

      // Assert
      _client.State.Should().Be(WebSocketState.Open);
    }

    /// <inheritdoc />
    public Task InitializeAsync()
    {
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task DisposeAsync()
    {
      if (_client.State is WebSocketState.Open)
        await _client.Disconnect();
    }
  }
}
