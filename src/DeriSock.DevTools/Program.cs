//#define TEST

namespace DeriSock.DevTools;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;
using DeriSock.DevTools.ApiDoc.Model;

internal class Program
{
  private const string ApiDocumentationUrl = "https://docs.deribit.com";
#if TEST
  private const string DeriSockGeneratedModelsDirectory = @"D:\Temp\CodeGen\Model";
  private const string DeriSockGeneratedApisDirectory = @"D:\Temp\CodeGen\Api";
#else
  private const string DeriSockGeneratedModelsDirectory = @"..\..\..\src\DeriSock\Model";
  private const string DeriSockGeneratedApisDirectory = @"..\..\..\src\DeriSock\Api";
#endif
  private const string ApiDocBaseDocumentPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.base.json";
  private const string ApiDocDocumentPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.json";
  private const string ApiDocEnumMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.enum-map.json";
  private const string ApiDocObjectMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.object-map.json";
  private const string ApiDocEnumOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.30.enum-types.overrides.json";
  private const string ApiDocObjectOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.31.object-types.overrides.json";

  public static async Task Main(string[] args)
  {
    try {
      await SubscriptionDemo();
      //await ApiDocManagement();
    }
    catch (Exception ex) {
      Console.WriteLine(ex.ToString());
    }
  }

  private static async Task SubscriptionDemo()
  {
    // Create a CancellationTokenSource to be able to cancel (unsubscribe from) the stream
    var cts = new CancellationTokenSource();

    // Run the demo in parallel
    Console.WriteLine("Subscribing to multiple channels at once and listen to all Notifications coming from them");
    var streamTask = NotificationStreamDemo.Run(cts.Token);

    Console.WriteLine("Waiting for you to press any key");
    Console.ReadKey();

    Console.WriteLine("Cancelling the stream");

    cts.Cancel();
    await Task.WhenAll(streamTask);

    Console.WriteLine("Unsubsribed. Bye Bye");
  }

  private static async Task ApiDocManagement()
  {
    //await CreateBaseApiDocument().ConfigureAwait(false);

    //var apiDoc = await BuildAndWriteApiDocument().ConfigureAwait(false);

    var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocDocumentPath).ConfigureAwait(false);
    var enumMap = await ApiDocUtils.ReadEnumMapAsync(ApiDocEnumMapPath).ConfigureAwait(false);
    var objectMap = await ApiDocUtils.ReadObjectMapAsync(ApiDocObjectMapPath).ConfigureAwait(false);

    //await ApiDocUtils.AnalyzeApiDocumentAsync(apiDoc).ConfigureAwait(false);

    //await ApiDocUtils.PrintEnumMapTemplateAsync(apiDoc, enumMap).ConfigureAwait(false);
    //await ApiDocUtils.PrintObjectMapTemplateAsync(apiDoc, objectMap).ConfigureAwait(false);

    //await ApiDocUtils.WriteEnumOverridesFromMapAsync(enumMap, ApiDocEnumOverridesPath).ConfigureAwait(false);
    //await ApiDocUtils.WriteObjectOverridesFromMapAsync(apiDoc, objectMap, ApiDocObjectOverridesPath).ConfigureAwait(false);

    await GenerateAllCode(apiDoc, enumMap, objectMap).ConfigureAwait(false);

    await Task.CompletedTask;
  }

  private static async Task<ApiDocDocument> CreateBaseApiDocument(CancellationToken cancellationToken = default)
  {
    var apiDoc = await ApiDocUtils.BuildApiDocumentAsync(ApiDocumentationUrl, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocBaseDocumentPath, cancellationToken).ConfigureAwait(false);
    return apiDoc;
  }

  private static async Task<ApiDocDocument> BuildAndWriteApiDocument(CancellationToken cancellationToken = default)
  {
    var apiDoc = await ApiDocUtils.BuildApiDocumentAsync(ApiDocumentationUrl, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.ApplyOverridesAsync(apiDoc, ApiDocDocumentPath, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocDocumentPath, cancellationToken).ConfigureAwait(false);
    return apiDoc;
  }

  private static async Task GenerateAllCode(ApiDocDocument apiDoc, ApiDocEnumMap enumMap, ApiDocObjectMap objectMap, CancellationToken cancellationToken = default)
  {
    DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Enums"), "*.g.cs");
    DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Objects"), "*.g.cs");
    DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Requests"), "*.g.cs");
    DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Responses"), "*.g.cs");
    DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Channels"), "*.g.cs");
    DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Notifications"), "*.g.cs");
    DeleteFiles(DeriSockGeneratedApisDirectory, "*.g.cs");

    await ApiDocUtils.GenerateEnumCodeAsync(apiDoc, enumMap, DeriSockGeneratedModelsDirectory, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateObjectCodeAsync(apiDoc, objectMap, DeriSockGeneratedModelsDirectory, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateRequestAndResponseClassesAsync(apiDoc, DeriSockGeneratedModelsDirectory, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateApiInterfaces(apiDoc, DeriSockGeneratedApisDirectory, cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateApiImplementations(apiDoc, DeriSockGeneratedApisDirectory, cancellationToken).ConfigureAwait(false);
  }

  private static void DeleteFiles(string path, string searchPattern)
  {
    try {
      var filePaths = Directory.EnumerateFiles(path, searchPattern);

      foreach (var filePath in filePaths)
        File.Delete(filePath);
    }
    catch {
      // ignore
    }
  }
}
