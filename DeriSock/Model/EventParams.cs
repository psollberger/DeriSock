namespace DeriSock.Model
{
  using System;
  using Newtonsoft.Json.Linq;

  public class EventParams
  {
    public string type;
    public string channel;
    public JToken data;
    public DateTime timestamp;
  }
}
