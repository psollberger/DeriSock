namespace DeriSock.Tests.Integration;

using DeriSock.Model;

public class Temporary
{
  private readonly DeribitConfiguration _config;
  private readonly DeribitClient _client;

  public Temporary()
  {
    _config = AppConfig.GetDeribitConfiguration();
    _client = new DeribitClient(EndpointType.Testnet);
  }

  [Fact]
  public async Task PlayAndFun()
  {
    await _client.Connect();

    var instr = await _client.Public.GetInstruments(new PublicGetInstrumentsRequest
    {
      Currency = CurrencySymbol.BTC,
      Expired = false
    });

    await _client.Disconnect();

    instr.ResultData.Should().NotBeEmpty(because: "it's impossible that there are no active instruments");
  }
}
