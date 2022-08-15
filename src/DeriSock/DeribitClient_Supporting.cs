namespace DeriSock;

using System;
using System.Threading.Tasks;

using DeriSock.Api;
using DeriSock.Converter;
using DeriSock.JsonRpc;
using DeriSock.Model;

public partial class DeribitClient : ISupportingApi
{
  /// <inheritdoc cref="ISessionManagementApi" />
  ISupportingApi ICategoriesApi.Supporting()
    => this;

  /// <inheritdoc cref="ISupportingApi.PublicGetTime" />
  public async Task<JsonRpcResponse<DateTime>> PublicGetTime()
  {
    return await Send("public/get_time", null, new TimestampJsonConverter()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="ISupportingApi.PublicHello" />
  public async Task<JsonRpcResponse<PublicHelloResponse>> PublicHello(PublicHelloRequest args)
  {
    return await Send("public/hello", args, new ObjectJsonConverter<PublicHelloResponse>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="ISupportingApi.PublicStatus" />
  public async Task<JsonRpcResponse<PublicStatusResponse>> PublicStatus()
  {
    return await Send("public/status", null, new ObjectJsonConverter<PublicStatusResponse>()).ConfigureAwait(false);
  }

  /// <inheritdoc cref="ISupportingApi.PublicTest" />
  public async Task<JsonRpcResponse<PublicTestResponse>> PublicTest(PublicTestRequest? args)
  {
    return await Send("public/test", args, new ObjectJsonConverter<PublicTestResponse>()).ConfigureAwait(false);
  }
}
