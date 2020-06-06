namespace DeriSock.Request
{
  using Newtonsoft.Json;

  public class BookGroupSubscriptionParams : ISubscriptionChannel
  {
    /// <summary>
    ///   Instrument name
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   <para>
    ///     Group prices (by rounding). Use <c>none</c> for no grouping. For ETH crypto currency, real group is divided by
    ///     100.0, e.g. given value 5 means using 0.05
    ///   </para>
    ///   <para>Allowed values for BTC - <c>none</c>, <c>1</c>, <c>2</c>, <c>5</c>, <c>10</c></para>
    ///   <para>Allowed values for ETH - <c>none</c>, <c>5</c>, <c>10</c>, <c>25</c>, <c>100</c>, <c>250</c></para>
    /// </summary>
    [JsonProperty("group")]
    public string Group { get; set; } = "none";

    /// <summary>
    ///   <para>Number of price levels to be included</para>
    ///   <para>Enum: <c>1</c>, <c>10</c>, <c>20</c></para>
    /// </summary>
    [JsonProperty("depth")]
    public int Depth { get; set; } = 1;

    /// <summary>
    ///   <para>Frequency of notifications. Events will be aggregated over this interval</para>
    ///   <para>Enum: <c>100ms</c></para>
    /// </summary>
    [JsonProperty("interval")]
    public string Interval { get; set; } = "100ms";

    public string ToChannelName()
    {
      return $"book.{InstrumentName}.{Group}.{Depth}.{Interval}";
    }
  }
}
