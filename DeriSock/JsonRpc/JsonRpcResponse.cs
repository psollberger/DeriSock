namespace DeriSock.JsonRpc
{
  using System;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class JsonRpcResponse<T> : JsonRpcResponse
  {
    [JsonIgnore]
    public T ResultData { get; set; }

    public JsonRpcResponse(JsonRpcResponse response, T value)
    {
      Original = response.Original;
      JsonRpc = response.JsonRpc;
      Id = response.Id;
      Result = response.Result;
      Error = response.Error;
      Testnet = response.Testnet;
      UsIn = response.UsIn;
      UsOut = response.UsOut;
      UsDiff = response.UsDiff;
      ResultData = value;
    }
  }

  public class JsonRpcResponse
  {
    [JsonIgnore]
    public string Original { get; set; }

    [JsonProperty("jsonrpc")]
    public string JsonRpc { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("result")]
    public JToken Result { get; set; }

    [JsonProperty("error")]
    public JsonRpcError Error { get; set; }

    public JsonRpcResponse<T> CreateTyped<T>(T value)
    {
      return new JsonRpcResponse<T>(this, value);
    }

    #region Fields not part of the JSON-RPC standard

    [JsonProperty("testnet")]
    public bool Testnet { get; set; }

    [JsonProperty("usIn")]
    public long UsIn { get; set; }

    [JsonIgnore]
    public DateTime UsInDateTime => UsIn.AsDateTimeFromMicroseconds();

    [JsonProperty("usOut")]
    public long UsOut { get; set; }

    [JsonIgnore]
    public DateTime UsOutDateTime => UsOut.AsDateTimeFromMicroseconds();

    [JsonProperty("usDiff")]
    public int UsDiff { get; set; }

    #endregion
  }
}
