namespace DeriSock.Events
{
  using Model;
  using Utils;

  public class MessageResponseReceivedEventArgs : MessageReceivedEventArgs
  {
    public MessageResponseReceivedEventArgs(string message, JsonRpcResponse response, TaskInfo taskInfo) : base(message)
    {
      Response = response;
      TaskInfo = taskInfo;
    }

    public JsonRpcResponse Response { get; }
    public TaskInfo TaskInfo { get; }

  }
}
