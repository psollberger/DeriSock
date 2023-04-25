namespace DeriSock.DevTools.UseCases;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class CreateRequestMap : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(Consts.FinalDocumentPath, cancellationToken).ConfigureAwait(false);
    var map = await ApiDocUtils.ReadRequestMapAsync(Consts.RequestMapPath, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.CreateAndWriteRequestMapAsync(apiDoc, map, Consts.RequestMapPath, cancellationToken).ConfigureAwait(false);
  }
}