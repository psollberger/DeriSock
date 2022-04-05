namespace DeriSock.Model;

using System;
using Newtonsoft.Json;

public class ApiKeyInfo
{
  /// <summary>
  ///   Client identifier used for authentication
  /// </summary>
  [JsonProperty("client_id")]
  public string ClientId { get; set; }

  /// <summary>
  ///   Client secret used for authentication
  /// </summary>
  [JsonProperty("client_secret")]
  public string ClientSecret { get; set; }

  /// <summary>
  ///   Informs whether this api key is default (default one is used for api console or trading view)
  /// </summary>
  [JsonProperty("default")]
  public bool Default { get; set; }

  /// <summary>
  ///   Informs whether api key is enabled and can be used for authentication
  /// </summary>
  [JsonProperty("enabled")]
  public bool Enabled { get; set; }

  /// <summary>
  ///   Key identifier
  /// </summary>
  [JsonProperty("id")]
  public int Id { get; set; }

  /// <summary>
  ///   <para>Describes maximal access for tokens generated with given key, possible values:</para>
  ///   <para>
  ///     <c>trade:[read, read_write, none]</c>, <c>wallet:[read, read_write, none]</c>,
  ///     <c>account:[read, read_write, none]</c>, <c>block_trade:[read, read_write, none]</c>
  ///   </para>
  ///   <para>If scope is not provided, it value is set as none.</para>
  ///   <para>Please check details described in <a href="https://docs.deribit.com/#access-scope">Access scope</a></para>
  /// </summary>
  [JsonProperty("max_scope")]
  public string MaxScope { get; set; }

  /// <summary>
  ///   API key name that can be displayed in transaction log
  /// </summary>
  [JsonProperty("name")]
  public string Name { get; set; }

  /// <summary>
  ///   The timestamp (milliseconds since the Unix epoch)
  /// </summary>
  [JsonProperty("timestamp")]
  public long Timestamp { get; set; }

  /// <inheritdoc cref="Timestamp" />
  [JsonIgnore]
  public DateTime DateTime => Timestamp.AsDateTimeFromMilliseconds();
}
