namespace DeriSock.Tests.Unit;

using System.Net.Sockets;

using DeriSock.Constants;
using DeriSock.Model;
using DeriSock.Net;
using DeriSock.Utils;

// ReSharper disable once InconsistentNaming
public sealed class DeribitClient_ConnectionHandling
{
  private bool _messageClientIsConnected;
  private readonly ITextMessageClient _messageClient = Substitute.For<ITextMessageClient>();
  private readonly DeribitClient _sut;

  public DeribitClient_ConnectionHandling()
  {
    _sut = new DeribitClient(EndpointType.Testnet, _messageClient);

    _messageClient.IsConnected.Returns(_ => _messageClientIsConnected);
    _messageClient.When(x => x.Connect(Arg.Any<Uri>(), Arg.Any<CancellationToken>())).Do(_ => _messageClientIsConnected = true);
    _messageClient.When(x => x.Disconnect(Arg.Any<CancellationToken>())).Do(_ => _messageClientIsConnected = true);
  }

  [Fact]
  public async Task ConnectionLoss_LeadsTo_ReConnect()
  {
    // Arrange
    RequestIdGenerator.Reset();

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable((Func<Task<string>>)(() => throw new SocketException((int)SocketError.ConnectionAborted))),
      Helpers.GetStringAsyncEnumerable(string.Empty)
    );

    await _sut.Connect();
    await Task.Delay(_sut.ReConnectDelay);
    await Task.Delay(20);

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _ = _messageClient.IsConnected;
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(5);
  }

  [Fact]
  public async Task ConnectionLossDuringRequest_LeadsTo_ThrowOnRequestAndReConnect()
  {
    // Arrange
    RequestIdGenerator.Reset();

    var requestReceived = false;
    _messageClient.When(x => x.Send(Arg.Any<string>(), Arg.Any<CancellationToken>())).Do(_ => requestReceived = true);

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(
        (Func<Task<string>>)(async () =>
                              {
                                while (!requestReceived)
                                  await Task.Delay(1);

                                throw new SocketException((int)SocketError.ConnectionAborted);
                              })
      ),
      Helpers.GetStringAsyncEnumerable(string.Empty)
    );

    // Act
    await _sut.Connect();
    var request = () => _sut.Supporting.PublicTest();
    await request.Should().ThrowAsync<TaskCanceledException>();
    await Task.Delay(_sut.ReConnectDelay);
    await Task.Delay(10);

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _ = _messageClient.IsConnected;
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(6);
  }

  [Fact]
  public async Task ConnectionLossDuringSend_LeadsTo_ThrowOnSend()
  {
    // Arrange
    RequestIdGenerator.Reset();

    _messageClient.Send(Arg.Any<string>(), Arg.Any<CancellationToken>())
      .Returns(
        _ => throw new SocketException((int)SocketError.ConnectionAborted)
      );

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(string.Empty)
    );

    // Act
    await _sut.Connect();
    var request = async () => await _sut.Supporting.PublicTest();
    await request.Should().ThrowAsync<SocketException>();

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(Arg.Any<string>(), Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(3);
  }

  [Fact]
  public async Task ConnectionResetDuringSend_LeadsTo_ReConnectAndReSend()
  {
    // Arrange
    RequestIdGenerator.Reset();

    _messageClient.When(x => x.Send(Arg.Any<string>(), Arg.Any<CancellationToken>()))
      .Do(
        Callback
          .First(_ => throw new SocketException((int)SocketError.ConnectionReset))
          .ThenKeepDoing(_ => { })
      );

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(string.Empty),
      Helpers.GetStringAsyncEnumerable("{\"jsonrpc\": \"2.0\",\"id\": 2,\"testnet\": true,\"result\": {\"version\": \"1.2.26\"},\"usIn\": 1535043730126248,\"usOut\": 1535043730126250,\"usDiff\": 2}")
    );

    // Act
    await _sut.Connect();
    await _sut.Supporting.PublicTest();

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(Arg.Any<string>(), Arg.Any<CancellationToken>());
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _ = _messageClient.IsConnected;
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(Arg.Any<string>(), Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(7);
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

    var requestCount = 0;
    var responseCount = 0;

    _messageClient.When(x => x.Send(Arg.Any<string>(), Arg.Any<CancellationToken>())).Do(_ => ++requestCount);

    _messageClient.GetMessageStream(Arg.Any<CancellationToken>()).Returns(
      Helpers.GetStringAsyncEnumerable(
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

    // Act
    await _sut.Connect();

    await _sut.Subscriptions.SubscribeTicker(
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

    var forceConnectionLoss = () => _sut.Supporting.PublicTest();
    await forceConnectionLoss.Should().ThrowAsync<TaskCanceledException>();
    await Task.Delay(_sut.ReConnectDelay);
    await Task.Delay(10);

    // Assert
    Received.InOrder(
      () =>
      {
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(requestJson1, Arg.Any<CancellationToken>());
        _messageClient.Send(requestJson2, Arg.Any<CancellationToken>());
        _messageClient.Connect(Endpoint.TestNet, Arg.Any<CancellationToken>());
        _ = _messageClient.IsConnected;
        _messageClient.GetMessageStream(Arg.Any<CancellationToken>());
        _messageClient.Send(requestJson3, Arg.Any<CancellationToken>());
      }
    );

    _messageClient.ReceivedCalls().Should().HaveCount(8);
  }
}
