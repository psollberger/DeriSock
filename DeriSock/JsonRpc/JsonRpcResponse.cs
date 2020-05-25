namespace DeriSock.JsonRpc
{
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class JsonRpcResponse
  {
    [JsonProperty("jsonrpc")] public string JsonRpc { get; set; }

    [JsonProperty("id")] public int Id { get; set; }

    [JsonProperty("result")] public JToken Result { get; set; }

    [JsonProperty("error")] public JsonRpcError JsonRpcError { get; set; }

    // Fields not part of the JSON-RPC standard

    [JsonProperty("testnet")] public bool Testnet { get; set; }

    [JsonProperty("usIn")] public long UsIn { get; set; }

    [JsonProperty("usOut")] public long UsOut { get; set; }

    [JsonProperty("usDiff")] public int UsDiff { get; set; }
  }
}
