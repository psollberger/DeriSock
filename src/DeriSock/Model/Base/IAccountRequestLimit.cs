namespace DeriSock.Model;

public interface IAccountRequestLimit
{
  /// <summary>
  ///   Maximum number of requests allowed for user in burst mode
  /// </summary>
  int Burst { get; set; }

  /// <summary>
  ///   Number of requests per second allowed for user
  /// </summary>
  int Rate { get; set; }
}
