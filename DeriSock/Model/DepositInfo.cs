namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class DepositInfo
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
    ///   Currency, i.e "BTC", "ETH"
    /// </summary>
    [JsonProperty("currency")]
    public string Currency { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("received_timestamp")]
    public long ReceivedTimestamp { get; set; }

    /// <inheritdoc cref="ReceivedTimestamp" />
    [JsonIgnore]
    public DateTime ReceiveDateTime => ReceivedTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Deposit state, allowed values : pending, completed, rejected, replaced
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
}
