namespace DeriSock.Request
{
  using Newtonsoft.Json;

  public class UserChangesInstrumentSubscriptionParams : ISubscriptionChannel
  {
    /// <summary>
    ///   Instrument name
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   <para>
    ///     Frequency of notifications. Events will be aggregated over this interval. The value <c>raw</c> means no
    ///     aggregation will be applied
    ///   </para>
    ///   <para>Enum: <c>100ms</c>, <c>raw</c></para>
    /// </summary>
    [JsonProperty("interval")]
    public string Interval { get; set; }

    public string ToChannelName()
    {
      return $"user.changes.{InstrumentName}.{Interval}";
    }
  }
}
