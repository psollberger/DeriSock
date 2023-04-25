namespace DeriSock.DevTools.UseCases;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class CreateObjectMap : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(Consts.FinalDocumentPath, cancellationToken).ConfigureAwait(false);
    var map = await ApiDocUtils.ReadObjectMapAsync(Consts.ObjectMapPath, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.CreateAndWriteObjectMapAsync(apiDoc, map, Consts.ObjectMapPath, cancellationToken).ConfigureAwait(false);
  }
}