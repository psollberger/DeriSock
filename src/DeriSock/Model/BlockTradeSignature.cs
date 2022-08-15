namespace DeriSock.Model;

using Newtonsoft.Json;

public class BlockTradeSignature
{
  /// <summary>
  ///   <para>Signature of block trade</para>
  ///   <para>It is valid only for 1 minute "around" given timestamp</para>
  /// </summary>
  [JsonProperty("signature")]
  public string Signature { get; set; }
}
