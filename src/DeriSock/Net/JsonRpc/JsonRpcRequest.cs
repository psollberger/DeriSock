namespace DeriSock.Net.JsonRpc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class JsonRpcRequest
{
  [JsonIgnore]
  public JObject? Original { get; set; }

  [JsonProperty("jsonrpc")]
  public string JsonRpc { get; set; } = "2.0";

  [JsonProperty("id")]
  public int Id { get; set; }

  [JsonProperty("method")]
  public string Method { get; set; } = string.Empty;

  [JsonProperty("params")]
  public object? Params { get; set; }
}
