namespace DeriSock.Request;

using Newtonsoft.Json;

public class QuoteSubscriptionParams : ISubscriptionChannel
{
  /// <summary>
  ///   Instrument name
  /// </summary>
  [JsonProperty("instrument_name")]
  public string InstrumentName { get; set; }

  public string ToChannelName()
  {
    return $"quote.{InstrumentName}";
  }
}
