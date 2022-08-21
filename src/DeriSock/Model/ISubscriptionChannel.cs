namespace DeriSock.Request;

public interface ISubscriptionChannel
{
  /// <summary>
  /// Constructs the channel name.
  /// </summary>
  /// <returns>The constructed channel name.</returns>
  string ToChannelName();
}
