namespace DeriSock.Net.JsonRpc;

using System;

using DeriSock.Converter;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

/// <summary>
///   Contains typed data for a JSON-RPC response.
/// </summary>
/// <typeparam name="T">The type of the data contained in the response.</typeparam>
public class JsonRpcResponse<T> : JsonRpcResponse
{
  /// <summary>
  ///   The typed data that represents the <c>Result</c> property.
  /// </summary>
  [JsonIgnore]
  public T? Data { get; set; }

  /// <summary>
  ///   Obsoltete. Use <see cref="Data" /> instead.
  /// </summary>
  [JsonIgnore]
  [Obsolete("This property is obsolete and will be removed in a further version. Use the property Data instead.")]
  public T ResultData { get; set; }

  /// <summary>
  ///   Initializes a new instance of the <see cref="JsonRpcResponse{T}" /> class.
  /// </summary>
  /// <param name="response">The base response that contains the original response.</param>
  /// <param name="value">The typed value converted from the base response.</param>
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
    Data = value;
#pragma warning disable CS0618
    ResultData = value;
#pragma warning restore CS0618
  }
}

/// <summary>
///   Contains data for a JSON-RPC response.
/// </summary>
public class JsonRpcResponse
{
  /// <summary>
  ///   The original JSON string.
  /// </summary>
  [JsonIgnore]
  public string? Original { get; set; }

  /// <summary>
  ///   The JSON-RPC version.
  /// </summary>
  [JsonProperty("jsonrpc")]
  public string JsonRpc { get; set; } = null!;

  /// <summary>
  ///   The id from the request this response belongs to.
  /// </summary>
  [JsonProperty("id")]
  public int Id { get; set; }

  /// <summary>
  ///   The result property from the JSON-RPC response as <see cref="JToken" />.
  /// </summary>
  [JsonProperty("result")]
  public JToken Result { get; set; } = null!;

  /// <summary>
  ///   The error information in case the request was not successful.
  /// </summary>
  [JsonProperty("error")]
  public JsonRpcError? Error { get; set; }

  /// <summary>
  ///   Creates a <see cref="JsonRpcResponse{T}" /> instance from the given type.
  /// </summary>
  /// <typeparam name="T">The type that represents the data in the response.</typeparam>
  /// <param name="value">The typed value converted from the response data.</param>
  /// <returns></returns>
  public JsonRpcResponse<T> CreateTyped<T>(T value)
    => new(this, value);

#region Fields not part of the JSON-RPC standard

  /// <summary>
  ///   Indicates if this response comes from the Testnet.
  /// </summary>
  [JsonProperty("testnet")]
  public bool Testnet { get; set; }

  /// <summary>
  ///   The timestamp when the request was received (microseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("usIn")]
  [JsonConverter(typeof(MicrosecondsTimestampConverter))]
  public DateTime UsIn { get; set; }

  /// <summary>
  ///   The timestamp when the response was sent (microseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("usOut")]
  [JsonConverter(typeof(MicrosecondsTimestampConverter))]
  public DateTime UsOut { get; set; }

  /// <summary>
  ///   The number of microseconds that was spent handling the request
  /// </summary>
  [JsonProperty("usDiff")]
  public long UsDiff { get; set; }

#endregion
}
