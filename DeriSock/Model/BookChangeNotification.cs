namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class BookChangeNotification
  {
    /// <summary>
    ///   The first notification will contain the amounts for all price levels (a list of ["new", price, amount] tuples). All
    ///   following notifications will contain a list of tuples with action, price level and new amount ([action, price,
    ///   amount]). Action can be new, change or delete.
    /// </summary>
    [JsonProperty("asks")]
    public ActionItem[] Asks { get; set; }

    /// <inheritdoc cref="Asks" />
    [JsonProperty("bids")]
    public ActionItem[] Bids { get; set; }

    /// <summary>
    ///   Identifier of the notification
    /// </summary>
    [JsonProperty("change_id")]
    public long ChangeId { get; set; }

    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    /// <summary>
    ///   Identifier of the previous notification (it's not included for the first notification)
    /// </summary>
    [JsonProperty("prev_change_id")]
    public long PrevChangeId { get; set; }

    /// <summary>
    ///   The timestamp of last change (milliseconds since the Unix epoch)
    /// </summary>
    [JsonProperty("timestamp")]
    public long Timestamp { get; set; }

    /// <inheritdoc cref="Timestamp" />
    [JsonIgnore]
    public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();

    /// <summary>
    ///   Type of notification: snapshot for initial, change for others. The field is only included in aggregated response
    ///   (when input parameter interval != raw)
    /// </summary>
    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonConverter(typeof(ActionItemConverter))]
    public class ActionItem
    {
      public string Action { get; set; }
      public decimal Price { get; set; }
      public decimal Amount { get; set; }
    }

    public class ActionItemConverter : JsonConverter<ActionItem>
    {
      public override bool CanWrite => false;

      public override void WriteJson(JsonWriter writer, ActionItem value, JsonSerializer serializer)
      {
        throw new NotSupportedException();
      }

      public override ActionItem ReadJson(
        JsonReader reader, Type objectType,
        ActionItem existingValue, bool hasExistingValue,
        JsonSerializer serializer)
      {
        var arr = JArray.Load(reader);
        return new ActionItem {Action = arr[0].Value<string>(), Price = arr[1].Value<decimal>(), Amount = arr[2].Value<decimal>()};
      }
    }
  }
}
