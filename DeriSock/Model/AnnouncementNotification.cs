namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;

  public class AnnouncementNotification
  {
    /// <summary>
    ///   Action taken by the platform administrators. Published a new announcement, or deleted the old one
    /// </summary>
    [JsonProperty("action")]
    public string Action { get; set; }

    /// <summary>
    ///   HTML-formatted announcement body
    /// </summary>
    [JsonProperty("body")]
    public string Body { get; set; }

    /// <summary>
    ///   Whether the user confirmation is required for this announcement
    /// </summary>
    [JsonProperty("confirmation")]
    public bool Confirmation { get; set; }

    /// <summary>
    ///   Announcement's identifier
    /// </summary>
    [JsonProperty("id")]
    public long Id { get; set; }

    /// <summary>
    ///   Whether the announcement is marked as important
    /// </summary>
    [JsonProperty("important")]
    public bool Important { get; set; }

    /// <summary>
    ///   The timestamp (milliseconds since the Unix epoch) of announcement publication
    /// </summary>
    [JsonProperty("publication_timestamp")]
    public long PublicationTimestamp { get; set; }

    /// <inheritdoc cref="PublicationTimestamp" />
    [JsonIgnore]
    public DateTime PublicationDateTime => PublicationTimestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Announcement's title
    /// </summary>
    [JsonProperty("title")]
    public string Title { get; set; }

    /// <summary>
    ///   The number of previous unread announcements (optional, only for authorized users).
    /// </summary>
    [JsonProperty("unread")]
    public int Unread { get; set; }
  }
}
