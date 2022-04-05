namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class DepositAddress
{
  /// <summary>
  ///   Address in proper format for currency
  /// </summary>
  [JsonProperty("address")]
  public string Address { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("creation_timestamp")]
  public long CreationTimestamp { get; set; }

  /// <inheritdoc cref="CreationTimestamp" />
  [JsonIgnore]
  public DateTime CreationDateTime => CreationTimestamp.AsDateTimeFromMilliseconds();

  /// <summary>
  ///   Currency, i.e "BTC", "ETH"
  /// </summary>
  [JsonProperty("currency")]
  public string Currency { get; set; }

  /// <summary>
  ///   Address type/purpose, allowed values : deposit, withdrawal, transfer
  /// </summary>
  [JsonProperty("type")]
  public string Type { get; set; }
}
