namespace DeriSock.DevTools.UseCases;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class CreateEnumMap : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(Consts.FinalDocumentPath, cancellationToken).ConfigureAwait(false);
    var map = await ApiDocUtils.ReadEnumMapAsync(Consts.EnumMapPath, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.CreateAndWriteEnumMapAsync(apiDoc, map, Consts.EnumMapPath, cancellationToken).ConfigureAwait(false);
  }
}