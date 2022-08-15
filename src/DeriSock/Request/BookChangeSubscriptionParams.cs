namespace DeriSock.Request;

public class BookChangeSubscriptionParams : ISubscriptionChannel
{
  /// <summary>
  ///   Instrument name
  /// </summary>
  public string InstrumentName { get; set; }

  /// <summary>
  ///   <para>Frequency of notifications. Events will be aggregated over this interval.</para>
  ///   <para>Enum: <c>raw</c>, <c>100ms</c></para>
  /// </summary>
  public string Interval { get; set; }

  public string ToChannelName()
  {
    return $"book.{InstrumentName}.{Interval}";
  }
}
