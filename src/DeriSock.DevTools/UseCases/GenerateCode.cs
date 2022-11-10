namespace DeriSock.DevTools.UseCases;

using System.IO;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;

using JetBrains.Annotations;

[UsedImplicitly]
internal sealed class GenerateCode : IUseCase
{
  /// <inheritdoc />
  public async Task Run(CancellationToken cancellationToken)
  {
    var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(Consts.FinalDocumentPath, cancellationToken).ConfigureAwait(false);
    var enumMap = await ApiDocUtils.ReadEnumMapAsync(Consts.EnumMapPath, cancellationToken).ConfigureAwait(false);
    var objectMap = await ApiDocUtils.ReadObjectMapAsync(Consts.ObjectMapPath, cancellationToken).ConfigureAwait(false);

    if (enumMap is not null && objectMap is not null)
    {
      Path.Combine(Consts.DeriSockGeneratedModelsDirectory, "Enums").DeleteFiles("*.g.cs");
      Path.Combine(Consts.DeriSockGeneratedModelsDirectory, "Objects").DeleteFiles("*.g.cs");
      Path.Combine(Consts.DeriSockGeneratedModelsDirectory, "Requests").DeleteFiles("*.g.cs");
      Path.Combine(Consts.DeriSockGeneratedModelsDirectory, "Responses").DeleteFiles("*.g.cs");
      Path.Combine(Consts.DeriSockGeneratedModelsDirectory, "Channels").DeleteFiles("*.g.cs");
      Path.Combine(Consts.DeriSockGeneratedModelsDirectory, "Notifications").DeleteFiles("*.g.cs");
      Consts.DeriSockGeneratedApisDirectory.DeleteFiles("*.g.cs");

      await ApiDocUtils.GenerateEnumCodeAsync(apiDoc, enumMap, Consts.DeriSockGeneratedModelsDirectory, cancellationToken).ConfigureAwait(false);
      await ApiDocUtils.GenerateObjectCodeAsync(apiDoc, objectMap, Consts.DeriSockGeneratedModelsDirectory, cancellationToken).ConfigureAwait(false);
      await ApiDocUtils.GenerateRequestAndResponseClassesAsync(apiDoc, Consts.DeriSockGeneratedModelsDirectory, cancellationToken).ConfigureAwait(false);
      await ApiDocUtils.GenerateApiInterfaces(apiDoc, Consts.DeriSockGeneratedApisDirectory, cancellationToken).ConfigureAwait(false);
      await ApiDocUtils.GenerateApiImplementations(apiDoc, Consts.DeriSockGeneratedApisDirectory, cancellationToken).ConfigureAwait(false);
    }
  }
}