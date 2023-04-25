namespace DeriSock.DevTools.UseCases;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class CreateRequestOverrides : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(Consts.FinalDocumentPath, cancellationToken).ConfigureAwait(false);
    var map = await ApiDocUtils.ReadRequestMapAsync(Consts.RequestMapPath, cancellationToken).ConfigureAwait(false);

    if (map is not null)
      await ApiDocUtils.WriteRequestOverridesFromMapAsync(apiDoc, map, Consts.RequestOverridesPath, cancellationToken).ConfigureAwait(false);
  }
}
