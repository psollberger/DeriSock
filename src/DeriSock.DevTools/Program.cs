namespace DeriSock.DevTools;

using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc;
using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.CodeDom;

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

      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocDocumentPath).ConfigureAwait(false);
      //var enumMap = await ApiDocUtils.ReadEnumMapAsync(ApiDocEnumMapPath).ConfigureAwait(false);
      //var objectMap = await ApiDocUtils.ReadObjectMapAsync(ApiDocObjectMapPath).ConfigureAwait(false);
      //await GenerateAllCode(apiDoc, enumMap, objectMap).ConfigureAwait(false);

      //await ApiDocUtils.AnalyzeApiDocumentAsync(apiDoc).ConfigureAwait(false);

      //await ApiDocUtils.PrintEnumMapTemplateAsync(apiDoc, enumMap).ConfigureAwait(false);
      //await ApiDocUtils.PrintEnumOverridesFromMapAsync(enumMap).ConfigureAwait(false);

      //await ApiDocUtils.PrintObjectMapTemplateAsync(apiDoc, objectMap).ConfigureAwait(false);
      //await ApiDocUtils.PrintObjectOverridesFromMapAsync(apiDoc, objectMap).ConfigureAwait(false);

      await Playground(apiDoc).ConfigureAwait(false);
    }
    catch (Exception ex) {
      Console.WriteLine(ex.ToString());
    }
  }

  private static Task Playground(ApiDocDocument apiDoc)
  {
    var functionsPerCategory = apiDoc.Methods.GroupBy(x => x.Value.Category);

    foreach (var category in functionsPerCategory) {
      Debug.Assert(!string.IsNullOrEmpty(category.Key));

      var categorieName = category.Key.ToPublicCodeName();
      var interfaceName = $"I{categorieName}Api";

      var sb = new StringBuilder();
      var itw = new IndentedTextWriter(new StringWriter(sb), "  ");
      itw.WriteLine($"public partial class DeribitClient : {interfaceName}");
      itw.WriteLine("{");
      itw.Indent++;

      itw.WriteLine($"/// <inheritdoc cref=\"{interfaceName}\" />");
      itw.WriteLine($"{interfaceName} ICategoriesApi.{categorieName}()");
      itw.Indent++;
      itw.WriteLine($"=> this;");
      itw.Indent--;

      foreach (var (_, function) in category) {
        itw.WriteLine();

        var functionName = function.Name.ToPublicCodeName();

        // Determine return type
        var returnType = string.Empty;

        if (function.Response is { Properties.Count: > 0 }) {
          returnType = $"{functionName}Response";
        }
        else if (function.Response is not null) {
          var dataTypeInfo = function.Response.GetDataTypeInfo();

          returnType = dataTypeInfo.TypeName;

          if (dataTypeInfo.IsArray)
            returnType = $"{returnType}[]";

          if (dataTypeInfo.IsNullable)
            returnType += "?";
        }

        var fullReturnType = string.IsNullOrEmpty(returnType) ? "JsonRpcResponse" : $"JsonRpcResponse<{returnType}>";
        var objectConverter = string.IsNullOrEmpty(returnType) ? "null" : $"new ObjectJsonConverter<{returnType}>()";
        // Determine parameters

        var hasParameters = false;
        var parameters = "()";

        if (function.Request is { Properties.Count: > 0 }) {
          hasParameters = true;
          var required = function.Request.IsAnyPropertyRequired;
          parameters = $"({functionName}Request{(required ? string.Empty : "?")} args)";
        }

        itw.WriteLine($"/// <inheritdoc cref=\"{interfaceName}.{functionName}\" />");
        itw.WriteLine($"public async Task<{fullReturnType}> {functionName}{parameters}");
        itw.WriteLine("{");
        itw.Indent++;
        itw.WriteLine($"return await Send(\"{function.Name}\", {(hasParameters ? "args" : "null")}, {objectConverter}).ConfigureAwait(false);");
        itw.Indent--;
        itw.WriteLine("}");
      }

      itw.Indent--;
      itw.WriteLine("}");

      Console.WriteLine(sb.ToString());
    }

    return Task.CompletedTask;
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
    await ApiDocUtils.GenerateEnumCodeAsync(enumMap, DeriSockGeneratedModelsDirectory, "Enums", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateObjectCodeAsync(objectMap, DeriSockGeneratedModelsDirectory, "Objects", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateRequestClassesAsync(apiDoc, DeriSockGeneratedModelsDirectory, "Requests", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateResponseClassesAsync(apiDoc, DeriSockGeneratedModelsDirectory, "Responses", cancellationToken).ConfigureAwait(false);
    await ApiDocUtils.GenerateApiInterfaces(apiDoc, DeriSockGeneratedApisDirectory, cancellationToken).ConfigureAwait(false);
  }

}
