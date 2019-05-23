namespace DeriSock
{
  using System;
  using Model;

  public class EventResponseReceivedEventArgs : MessageReceivedEventArgs
  {
    public EventResponseReceivedEventArgs(string message, EventResponse eventData) : base(message)
    {
      EventData = eventData;
    }

    public EventResponse EventData { get; }
  }
}
