namespace DeriSock.Events
{
  using System;

  public class MessageReceivedEventArgs : EventArgs
  {
    public string Message { get; }

    public MessageReceivedEventArgs(string message)
    {
      Message = message;
    }
  }
}
