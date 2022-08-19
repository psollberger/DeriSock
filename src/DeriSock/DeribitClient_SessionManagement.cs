namespace DeriSock;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.Converter;
using DeriSock.Model;
using DeriSock.Net.JsonRpc;

public partial class DeribitClient
{
  private async Task<JsonRpcResponse<string>> InternalPublicSetHeartbeat(PublicSetHeartbeatRequest args, CancellationToken cancellationToken = default)
    => await Send("public/set_heartbeat", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPublicDisableHeartbeat(CancellationToken cancellationToken = default)
    => await Send("public/disable_heartbeat", null, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateEnableCancelOnDisconnect(PrivateEnableCancelOnDisconnectRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("public/enable_cancel_on_disconnect", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<string>> InternalPrivateDisableCancelOnDisconnect(PrivateDisableCancelOnDisconnectRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("public/disable_cancel_on_disconnect", args, new ObjectJsonConverter<string>(), cancellationToken).ConfigureAwait(false);

  private async Task<JsonRpcResponse<CancelOnDisconnectData>> InternalPrivateGetCancelOnDisconnect(PrivateGetCancelOnDisconnectRequest? args = null, CancellationToken cancellationToken = default)
    => await Send("public/get_cancel_on_disconnect", args, new ObjectJsonConverter<CancelOnDisconnectData>(), cancellationToken).ConfigureAwait(false);
}
