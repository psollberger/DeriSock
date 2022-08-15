using DeriSock.Api;

namespace DeriSock;

public partial class DeribitClient : IPrivateApi
{
  /// <inheritdoc />
  bool IPrivateApi.Logout(bool? invalidateToken)
    => PrivateLogout(invalidateToken);
}
