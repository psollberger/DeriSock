﻿namespace DeriSock.Request;

using Newtonsoft.Json;

public class DeribitPriceIndexSubscriptionParams : ISubscriptionChannel
{
  /// <summary>
  ///   <para>Index identifier, matches (base) crypto currency with quote currency</para>
  ///   <para>Enum: <c>btcdvol_usdc</c>, <c>ethdvol_usdc</c></para>
  /// </summary>
  [JsonProperty("index_name")]
  public string IndexName { get; set; }

  public string ToChannelName()
  {
    return $"deribit_price_index.{IndexName}";
  }
}
