namespace DeriSock;

using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient : ISessionManagementApi
{
  /// <inheritdoc cref="ISessionManagementApi" />
  ISessionManagementApi ICategoriesApi.SessionManagement()
    => this;

  /// <inheritdoc cref="ISessionManagementApi.PublicSetHeartbeat" />
  public async Task<JsonRpcResponse<string?>> PublicSetHeartbeat(PublicSetHeartbeatRequest args)
  {
    return await Send("public/set_heartbeat", args, new ObjectJsonConverter<string?>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="ISessionManagementApi.PublicDisableHeartbeat" />
  public async Task<JsonRpcResponse<string?>> PublicDisableHeartbeat()
  {
    return await Send("public/disable_heartbeat", null, new ObjectJsonConverter<string?>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="ISessionManagementApi.PrivateEnableCancelOnDisconnect" />
  public async Task<JsonRpcResponse<string?>> PrivateEnableCancelOnDisconnect(PrivateEnableCancelOnDisconnectRequest? args)
  {
    return await Send("public/enable_cancel_on_disconnect", args, new ObjectJsonConverter<string?>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="ISessionManagementApi.PrivateDisableCancelOnDisconnect" />
  public async Task<JsonRpcResponse<string?>> PrivateDisableCancelOnDisconnect(PrivateDisableCancelOnDisconnectRequest? args)
  {
    return await Send("public/disable_cancel_on_disconnect", args, new ObjectJsonConverter<string?>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="ISessionManagementApi.PrivateGetCancelOnDisconnect" />
  public async Task<JsonRpcResponse<PrivateGetCancelOnDisconnectResponse>> PrivateGetCancelOnDisconnect(PrivateGetCancelOnDisconnectRequest? args)
  {
    return await Send("public/get_cancel_on_disconnect", args, new ObjectJsonConverter<PrivateGetCancelOnDisconnectResponse>()).ConfigureAwait(false);
  }
}
