namespace DeriSock.Tests.Unit;

using System.Net.Sockets;

using DeriSock.Constants;
using DeriSock.Model;
using DeriSock.Net;
using DeriSock.Utils;

using FluentAssertions;

// ReSharper disable once InconsistentNaming
public sealed class DeribitClient_ConnectionHandling
{
  private static async IAsyncEnumerable<string> GetSingleResponseAsyncEnumerable(Func<Task<string>> responseFunc)
  {
    yield return await responseFunc.Invoke();
  }

  private static async IAsyncEnumerable<string> GetMultiResponseAsyncEnumerable(Func<Task<(bool, Func<string>)>> responseFunc)
  {
    var more = true;
    while (more)
    {
      (more, var result) = await responseFunc.Invoke();
      yield return result.Invoke();
    }
  }

  [Fact]
  public async Task ConnectionLoss_LeadsTo_ReConnect()
  {
    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var abortionCount = 0;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });

    mockTextMessageClient.Setup(
      l => l.GetMessageStream(It.IsAny<CancellationToken>())
    ).Returns(
      () => GetSingleResponseAsyncEnumerable(
        async () =>
        {
          await Task.Delay(10);

          if (++abortionCount < 2)
            throw new SocketException((int)SocketError.ConnectionAborted);

          return string.Empty;
        }
      )
    );

    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();
    await Task.Delay(20);

    // Assert
    mockTextMessageClient.Verify(l => l.Connect(Endpoint.TestNet, It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.GetMessageStream(It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task ConnectionLossDuringRequest_LeadsTo_ThrowOnRequestAndReConnect()
  {
    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var requestReceived = false;
    var abortionCount = 0;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });

    mockTextMessageClient.Setup(l => l.Send(It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, CancellationToken>(
      (_, _) => { requestReceived = true; }
    );

    mockTextMessageClient.Setup(
      l => l.GetMessageStream(It.IsAny<CancellationToken>())
    ).Returns(
      () => GetSingleResponseAsyncEnumerable(
        async () =>
        {
          while (!requestReceived)
            await Task.Delay(1);

          if (++abortionCount < 2)
            throw new SocketException((int)SocketError.ConnectionAborted);

          return string.Empty;
        }
      )
    );

    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();
    var request = () => client.Supporting.PublicTest();
    await request.Should().ThrowAsync<TaskCanceledException>();
    await Task.Delay(10);

    // Assert
    mockTextMessageClient.Verify(l => l.Connect(Endpoint.TestNet, It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.GetMessageStream(It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.Send(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    mockTextMessageClient.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task ConnectionLossDuringSend_LeadsTo_ThrowOnSend()
  {
    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });

    mockTextMessageClient.Setup(l => l.Send(It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, CancellationToken>(
      (_, _) => throw new SocketException((int)SocketError.ConnectionAborted)
    );

    mockTextMessageClient.Setup(
      l => l.GetMessageStream(It.IsAny<CancellationToken>())
    ).Returns(
      () => GetSingleResponseAsyncEnumerable(
        async () =>
        {
          await Task.Delay(1);
          return string.Empty;
        }
      )
    );

    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();
    var request = async () => await client.Supporting.PublicTest();
    await request.Should().ThrowAsync<SocketException>();

    // Assert
    mockTextMessageClient.Verify(l => l.Connect(Endpoint.TestNet, It.IsAny<CancellationToken>()), Times.Once);
    mockTextMessageClient.Verify(l => l.GetMessageStream(It.IsAny<CancellationToken>()), Times.Once);
    mockTextMessageClient.Verify(l => l.Send(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Once);
    mockTextMessageClient.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task ConnectionResetDuringSend_LeadsTo_ReConnectAndReSend()
  {
    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var resetCount = 0;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });

    mockTextMessageClient.Setup(l => l.Send(It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, CancellationToken>(
      (_, _) =>
      {
        if (++resetCount < 2)
          throw new SocketException((int)SocketError.ConnectionReset);
      }
    );

    mockTextMessageClient.Setup(
      l => l.GetMessageStream(It.IsAny<CancellationToken>())
    ).Returns(
      () => GetSingleResponseAsyncEnumerable(
        async () =>
        {
          await Task.Delay(1);
          return "{\"jsonrpc\": \"2.0\",\"id\": 2,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";
        }
      )
    );

    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();
    await client.Supporting.PublicTest();

    // Assert
    mockTextMessageClient.Verify(l => l.Connect(Endpoint.TestNet, It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.GetMessageStream(It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.Send(It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.IsConnected, Times.Once);
    mockTextMessageClient.VerifyNoOtherCalls();
  }

  [Fact]
  public async Task ReConnect_LeadsTo_ReSubscribingOfActiveNotificationChannels()
  {
    const string requestJson1 = "{\"jsonrpc\":\"2.0\",\"id\":1,\"method\":\"public/subscribe\",\"params\":{\"channels\":[\"ticker.my.test.instrument.number.0.100ms\",\"ticker.my.test.instrument.number.1.100ms\",\"ticker.my.test.instrument.number.2.100ms\",\"ticker.my.test.instrument.number.3.100ms\",\"ticker.my.test.instrument.number.4.100ms\",\"ticker.my.test.instrument.number.5.100ms\",\"ticker.my.test.instrument.number.6.100ms\",\"ticker.my.test.instrument.number.7.100ms\",\"ticker.my.test.instrument.number.8.100ms\",\"ticker.my.test.instrument.number.9.100ms\"]}}";
    const string requestJson2 = "{\"jsonrpc\":\"2.0\",\"id\":2,\"method\":\"public/test\"}";
    const string requestJson3 = "{\"jsonrpc\":\"2.0\",\"id\":3,\"method\":\"public/subscribe\",\"params\":{\"channels\":[\"ticker.my.test.instrument.number.0.100ms\",\"ticker.my.test.instrument.number.1.100ms\",\"ticker.my.test.instrument.number.2.100ms\",\"ticker.my.test.instrument.number.3.100ms\",\"ticker.my.test.instrument.number.4.100ms\",\"ticker.my.test.instrument.number.5.100ms\",\"ticker.my.test.instrument.number.6.100ms\",\"ticker.my.test.instrument.number.7.100ms\",\"ticker.my.test.instrument.number.8.100ms\",\"ticker.my.test.instrument.number.9.100ms\"]}}";

    const string responseJson1 = "{\"jsonrpc\": \"2.0\",\"id\": 1,\"testnet\": true,\"result\":[\"ticker.my.test.instrument.number.0.100ms\",\"ticker.my.test.instrument.number.1.100ms\",\"ticker.my.test.instrument.number.2.100ms\",\"ticker.my.test.instrument.number.3.100ms\",\"ticker.my.test.instrument.number.4.100ms\",\"ticker.my.test.instrument.number.5.100ms\",\"ticker.my.test.instrument.number.6.100ms\",\"ticker.my.test.instrument.number.7.100ms\",\"ticker.my.test.instrument.number.8.100ms\",\"ticker.my.test.instrument.number.9.100ms\"],\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";
    const string responseJson3 = "{\"jsonrpc\": \"2.0\",\"id\": 3,\"testnet\": true,\"result\":[\"ticker.my.test.instrument.number.0.100ms\",\"ticker.my.test.instrument.number.1.100ms\",\"ticker.my.test.instrument.number.2.100ms\",\"ticker.my.test.instrument.number.3.100ms\",\"ticker.my.test.instrument.number.4.100ms\",\"ticker.my.test.instrument.number.5.100ms\",\"ticker.my.test.instrument.number.6.100ms\",\"ticker.my.test.instrument.number.7.100ms\",\"ticker.my.test.instrument.number.8.100ms\",\"ticker.my.test.instrument.number.9.100ms\"],\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}";

    // Arrange
    RequestIdGenerator.Reset();
    var isConnected = false;
    var requestCount = 0;
    var responseCount = 0;
    var mockTextMessageClient = new Mock<ITextMessageClient>();
    mockTextMessageClient.Setup(l => l.IsConnected).Returns(() => isConnected);
    mockTextMessageClient.Setup(l => l.Connect(It.IsAny<Uri>(), It.IsAny<CancellationToken>())).Callback(() => { isConnected = true; });
    mockTextMessageClient.Setup(l => l.Disconnect(It.IsAny<CancellationToken>())).Callback(() => { isConnected = false; });

    mockTextMessageClient.Setup(l => l.Send(It.IsAny<string>(), It.IsAny<CancellationToken>())).Callback<string, CancellationToken>(
      (_, _) => { ++requestCount; }
    );

    mockTextMessageClient.Setup(
      l => l.GetMessageStream(It.IsAny<CancellationToken>())
    ).Returns(
      () => GetMultiResponseAsyncEnumerable(
        async () =>
        {
          ++responseCount;

          while (requestCount != responseCount)
            await Task.Delay(1);

          return responseCount switch
          {
            1 => (true, () => responseJson1),
            2 => (true, () => throw new SocketException((int)SocketError.ConnectionAborted)),
            3 => (false, () => responseJson3),
            _ => throw new ArgumentOutOfRangeException()
          };
        }
      )
    );

    var client = new DeribitClient(EndpointType.Testnet, mockTextMessageClient.Object);

    // Act
    await client.Connect();

    await client.Subscriptions.SubscribeTicker(
      new TickerChannel { InstrumentName = "my.test.instrument.number.0", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.1", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.2", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.3", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.4", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.5", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.6", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.7", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.8", Interval = NotificationInterval2._100ms },
      new TickerChannel { InstrumentName = "my.test.instrument.number.9", Interval = NotificationInterval2._100ms }
    );

    var forceConnectionLoss = () => client.Supporting.PublicTest();
    await forceConnectionLoss.Should().ThrowAsync<TaskCanceledException>();
    await Task.Delay(10);

    // Assert
    mockTextMessageClient.Verify(l => l.Connect(Endpoint.TestNet, It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.GetMessageStream(It.IsAny<CancellationToken>()), Times.Exactly(2));
    mockTextMessageClient.Verify(l => l.Send(requestJson1, It.IsAny<CancellationToken>()), Times.Once);
    mockTextMessageClient.Verify(l => l.Send(requestJson2, It.IsAny<CancellationToken>()), Times.Once);
    mockTextMessageClient.Verify(l => l.Send(requestJson3, It.IsAny<CancellationToken>()), Times.Once);
    mockTextMessageClient.VerifyNoOtherCalls();
  }
}
