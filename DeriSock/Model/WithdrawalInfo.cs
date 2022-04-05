namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class WithdrawalInfo
{
  /// <summary>
  ///   Address in proper format for currency
  /// </summary>
  [JsonProperty("address")]
  public string Address { get; set; }

  /// <summary>
  ///   Amount of funds in given currency
  /// </summary>
  [JsonProperty("amount")]
  public decimal Amount { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch) of withdrawal confirmation, null when not confirmed
  /// </summary>
  [JsonProperty("confirmed_timestamp")]
  public long ConfirmedTimestamp { get; set; }

  /// <inheritdoc cref="ConfirmedTimestamp" />
  [JsonIgnore]
  public DateTime ConfirmedDateTime => ConfirmedTimestamp.AsDateTimeFromMilliseconds();

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
  ///   Fee in currency
  /// </summary>
  [JsonProperty("fee")]
  public decimal Fee { get; set; }

  /// <summary>
  ///   Withdrawal id in Deribit system
  /// </summary>
  [JsonProperty("id")]
  public long Id { get; set; }

  /// <summary>
  ///   Id of priority level
  /// </summary>
  [JsonProperty("priority")]
  public double Priority { get; set; }

  /// <summary>
  ///   Withdrawal state, allowed values : unconfirmed, confirmed, cancelled, completed, interrupted, rejected
  /// </summary>
  [JsonProperty("state")]
  public string State { get; set; }

  /// <summary>
  ///   Transaction id in proper format for currency, null if id is not available
  /// </summary>
  [JsonProperty("transaction_id")]
  public string TransactionId { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("updated_timestamp")]
  public long UpdatedTimestamp { get; set; }

  /// <inheritdoc cref="UpdatedTimestamp" />
  [JsonIgnore]
  public DateTime UpdatedDateTime => UpdatedTimestamp.AsDateTimeFromMilliseconds();
}
