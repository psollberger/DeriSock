﻿namespace DeriSock.JsonRpc
{
  using Newtonsoft.Json;

  public class Heartbeat : Request
  {
    [JsonProperty("params")] public new HeartbeatParams Params { get; set; }

    public string Type => Params.Type;

    public class HeartbeatParams
    {
      [JsonProperty("type")] public string Type { get; set; }
    }
  }
}
