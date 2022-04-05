namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class Announcement
{
  /// <summary>
  ///   The HTML body of the announcement
  /// </summary>
  [JsonProperty("body")]
  public string Body { get; set; }

  /// <summary>
  ///   Whether the user confirmation is required for this announcement
  /// </summary>
  [JsonProperty("confirmation")]
  public bool Confirmation { get; set; }

  /// <summary>
  ///   A unique identifier for the announcement
  /// </summary>
  [JsonProperty("id")]
  public long Id { get; set; }

  /// <summary>
  ///   Whether the announcement is marked as important
  /// </summary>
  [JsonProperty("important")]
  public bool Important { get; set; }

  /// <summary>
  ///   The timestamp in ms at which the announcement was published
  /// </summary>
  [JsonProperty("publication_timestamp")]
  public long PublicationTimestamp { get; set; }

  /// <inheritdoc cref="PublicationTimestamp" />
  [JsonIgnore]
  public DateTime PublicationDateTime => PublicationTimestamp.AsDateTimeFromMilliseconds();

  /// <summary>
  ///   The title of the announcement
  /// </summary>
  [JsonProperty("title")]
  public string Title { get; set; }
}
