namespace DeriSock.Request
{
  using Newtonsoft.Json;

  public class ChartTradesSubscriptionParams : ISubscriptionChannel
  {
    /// <summary>
    ///   Instrument name
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   <para>Chart bars resolution given in full minutes or keyword <c>1D</c> (only some specific resolutions are supported)</para>
    ///   <para>
    ///     Enum: <c>1</c>, <c>3</c>, <c>5</c>, <c>10</c>, <c>15</c>, <c>30</c>, <c>60</c>, <c>120</c>, <c>180</c>,
    ///     <c>360</c>, <c>720</c>, <c>1D</c>
    ///   </para>
    /// </summary>
    [JsonProperty("resolution")]
    public string Resolution { get; set; }

    public string ToChannelName()
    {
      return $"chart.trades.{InstrumentName}.{Resolution}";
    }
  }
}
