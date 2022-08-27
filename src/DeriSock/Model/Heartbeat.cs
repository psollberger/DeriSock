namespace DeriSock.Model;

using DeriSock.Net.JsonRpc;

using Newtonsoft.Json;

/// <summary>
///   Represents the heartbeat message received from the endpoint.
/// </summary>
public class Heartbeat : JsonRpcRequest
{
  /// <summary>
  ///   The type of the heartbeat message.
  /// </summary>
  [JsonProperty("type")]
  public string Type { get; set; } = string.Empty;
}
