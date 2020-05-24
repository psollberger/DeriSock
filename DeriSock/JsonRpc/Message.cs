namespace DeriSock.JsonRpc
{
  using System;

  public class Message
  {
    public DateTime ReceiveStart { get; set; }
    public DateTime ReceiveEnd { get; set; }
    public string Data { get; set; }

    public bool IsValid => !string.IsNullOrEmpty(Data);
  }
}
