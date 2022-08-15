namespace DeriSock.Model;

using Newtonsoft.Json;

public class PlatformStateNotification
{
  /// <summary>
  ///   Value is set to <c>true</c> when platform is locked
  /// </summary>
  [JsonProperty("locked")]
  public bool Locked { get; set; }
}
