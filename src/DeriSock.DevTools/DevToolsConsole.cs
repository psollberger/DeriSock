namespace DeriSock.DevTools;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using CommandLine;

using DeriSock.DevTools.UseCases;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

internal sealed class DevToolsConsole : IDevToolsConsole
{
  private readonly IServiceProvider _services;
  private readonly ILogger<DevToolsConsole> _logger;

  /// <summary>
  ///   Initializes a new instance of the <see cref="DevToolsConsole" /> class.
  /// </summary>
  public DevToolsConsole(IServiceProvider services, ILogger<DevToolsConsole> logger)
  {
    _services = services;
    _logger = logger;
  }

  public async Task Run(IEnumerable<string> args, CancellationToken cancellationToken = default)
  {
    try
    {
      var cmdLineParser = new Parser(
        config =>
        {
          config.AllowMultiInstance = false;
          config.CaseInsensitiveEnumValues = true;
          config.HelpWriter = Console.Out;
          config.IgnoreUnknownArguments = true;
        }
      );

      var parseResult = cmdLineParser.ParseArguments<RunOptions>(args);

      if (parseResult is NotParsed<RunOptions>)
        return;

      var options = parseResult.Value;

      if (options.ScratchPad)
        await _services.GetRequiredService<ScratchPad>().Run(cancellationToken);

      if (options.CreateBaseDocument)
        await _services.GetRequiredService<CreateBaseDocument>().Run(cancellationToken);

      if (options.CreateFinalDocument)
        await _services.GetRequiredService<CreateFinalDocument>().Run(cancellationToken);

      if (options.CreateEnumMap)
        await _services.GetRequiredService<CreateEnumMap>().Run(cancellationToken);

      if (options.CreateEnumOverrides)
      {
        await _services.GetRequiredService<CreateEnumOverrides>().Run(cancellationToken);

        if (options.CreateFinalDocument)
          await _services.GetRequiredService<CreateFinalDocument>().Run(cancellationToken);
      }

      if (options.CreateObjectMap)
        await _services.GetRequiredService<CreateObjectMap>().Run(cancellationToken);

      if (options.CreateObjectOverrides)
      {
        await _services.GetRequiredService<CreateObjectOverrides>().Run(cancellationToken);

        if (options.CreateFinalDocument)
          await _services.GetRequiredService<CreateFinalDocument>().Run(cancellationToken);
      }

      if (options.CreateRequestMap)
        await _services.GetRequiredService<CreateRequestMap>().Run(cancellationToken);

      if (options.CreateRequestOverrides)
      {
        await _services.GetRequiredService<CreateRequestOverrides>().Run(cancellationToken);

        if (options.CreateFinalDocument)
          await _services.GetRequiredService<CreateFinalDocument>().Run(cancellationToken);
      }

      if (options.GenerateCode)
        await _services.GetRequiredService<GenerateCode>().Run(cancellationToken);
    }
    catch (Exception ex)
    {
      _logger.LogError(ex, "Unhandled Error");
    }
  }
}
