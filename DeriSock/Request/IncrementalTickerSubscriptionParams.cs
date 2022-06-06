namespace DeriSock.Request;

using Newtonsoft.Json;

/// <summary>
/// Notifies about changes in instrument ticker (key information about the instrument).
/// The first notification will contain the whole ticker.
/// After that there will only information about changes in the ticker.
/// This event is send at most once per second.
/// </summary>
public class IncrementalTickerSubscriptionParams : ISubscriptionChannel
{
  /// <summary>
  ///   Instrument name
  /// </summary>
  [JsonProperty("instrument_name")]
  public string InstrumentName { get; set; }

  public string ToChannelName()
  {
    return $"incremental_ticker.{InstrumentName}";
  }
}
