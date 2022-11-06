namespace DeriSock.Tests.Unit;

using DeriSock.Constants;
using DeriSock.Model;
using DeriSock.Net;
using DeriSock.Utils;

// ReSharper disable once InconsistentNaming
[UsesVerify]
public sealed class DeribitClient_Supporting
{
  private static readonly VerifySettings VerifySettings = new();
  private bool _messageClientIsConnected;
  private readonly ITextMessageClient _messageClient = Substitute.For<ITextMessageClient>();
  private readonly DeribitClient _sut;

  public DeribitClient_Supporting()
  {
    VerifySettings.UseDirectory("VerifyData");

    _sut = new DeribitClient(EndpointType.Testnet, _messageClient);

    _messageClient.IsConnected.Returns(_ => _messageClientIsConnected);
    _messageClient.When(x => x.Connect(Arg.Any<Uri>(), Arg.Any<CancellationToken>())).Do(_ => _messageClientIsConnected = true);
    _messageClient.When(x => x.Disconnect(Arg.Any<CancellationToken>())).Do(_ => _messageClientIsConnected = true);
  }

  [Fact]
  public async Task PublicGetTime_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/get_time\"}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": 1550147385946,\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(responseJson)
    );

    // Act
    await _sut.Connect();
    var result = await _sut.Supporting.PublicGetTime();
    await _sut.Disconnect();

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(requestJson, Arg.Any<CancellationToken>());
        _messageClient.Disconnect(Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(4);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicHello_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/hello\",\"params\":{\"client_name\":\"UnitTestClient\",\"client_version\":\"1.2.3.4\"}}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(responseJson)
    );

    // Act
    await _sut.Connect();

    var result = await _sut.Supporting.PublicHello(
                   new PublicHelloRequest
                   {
                     ClientName = "UnitTestClient",
                     ClientVersion = "1.2.3.4"
                   }
                 );

    await _sut.Disconnect();

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(requestJson, Arg.Any<CancellationToken>());
        _messageClient.Disconnect(Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(4);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicStatus_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/status\"}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"locked\": true, \"locked_currencies\":[\"BTC\",\"ETH\"]},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(responseJson)
    );

    // Act
    await _sut.Connect();

    var result = await _sut.Supporting.PublicStatus();

    await _sut.Disconnect();

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(requestJson, Arg.Any<CancellationToken>());
        _messageClient.Disconnect(Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(4);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicTest_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/test\"}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(responseJson)
    );

    // Act
    await _sut.Connect();
    var result = await _sut.Supporting.PublicTest();
    await _sut.Disconnect();

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(requestJson, Arg.Any<CancellationToken>());
        _messageClient.Disconnect(Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(4);
    await Verify(result, VerifySettings);
  }
}
