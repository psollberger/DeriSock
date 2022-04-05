namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class BlockTrade
{
  /// <summary>
  ///   Currency, i.e "BTC", "ETH"
  /// </summary>
  [JsonProperty("currency")]
  public string Currency { get; set; }

  /// <summary>
  ///   Id of user that executed block_trade
  /// </summary>
  [JsonProperty("executor_user_id")]
  public long ExecutorUserId { get; set; }

  /// <summary>
  ///   Block trade id
  /// </summary>
  [JsonProperty("id")]
  public string Id { get; set; }

  /// <summary>
  ///   Id of user that initiated block_trade
  /// </summary>
  [JsonProperty("initiator_user_id")]
  public long InitiatorUserId { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("timestamp")]
  public long Timestamp { get; set; }

  /// <inheritdoc cref="Timestamp" />
  [JsonIgnore]
  public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();

  /// <summary>
  ///   of object
  /// </summary>
  [JsonProperty("trades")]
  public UserTrade[] Trades { get; set; }
}
