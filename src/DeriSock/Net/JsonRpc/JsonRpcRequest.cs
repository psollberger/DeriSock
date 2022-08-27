namespace DeriSock.Net.JsonRpc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Represents a request that can be sent to the endpoint or was received from the endpoint (in case of a notification).
/// </summary>
public class JsonRpcRequest
{
  /// <summary>
  ///   The original JSON that was received from the endpoint (only for notifications).
  /// </summary>
  [JsonIgnore]
  public JObject? Original { get; set; }

  /// <summary>
  ///   The JSON-RPC version.
  /// </summary>
  [JsonProperty("jsonrpc")]
  public string JsonRpc { get; set; } = "2.0";

  /// <summary>
  ///   The request id.
  /// </summary>
  [JsonProperty("id")]
  public int Id { get; set; }

  /// <summary>
  ///   The request method.
  /// </summary>
  [JsonProperty("method")]
  public string Method { get; set; } = string.Empty;

  /// <summary>
  ///   The parameters for the request method.
  /// </summary>
  [JsonProperty("params")]
  public object? Params { get; set; }
}
