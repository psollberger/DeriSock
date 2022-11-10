namespace DeriSock.DevTools.UseCases;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class CreateFinalDocument : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(Consts.BaseDocumentPath, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.ApplyOverridesAsync(apiDoc, Consts.FinalDocumentPath, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.WriteApiDocumentAsync(apiDoc, Consts.FinalDocumentPath, cancellationToken).ConfigureAwait(false);
  }
}