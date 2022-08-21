namespace DeriSock.Model;

using DeriSock.Net.JsonRpc;

using Newtonsoft.Json;

public class Heartbeat : JsonRpcRequest
{
  [JsonProperty("type")]
  public string Type { get; set; }
}
