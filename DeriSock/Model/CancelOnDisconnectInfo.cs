namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class CancelOnDisconnectInfo
  {
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }
  }
}
