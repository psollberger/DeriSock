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

  public DeribitClient_Supporting()
  {
    VerifySettings.UseDirectory("VerifyData");
  }

  private static async IAsyncEnumerable<string> GetResponseAsyncEnumerable(string responseJson)
  {
    await Task.Delay(10);
    yield return responseJson;
  }

  private static void VerifyTextMessageClientMockDefaults(Mock<ITextMessageClient> mock)
  {
    mock.Verify(l => l.Connect(Endpoint.TestNet, It.IsAny<CancellationToken>()), Times.Once);
    mock.Verify(l => l.Disconnect(It.IsAny<CancellationToken>()), Times.Once);
    mock.Verify(l => l.GetMessageStream(It.IsAny<CancellationToken>()), Times.Once);
    mock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task PublicGetTime_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/get_time\"}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": 1550147385946,\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });
    mockTextMessageClient.Setup(l => l.GetMessageStream(It.IsAny<CancellationToken>())).Returns(() => GetResponseAsyncEnumerable(responseJson));
    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();
    var result = await client.Supporting.PublicGetTime();
    await client.Disconnect();

    // Assert
    mockTextMessageClient.Verify(l => l.Send(requestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyTextMessageClientMockDefaults(mockTextMessageClient);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicHello_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/hello\",\"params\":{\"client_name\":\"UnitTestClient\",\"client_version\":\"1.2.3.4\"}}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });
    mockTextMessageClient.Setup(l => l.GetMessageStream(It.IsAny<CancellationToken>())).Returns(() => GetResponseAsyncEnumerable(responseJson));
    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();

    var result = await client.Supporting.PublicHello(
                   new PublicHelloRequest
                   {
                     ClientName = "UnitTestClient",
                     ClientVersion = "1.2.3.4"
                   }
                 );

    await client.Disconnect();

    // Assert
    mockTextMessageClient.Verify(l => l.Send(requestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyTextMessageClientMockDefaults(mockTextMessageClient);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicStatus_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/status\"}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"locked\": true, \"locked_currencies\":[\"BTC\",\"ETH\"]},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });
    mockTextMessageClient.Setup(l => l.GetMessageStream(It.IsAny<CancellationToken>())).Returns(() => GetResponseAsyncEnumerable(responseJson));
    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();

    var result = await client.Supporting.PublicStatus();

    await client.Disconnect();

    // Assert
    mockTextMessageClient.Verify(l => l.Send(requestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyTextMessageClientMockDefaults(mockTextMessageClient);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicTest_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string requestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/test\"}";
    const string responseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });
    mockTextMessageClient.Setup(l => l.GetMessageStream(It.IsAny<CancellationToken>())).Returns(() => GetResponseAsyncEnumerable(responseJson));
    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();
    var result = await client.Supporting.PublicTest();
    await client.Disconnect();

    // Assert
    mockTextMessageClient.Verify(l => l.Send(requestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyTextMessageClientMockDefaults(mockTextMessageClient);
    await Verify(result, VerifySettings);
  }
}
