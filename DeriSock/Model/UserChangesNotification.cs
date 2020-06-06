namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class UserChangesNotification
  {
    /// <summary>
    ///   Unique instrument identifier
    /// </summary>
    [JsonProperty("instrument_name")]
    public string InstrumentName { get; set; }

    [JsonProperty("orders")]
    public UserOrder[] Orders { get; set; }

    [JsonProperty("position")]
    public UserPosition[] Position { get; set; }

    [JsonProperty("trades")]
    public UserTrade[] Trades { get; set; }
  }
}
