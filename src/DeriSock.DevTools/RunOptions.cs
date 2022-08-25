namespace DeriSock.DevTools;

using CommandLine;

public class RunOptions
{
  [Option(
    "scratchpad",
    Default = false,
    HelpText = "Runs some temporary code to play around with.",
    Hidden = true)]
  public bool ScratchPad { get; set; }

  [Option(
    "create-base",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.base.json' file.")]
  public bool CreateBaseDocument { get; set; }

  [Option(
    "create-final",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.json' file.")]
  public bool CreateFinalDocument { get; set; }

  [Option(
    "create-enum-map",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.enum-map.json' file.")]
  public bool CreateEnumMap { get; set; }

  [Option(
    "create-enum-overrides",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.30.enum-types.overrides.json' file.")]
  public bool CreateEnumOverrides { get; set; }

  [Option(
    "create-object-map",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.object-map.json' file.")]
  public bool CreateObjectMap { get; set; }

  [Option(
    "create-object-overrides",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.31.object-types.overrides.json' file.")]
  public bool CreateObjectOverrides { get; set; }

  [Option(
    "create-request-map",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.request-map.json' file.")]
  public bool CreateRequestMap { get; set; }

  [Option(
    "create-request-overrides",
    Default = false,
    HelpText = "Creates the 'deribit.api.vXXX.32.request-types.overrides.json' file.")]
  public bool CreateRequestOverrides { get; set; }

  [Option(
    "generate-code",
    Default = false,
    HelpText = "Generates code files in Deribit project folder.")]
  public bool GenerateCode { get; set; }
}
