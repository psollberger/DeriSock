namespace DeriSock.Response
{
  using Newtonsoft.Json;

  public class TestResponseData
  {
    [JsonProperty("version")]
    public string Version { get; set; }
  }
}
