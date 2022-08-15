namespace DeriSock.DevTools;

using System;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;
using DeriSock.DevTools.ApiDoc.Model;

internal class Program
{
  private const string ApiDocumentationUrl = "https://docs.deribit.com";
  private const string DeriSockGeneratedModelsDirectory = @"..\..\..\src\DeriSock\Model";
  private const string DeriSockGeneratedApisDirectory = @"..\..\..\src\DeriSock\Api";
  private const string ApiDocBaseDocumentPath = @"..\..\..\src\DeriSock.DevTools\deribit.api.v211.base.json";
  private const string ApiDocDocumentPath = @"..\..\..\src\DeriSock.DevTools\deribit.api.v211.json";
  private const string ApiDocEnumMapPath = @"..\..\..\src\DeriSock.DevTools\deribit.api.enum-map.json";
  private const string ApiDocObjectMapPath = @"..\..\..\src\DeriSock.DevTools\deribit.api.object-map.json";

  public static async Task Main(string[] args)
  {
    try {
      //var apiDoc = await BuildAndWriteApiDocument().ConfigureAwait(false);
      //await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocBaseDocumentPath).ConfigureAwait(false);

      //var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocDocumentPath).ConfigureAwait(false);
      //var enumMap = await ApiDocUtils.ReadEnumMapAsync(ApiDocEnumMapPath).ConfigureAwait(false);
      //var objectMap = await ApiDocUtils.ReadObjectMapAsync(ApiDocObjectMapPath).ConfigureAwait(false);
      //await GenerateAllCode(apiDoc, enumMap, objectMap).ConfigureAwait(false);

      //await ApiDocUtils.AnalyzeApiDocumentAsync(apiDoc).ConfigureAwait(false);

      //await ApiDocUtils.PrintEnumMapTemplateAsync(apiDoc, enumMap).ConfigureAwait(false);
      //await ApiDocUtils.PrintEnumOverridesFromMapAsync(enumMap).ConfigureAwait(false);

      //await ApiDocUtils.PrintObjectMapTemplateAsync(apiDoc, objectMap).ConfigureAwait(false);
      //await ApiDocUtils.PrintObjectOverridesFromMapAsync(apiDoc, objectMap).ConfigureAwait(false);

      await Playground().ConfigureAwait(false);
    }
    catch (Exception ex) {
      Console.WriteLine(ex.ToString());
    }
  }

  private static Task Playground()
    => Task.CompletedTask;

  private static async Task<ApiDocDocument> BuildAndWriteApiDocument(CancellationToken cancellationToken = default)
  {
    var apiDoc = await ApiDocUtils.BuildApiDocumentAsync(ApiDocumentationUrl, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.ApplyOverridesAsync(apiDoc, ApiDocDocumentPath, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocDocumentPath, cancellationToken).ConfigureAwait(false);
    return apiDoc;
  }

  private static async Task GenerateAllCode(ApiDocDocument apiDoc, ApiDocEnumMap enumMap, ApiDocObjectMap objectMap, CancellationToken cancellationToken = default)
  {
    await ApiDocUtils.GenerateEnumCodeAsync(enumMap, DeriSockGeneratedModelsDirectory, "Enums", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateObjectCodeAsync(objectMap, DeriSockGeneratedModelsDirectory, "Objects", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateRequestClassesAsync(apiDoc, DeriSockGeneratedModelsDirectory, "Requests", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateResponseClassesAsync(apiDoc, DeriSockGeneratedModelsDirectory, "Responses", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateApiInterfaces(apiDoc, DeriSockGeneratedApisDirectory, cancellationToken).ConfigureAwait(false);
  }

}
