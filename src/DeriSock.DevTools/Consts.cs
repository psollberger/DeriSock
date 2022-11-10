namespace DeriSock.DevTools;

internal static class Consts
{
  public const string DocumentationUrl = "https://docs.deribit.com";
#if TEST
  public const string DeriSockGeneratedModelsDirectory = @"D:\Temp\CodeGen\Model";
  public const string DeriSockGeneratedApisDirectory = @"D:\Temp\CodeGen\Api";
#else
  public const string DeriSockGeneratedModelsDirectory = @"..\..\..\src\DeriSock\Model";
  public const string DeriSockGeneratedApisDirectory = @"..\..\..\src\DeriSock\Api";
#endif
  public const string BaseDocumentPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.base.json";
  public const string FinalDocumentPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.json";
  public const string EnumMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.enum-map.json";
  public const string EnumOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.30.enum-types.overrides.json";
  public const string ObjectMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.object-map.json";
  public const string ObjectOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.31.object-types.overrides.json";
  public const string RequestMapPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.request-map.json";
  public const string RequestOverridesPath = @"..\..\..\src\DeriSock.DevTools\Data\deribit.api.v211.32.request-types.overrides.json";
}
