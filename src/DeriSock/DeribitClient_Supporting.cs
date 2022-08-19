namespace DeriSock;

using System;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<DateTime>> InternalPublicGetTime(CancellationToken cancellationToken = default)
    => await Send("public/get_time", null, new TimestampJsonConverter(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ServerVersionData>> InternalPublicHello(PublicHelloRequest args, CancellationToken cancellationToken = default)
    => await Send("public/hello", args, new ObjectJsonConverter<ServerVersionData>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<PlatformLockStatus>> InternalPublicStatus(CancellationToken cancellationToken = default)
    => await Send("public/status", null, new ObjectJsonConverter<PlatformLockStatus>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<ServerVersionData>> InternalPublicTest(PublicTestRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("public/test", args, new ObjectJsonConverter<ServerVersionData>(), cancellationToken).ConfigureAwait(false);
}
