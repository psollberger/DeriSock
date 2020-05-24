namespace DeriSock.JsonRpc
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class Error
  {
    [JsonProperty("code")] public int Code { get; set; }

    [JsonProperty("message")] public string Message { get; set; }

    [JsonProperty("data")] public JToken Data { get; set; }
  }
}
