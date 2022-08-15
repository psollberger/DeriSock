namespace DeriSock.DevTools;

using System;
using System.Linq;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;
using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.Model;

using Newtonsoft.Json;

internal class Program
{
  private const string ApiDocumentationUrl = "https://docs.deribit.com";
  private const string DeriSockGeneratedDirectory = @"C:\Users\psoll\source\repos\DeriSock\src\DeriSock\Model\Generated";

  private const string ApiDocBaseDocumentPath = @"C:\Users\psoll\source\repos\DeriSock\src\DeriSock.DevTools\deribit.api.v211.base.json";
  private const string ApiDocDocumentPath = @"C:\Users\psoll\source\repos\DeriSock\src\DeriSock.DevTools\deribit.api.v211.json";
  private const string ApiDocEnumMapPath = @"C:\Users\psoll\source\repos\DeriSock\src\DeriSock.DevTools\deribit.api.enum-map.json";
  private const string ApiDocObjectMapPath = @"C:\Users\psoll\source\repos\DeriSock\src\DeriSock.DevTools\deribit.api.object-map.json";
  private const string GeneratedSingleFileDirectory = @"C:\Users\psoll\source\repos\DeriSock\src\DeriSock.DevTools";
  private const string GeneratedSingleFileName = @"GeneratedCode";

  public static async Task Main(string[] args)
  {
    try {
      //var apiDoc = await ApiDocUtils.BuildApiDocumentAsync(ApiDocumentationUrl).ConfigureAwait(false);
      //await ApiDocUtils.ApplyOverridesAsync(apiDoc, ApiDocDocumentPath).ConfigureAwait(false);
      //await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocDocumentPath).ConfigureAwait(false);
      //await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocBaseDocumentPath).ConfigureAwait(false);


      //var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocDocumentPath).ConfigureAwait(false);

      //await ApiDocUtils.AnalyzeApiDocumentAsync(apiDoc).ConfigureAwait(false);


      //var enumMap = await ApiDocUtils.ReadEnumMapAsync(ApiDocEnumMapPath).ConfigureAwait(false);
      //await ApiDocUtils.PrintEnumMapTemplateAsync(apiDoc, enumMap).ConfigureAwait(false);
      //await ApiDocUtils.PrintEnumOverridesFromMapAsync(enumMap).ConfigureAwait(false);
      //await ApiDocUtils.GenerateEnumCodeAsync(enumMap, DeriSockGeneratedDirectory, "Enums").ConfigureAwait(false);


      //var objectMap = await ApiDocUtils.ReadObjectMapAsync(ApiDocObjectMapPath).ConfigureAwait(false);
      //await ApiDocUtils.PrintObjectMapTemplateAsync(apiDoc, objectMap).ConfigureAwait(false);
      //await ApiDocUtils.PrintObjectOverridesFromMapAsync(apiDoc, objectMap).ConfigureAwait(false);
      //await ApiDocUtils.GenerateObjectCodeAsync(objectMap, DeriSockGeneratedDirectory, "Objects").ConfigureAwait(false);


      //await ApiDocUtils.GenerateRequestClassesAsync(apiDoc, DeriSockGeneratedDirectory, "Requests").ConfigureAwait(false);
      //await ApiDocUtils.GenerateResponseClassesAsync(apiDoc, DeriSockGeneratedDirectory, "Responses").ConfigureAwait(false);


      await Playground().ConfigureAwait(false);
    }
    catch (Exception ex) {
      Console.WriteLine(ex.ToString());
    }
  }

  private static Task Playground()
  {
    return Task.CompletedTask;
  }
}
