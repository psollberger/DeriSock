namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class TransferInfo
{
  /// <summary>
  ///   Amount of funds in given currency
  /// </summary>
  [JsonProperty("amount")]
  public decimal Amount { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("created_timestamp")]
  public long CreatedTimestamp { get; set; }

  /// <inheritdoc cref="CreatedTimestamp" />
  [JsonIgnore]
  public DateTime CreatedDateTime => CreatedTimestamp.AsDateTimeFromMilliseconds();

  /// <summary>
  ///   Currency, i.e "BTC", "ETH"
  /// </summary>
  [JsonProperty("currency")]
  public string Currency { get; set; }

  /// <summary>
  ///   Transfer direction
  /// </summary>
  [JsonProperty("direction")]
  public string Direction { get; set; }

  /// <summary>
  ///   Id of transfer
  /// </summary>
  [JsonProperty("id")]
  public long Id { get; set; }

  /// <summary>
  ///   For transfer from/to subaccount returns this subaccount name, for transfer to other account returns address, for
  ///   transfer from other account returns that accounts username.
  /// </summary>
  [JsonProperty("other_side")]
  public string OtherSide { get; set; }

  /// <summary>
  ///   Transfer state, allowed values : prepared, confirmed, cancelled, waiting_for_admin, rejection_reason
  /// </summary>
  [JsonProperty("state")]
  public string State { get; set; }

  /// <summary>
  ///   Type of transfer: user - sent to user, subaccount - sent to subaccount
  /// </summary>
  [JsonProperty("type")]
  public string Type { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("updated_timestamp")]
  public long UpdatedTimestamp { get; set; }

  /// <inheritdoc cref="UpdatedTimestamp" />
  [JsonIgnore]
  public DateTime UpdatedDateTime => UpdatedTimestamp.AsDateTimeFromMilliseconds();
}
