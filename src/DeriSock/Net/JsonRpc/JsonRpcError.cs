namespace DeriSock.Net.JsonRpc;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Represents the error information returned by the endpoint.
/// </summary>
public class JsonRpcError
{
  /// <summary>
  ///   The error code.
  /// </summary>
  [JsonProperty("code")]
  public int Code { get; set; }

  /// <summary>
  ///   The error message.
  /// </summary>
  [JsonProperty("message")]
  public string Message { get; set; } = string.Empty;

  /// <summary>
  ///   Additional data for the error. Content depends on the error.
  /// </summary>
  [JsonProperty("data")]
  public JToken? Data { get; set; }

  /// <inheritdoc />
  public override string ToString()
  {
    if (Data is null)
      return $"{Code}: {Message}";
    
    return $"{Code}: {Message} ({Data})";
  }
}
