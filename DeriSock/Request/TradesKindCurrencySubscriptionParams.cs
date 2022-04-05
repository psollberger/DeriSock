namespace DeriSock.Request;

using Newtonsoft.Json;

public class TradesKindCurrencySubscriptionParams : ISubscriptionChannel
{
  /// <summary>
  ///   <para>Instrument kind</para>
  ///   <para>Enum: <c>future</c>, <c>option</c></para>
  /// </summary>
  [JsonProperty("kind")]
  public string Kind { get; set; }

  /// <summary>
  ///   <para>The currency symbol or <c>any</c> for all</para>
  ///   <para>Enum: <c>BTC</c>, <c>ETH</c>, <c>any</c></para>
  /// </summary>
  [JsonProperty("currency")]
  public string Currency { get; set; }

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
    return $"trades.{Kind}.{Currency}.{Interval}";
  }
}
