namespace DeriSock.Model
{
  using DeriSock.JsonRpc;
  using Newtonsoft.Json;

  public class Heartbeat : JsonRpcRequest
  {
    [JsonProperty("params")]
    public new HeartbeatParams Params { get; set; }

    public string Type => Params.Type;

    public class HeartbeatParams
    {
      [JsonProperty("type")]
      public string Type { get; set; }
    }
  }
}
