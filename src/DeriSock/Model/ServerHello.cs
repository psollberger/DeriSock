namespace DeriSock.Model;

using Newtonsoft.Json;

public class ServerHello
{
  [JsonProperty("version")]
  public string Version { get; set; }
}
