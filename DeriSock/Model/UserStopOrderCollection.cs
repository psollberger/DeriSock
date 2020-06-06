namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class UserStopOrderCollection
  {
    /// <summary>
    ///   Continuation token for pagination
    /// </summary>
    [JsonProperty("continuation")]
    public string Continuation { get; set; }

    [JsonProperty("entities")]
    public UserStopOrder[] Entities { get; set; }
  }
}
