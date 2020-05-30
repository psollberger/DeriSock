namespace DeriSock.Response
{
  using Newtonsoft.Json;

  public class GetCancelOnDisconnectResponseData
  {
    [JsonProperty("enabled")]
    public bool Enabled { get; set; }

    [JsonProperty("scope")]
    public string Scope { get; set; }
  }
}
