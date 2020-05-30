namespace DeriSock.Response
{
  using Newtonsoft.Json;

  public class HelloResponseData
  {
    [JsonProperty("version")]
    public string Version { get; set; }
  }
}
