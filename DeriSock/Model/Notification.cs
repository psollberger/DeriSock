namespace DeriSock.Model
{
  using System;
  using DeriSock.JsonRpc;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Linq;

  public class Notification : JsonRpcRequest
  {
    [JsonIgnore]
    public DateTime Timestamp { get; set; }

    [JsonProperty("params")]
    public new NotificationParams Params { get; set; }

    public string Channel => Params.Channel;

    public JToken Data => Params.Data;

    public class NotificationParams
    {
      [JsonProperty("channel")]
      public string Channel { get; set; }

      [JsonProperty("data")]
      public JToken Data { get; set; }
    }
  }
}
