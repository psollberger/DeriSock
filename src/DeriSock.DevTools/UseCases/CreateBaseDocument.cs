namespace DeriSock.DevTools.UseCases;

using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class CreateBaseDocument : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var apiDoc = await ApiDocUtils.BuildApiDocumentAsync(Consts.DocumentationUrl, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.WriteApiDocumentAsync(apiDoc, Consts.BaseDocumentPath, cancellationToken).ConfigureAwait(false);
  }
}
