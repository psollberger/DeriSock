namespace DeriSock.JsonRpc
{
  using Newtonsoft.Json;

  public class Request
  {
    [JsonProperty("jsonrpc")] public string JsonRpc { get; set; } = "2.0";

    [JsonProperty("id")] public int? Id { get; set; }

    [JsonProperty("method")] public string Method { get; set; }

    [JsonProperty("params")] public object Params { get; set; }
  }
}
