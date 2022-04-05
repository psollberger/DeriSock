namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class BookGroupNotification
{
  /// <summary>
  ///   List of asks
  /// </summary>
  [JsonProperty("asks")]
  public decimal[,] Asks { get; set; }

  /// <summary>
  ///   List of bids
  /// </summary>
  [JsonProperty("bids")]
  public decimal[,] Bids { get; set; }

  /// <summary>
  ///   id of the notification
  /// </summary>
  [JsonProperty("change_id")]
  public long ChangeId { get; set; }

  /// <summary>
  ///   Unique instrument identifier
  /// </summary>
  [JsonProperty("instrument_name")]
  public string InstrumentName { get; set; }

  /// <summary>
  ///   The timestamp of last change (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("timestamp")]
  public long Timestamp { get; set; }

  /// <inheritdoc cref="Timestamp" />
  [JsonIgnore]
  public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();
}
