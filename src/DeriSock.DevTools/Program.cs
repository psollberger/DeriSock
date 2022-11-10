//#define TEST

namespace DeriSock.DevTools;

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;

using DeriSock.DevTools.ApiDoc;

using Microsoft.Extensions.Configuration;

using Serilog;

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
  private const string ApiDocFinalDocumentPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.json";
  private const string ApiDocEnumMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.enum-map.json";
  private const string ApiDocEnumOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.30.enum-types.overrides.json";
  private const string ApiDocObjectMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.object-map.json";
  private const string ApiDocObjectOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.31.object-types.overrides.json";
  private const string ApiDocRequestMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.request-map.json";
  private const string ApiDocRequestOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.32.request-types.overrides.json";

  public static async Task Main(string[] args)
  {
    var configuration = new ConfigurationBuilder()
      .SetBasePath(Directory.GetCurrentDirectory())
      .AddJsonFile("appsettings.json")
      .Build();

    Log.Logger = new LoggerConfiguration()
      .ReadFrom.Configuration(configuration)
      .CreateLogger();

    try {
      var cmdLineParser = new Parser(
        config =>
        {
          config.AllowMultiInstance = false;
          config.CaseInsensitiveEnumValues = true;
          config.HelpWriter = Console.Out;
          config.IgnoreUnknownArguments = true;
        });

      await cmdLineParser.ParseArguments<RunOptions>(args).WithParsedAsync(RunWithOptions);
    }
    catch (Exception ex) {
      Console.WriteLine(ex.ToString());
    }
  }

  private static async Task RunWithOptions(RunOptions options)
  {
    if (options.ScratchPad) {
      var cts = new CancellationTokenSource();
      var scratchPadTask = ScratchPad.Run(cts.Token);
      Console.WriteLine("Press any key to signal CancellationToken ...");
      Console.ReadLine();
      cts.Cancel();
      await scratchPadTask;
      return;
    }

    if (options.CreateBaseDocument) {
      var apiDoc = await ApiDocUtils.BuildApiDocumentAsync(ApiDocumentationUrl).ConfigureAwait(false);
      await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocBaseDocumentPath).ConfigureAwait(false);
    }

    if (options.CreateFinalDocument) {
      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocBaseDocumentPath).ConfigureAwait(false);
      await ApiDocUtils.ApplyOverridesAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
      await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
    }

    if (options.CreateEnumMap) {
      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
      var map = await ApiDocUtils.ReadEnumMapAsync(ApiDocEnumMapPath).ConfigureAwait(false);
      await ApiDocUtils.CreateAndWriteEnumMapAsync(apiDoc, map, ApiDocEnumMapPath).ConfigureAwait(false);
    }

    if (options.CreateEnumOverrides) {
      var map = await ApiDocUtils.ReadEnumMapAsync(ApiDocEnumMapPath).ConfigureAwait(false);

      if (map is not null) {
        await ApiDocUtils.WriteEnumOverridesFromMapAsync(map, ApiDocEnumOverridesPath).ConfigureAwait(false);

        if (options.CreateFinalDocument) {
          var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocBaseDocumentPath).ConfigureAwait(false);
          await ApiDocUtils.ApplyOverridesAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
          await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
        }
      }
    }

    if (options.CreateObjectMap) {
      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
      var map = await ApiDocUtils.ReadObjectMapAsync(ApiDocObjectMapPath).ConfigureAwait(false);
      await ApiDocUtils.CreateAndWriteObjectMapAsync(apiDoc, map, ApiDocObjectMapPath).ConfigureAwait(false);
    }

    if (options.CreateObjectOverrides) {
      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
      var map = await ApiDocUtils.ReadObjectMapAsync(ApiDocObjectMapPath).ConfigureAwait(false);

      if (map is not null) {
        await ApiDocUtils.WriteObjectOverridesFromMapAsync(apiDoc, map, ApiDocObjectOverridesPath).ConfigureAwait(false);

        if (options.CreateFinalDocument) {
          apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocBaseDocumentPath).ConfigureAwait(false);
          await ApiDocUtils.ApplyOverridesAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
          await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
        }
      }
    }

    if (options.CreateRequestMap) {
      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
      var map = await ApiDocUtils.ReadRequestMapAsync(ApiDocRequestMapPath).ConfigureAwait(false);
      await ApiDocUtils.CreateAndWriteRequestMapAsync(apiDoc, map, ApiDocRequestMapPath).ConfigureAwait(false);
    }

    if (options.CreateRequestOverrides) {
      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
      var map = await ApiDocUtils.ReadRequestMapAsync(ApiDocRequestMapPath).ConfigureAwait(false);

      if (map is not null) {
        await ApiDocUtils.WriteRequestOverridesFromMapAsync(apiDoc, map, ApiDocRequestOverridesPath).ConfigureAwait(false);

        if (options.CreateFinalDocument) {
          apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocBaseDocumentPath).ConfigureAwait(false);
          await ApiDocUtils.ApplyOverridesAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
          await ApiDocUtils.WriteApiDocumentAsync(apiDoc, ApiDocFinalDocumentPath).ConfigureAwait(false);
        }
      }
    }

    if (options.GenerateCode) {
      var apiDoc = await ApiDocUtils.ReadApiDocumentAsync(ApiDocFinalDocumentPath).ConfigureAwait(false);
      var enumMap = await ApiDocUtils.ReadEnumMapAsync(ApiDocEnumMapPath).ConfigureAwait(false);
      var objectMap = await ApiDocUtils.ReadObjectMapAsync(ApiDocObjectMapPath).ConfigureAwait(false);

      if (enumMap is not null && objectMap is not null) {
        DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Enums"), "*.g.cs");
        DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Objects"), "*.g.cs");
        DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Requests"), "*.g.cs");
        DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Responses"), "*.g.cs");
        DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Channels"), "*.g.cs");
        DeleteFiles(Path.Combine(DeriSockGeneratedModelsDirectory, "Notifications"), "*.g.cs");
        DeleteFiles(DeriSockGeneratedApisDirectory, "*.g.cs");

        await ApiDocUtils.GenerateEnumCodeAsync(apiDoc, enumMap, DeriSockGeneratedModelsDirectory).ConfigureAwait(false);
        await ApiDocUtils.GenerateObjectCodeAsync(apiDoc, objectMap, DeriSockGeneratedModelsDirectory).ConfigureAwait(false);
        await ApiDocUtils.GenerateRequestAndResponseClassesAsync(apiDoc, DeriSockGeneratedModelsDirectory).ConfigureAwait(false);
        await ApiDocUtils.GenerateApiInterfaces(apiDoc, DeriSockGeneratedApisDirectory).ConfigureAwait(false);
        await ApiDocUtils.GenerateApiImplementations(apiDoc, DeriSockGeneratedApisDirectory).ConfigureAwait(false);
      }
    }
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
