namespace DeriSock.Request;

using Newtonsoft.Json;

public class EstimatedExpirationPriceSubscriptionParams : ISubscriptionChannel
{
  /// <summary>
  ///   <para>Index identifier, matches (base) crypto currency with quote currency</para>
  ///   <para>Enum: <c>btc_usd</c>, <c>eth_usd</c></para>
  /// </summary>
  [JsonProperty("index_name")]
  public string IndexName { get; set; }

  public string ToChannelName()
  {
    return $"estimated_expiration_price.{IndexName}";
  }
}
