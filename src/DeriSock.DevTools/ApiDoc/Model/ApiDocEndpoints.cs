namespace DeriSock.DevTools.ApiDoc.Model;

using System.Text.Json.Serialization;

public class ApiDocEndpoints
{
  [JsonPropertyName("production")]
  public string Production { get; set; } = "wss://www.deribit.com/ws/api/v2";

  [JsonPropertyName("testnet")]
  public string TestNet { get; set; } = "wss://test.deribit.com/ws/api/v2";
}
