namespace DeriSock.Tests.Unit;

using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;

using DeriSock.Constants;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;
using DeriSock.Utils;

[UsesVerify]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class DeribitClient_Supporting
{
  private static readonly VerifySettings VerifySettings = new();

  public DeribitClient_Supporting()
  {
    VerifySettings.UseDirectory("VerifyData");
  }

  private static Mock<IJsonRpcMessageSource> CreateDefaultMessageSourceMock(string methodResponseJson)
  {
    var mock = new Mock<IJsonRpcMessageSource>();
    mock.Setup(l => l.State).Returns(WebSocketState.Open);
    mock.Setup(l => l.GetMessageStream(It.IsAny<CancellationToken>())).Returns(() => GetResponseAsyncEnumerable(methodResponseJson));
    return mock;
  }

  private static async IAsyncEnumerable<string> GetResponseAsyncEnumerable(string responseJson)
  {
    await Task.Delay(10);
    yield return responseJson;
  }

  private static void VerifyMessageSourceMockDefaults(Mock<IJsonRpcMessageSource> mock)
  {
    mock.Verify(l => l.Connect(Endpoint.TestNet, It.IsAny<CancellationToken>()), Times.Once);
    mock.Verify(l => l.Disconnect(null, null, It.IsAny<CancellationToken>()), Times.Once);
    mock.Verify(l => l.Exception, Times.Once);
    mock.Verify(l => l.GetMessageStream(It.IsAny<CancellationToken>()), Times.Once);
    mock.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task PublicGetTime_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string RequestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/get_time\"}";
    const string ResponseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": 1550147385946,\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var mockMessageSource = CreateDefaultMessageSourceMock(ResponseJson);
    var client = new DeribitClient(EndpointType.Testnet, mockMessageSource.Object);

    // Act
    await client.Connect();
    var result = await client.Supporting.PublicGetTime();
    await client.Disconnect();

    // Assert
    mockMessageSource.Verify(l => l.Send(RequestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyMessageSourceMockDefaults(mockMessageSource);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicHello_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string RequestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/hello\",\"params\":{\"client_name\":\"UnitTestClient\",\"client_version\":\"1.2.3.4\"}}";
    const string ResponseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var mockMessageSource = CreateDefaultMessageSourceMock(ResponseJson);
    var client = new DeribitClient(EndpointType.Testnet, mockMessageSource.Object);

    // Act
    await client.Connect();
    var result = await client.Supporting.PublicHello(new PublicHelloRequest
    {
      ClientName = "UnitTestClient",
      ClientVersion = "1.2.3.4"
    });
    await client.Disconnect();

    // Assert
    mockMessageSource.Verify(l => l.Send(RequestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyMessageSourceMockDefaults(mockMessageSource);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicStatus_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string RequestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/status\"}";
    const string ResponseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"locked\": true, \"locked_currencies\":[\"BTC\",\"ETH\"]},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var mockMessageSource = CreateDefaultMessageSourceMock(ResponseJson);
    var client = new DeribitClient(EndpointType.Testnet, mockMessageSource.Object);

    // Act
    await client.Connect();

    var result = await client.Supporting.PublicStatus();

    await client.Disconnect();

    // Assert
    mockMessageSource.Verify(l => l.Send(RequestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyMessageSourceMockDefaults(mockMessageSource);
    await Verify(result, VerifySettings);
  }

  [Fact]
  public async Task PublicTest_WithCorrectResult_CallsCorrectMethodsAndParsesCorrectly()
  {
    const string RequestJson = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/test\"}";
    const string ResponseJson = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var mockMessageSource = CreateDefaultMessageSourceMock(ResponseJson);
    var client = new DeribitClient(EndpointType.Testnet, mockMessageSource.Object);

    // Act
    await client.Connect();
    var result = await client.Supporting.PublicTest();
    await client.Disconnect();

    // Assert
    mockMessageSource.Verify(l => l.Send(RequestJson, It.IsAny<CancellationToken>()), Times.Once);
    VerifyMessageSourceMockDefaults(mockMessageSource);
    await Verify(result, VerifySettings);
  }
}
