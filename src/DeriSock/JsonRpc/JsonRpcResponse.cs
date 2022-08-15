namespace DeriSock.JsonRpc;

using System;

using DeriSock.Converter;

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
  public string? Original { get; set; }

  [JsonProperty("jsonrpc")]
  public string JsonRpc { get; set; } = null!;

  [JsonProperty("id")]
  public int Id { get; set; }

  [JsonProperty("result")]
  public JToken Result { get; set; } = null!;

  [JsonProperty("error")]
  public JsonRpcError? Error { get; set; }

  public JsonRpcResponse<T> CreateTyped<T>(T value)
  {
    return new JsonRpcResponse<T>(this, value);
  }

  #region Fields not part of the JSON-RPC standard

  [JsonProperty("testnet")]
  public bool Testnet { get; set; }

  [JsonProperty("usIn")]
  [JsonConverter(typeof(MicrosecondsTimestampConverter))]
  public DateTime UsIn { get; set; }

  [JsonProperty("usOut")]
  [JsonConverter(typeof(MicrosecondsTimestampConverter))]
  public DateTime UsOut { get; set; }

  [JsonProperty("usDiff")]
  public long UsDiff { get; set; }

  #endregion
}
