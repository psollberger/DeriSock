namespace DeriSock.DevTools.ApiDoc;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.ApiDoc.Model.Override;

public class ApiDocOverrideAdopter
{
  private readonly ApiDocDocument _apiDoc;
  private ApiDocOverrideDocument? _apiOverrideDoc;

  public ApiDocOverrideAdopter(ApiDocDocument apiDoc)
  {
    _apiDoc = apiDoc;
  }

  public static async Task AdoptAsync(ApiDocDocument apiDoc, string path, CancellationToken cancellationToken = default)
  {
    var directoryPath = Path.GetDirectoryName(Path.GetFullPath(path));

    if (string.IsNullOrEmpty(directoryPath))
      return;

    var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(path);

    var overrideFileNamePattern = new Regex(@$"^{Regex.Escape(fileNameWithoutExtension)}(\.(?!\.)[0-9]+\.(?!\.)[\w_-]+)?\.overrides\.json$", RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

    var jsonFilesInFolder = Directory.GetFiles(directoryPath, "*.json").Select(Path.GetFileName);

    var overrideFileNames = jsonFilesInFolder
      .Where(fileName => !string.IsNullOrEmpty(fileName) && overrideFileNamePattern.IsMatch(fileName))
      .Select(fileName => Path.Combine(directoryPath, fileName!)).ToArray();


    var adopter = new ApiDocOverrideAdopter(apiDoc);

    foreach (var overrideFileName in overrideFileNames)
      await adopter.AdoptFromFileAsync(overrideFileName, cancellationToken).ConfigureAwait(false);
  }

  public async Task AdoptFromFileAsync(string overrideFilePath, CancellationToken cancellationToken = default)
  {
    await using var fileStream = new FileStream(overrideFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    _apiOverrideDoc = await JsonSerializer.DeserializeAsync<ApiDocOverrideDocument>(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);

    if (_apiOverrideDoc == null)
      throw new FileNotFoundException("Can not adopt overrides.", overrideFilePath);

    if (_apiOverrideDoc.Methods is not null)
      foreach (var (key, value) in _apiOverrideDoc.Methods)
        AdoptFunctionOverrides(key, value, _apiDoc.Methods);

    if (_apiOverrideDoc.Subscriptions is not null)
      foreach (var (key, value) in _apiOverrideDoc.Subscriptions)
        AdoptFunctionOverrides(key, value, _apiDoc.Subscriptions);
  }

  private static void AdoptFunctionOverrides(string overrideFunctionKey, ApiDocOverrideFunction overrideFunction, ApiDocFunctionCollection functionCollection)
  {
    var functionsToOverride = overrideFunctionKey.IsRegexPattern() ?
                                functionCollection.Where(m => Regex.IsMatch(m.Key, overrideFunctionKey)).ToArray() :
                                functionCollection.Where(m => m.Key.Equals(overrideFunctionKey)).ToArray();

    if (functionsToOverride.Length < 1)
      return;

    foreach (var (_, function) in functionsToOverride) {
      if (overrideFunction.Description is not null)
        function.Description = overrideFunction.Description;

      if (overrideFunction.ExcludeInInterface.HasValue)
        function.ExcludeInInterface = overrideFunction.ExcludeInInterface.Value;

      if (overrideFunction.IsSynchronous.HasValue)
        function.IsSynchronous = overrideFunction.IsSynchronous.Value;

      if (overrideFunction.Deprecated.HasValue)
        function.Deprecated = overrideFunction.Deprecated.Value;

      //if (overrideFunction.Generate.HasValue)
      //  function.Generate = overrideFunction.Generate.Value;

      AdoptPropertyOverrides(function.Request, overrideFunction.Request);
      AdoptPropertyOverrides(function.Response, overrideFunction.Response);
    }
  }

  private static void AdoptPropertyCollectionOverrides(ApiDocProperty? property, ApiDocOverrideProperty? overrideProperty)
  {
    if (property is not { Properties.Count: > 0 } || overrideProperty is not { Properties.Count: > 0 })
      return;

    foreach (var (ovrPropKey, ovrPropValue) in overrideProperty.Properties) {
      var searchKey = ovrPropKey;

      if (ovrPropValue.Name is not null) {
        property.Properties.RenameKey(ovrPropKey, ovrPropValue.Name);
        searchKey = ovrPropValue.Name;
      }

      var propertiesToOverride = searchKey.IsRegexPattern() ?
                                   property.Properties.Where(op => Regex.IsMatch(op.Key, searchKey)).ToArray() :
                                   property.Properties.Where(op => op.Key.Equals(searchKey)).ToArray();

      if (propertiesToOverride.Length < 1)
        continue;

      foreach (var prop in propertiesToOverride)
        AdoptPropertyOverrides(prop.Value, ovrPropValue);
    }
  }

  private static void AdoptPropertyOverrides(ApiDocProperty? property, ApiDocOverrideProperty? overrideProperty)
  {
    if (property is null || overrideProperty is null)
      return;

    if (overrideProperty.Description is not null)
      property.Description = overrideProperty.Description;

    if (overrideProperty.Deprecated.HasValue)
      property.Deprecated = overrideProperty.Deprecated.Value;

    if (overrideProperty.Required.HasValue)
      property.Required = overrideProperty.Required.Value;

    if (overrideProperty.ApiDataType is not null)
      property.ApiDataType = overrideProperty.ApiDataType;

    if (overrideProperty.DataType is not null)
      property.DataType = overrideProperty.DataType;

    if (overrideProperty.ArrayDataType is not null)
      property.ArrayDataType = overrideProperty.ArrayDataType;

    if (overrideProperty is { Converters.Length: > 0 }) {
      var startIdx = 0;
      if (string.IsNullOrEmpty(overrideProperty.Converters[0])) {
        property.Converters = null;
        startIdx++;
      }

      property.Converters = overrideProperty.Converters
        .Skip(startIdx)
        .Concat(property.Converters ?? Array.Empty<string>()).ToArray();
    }

    if (overrideProperty.EnumValues is not null)
      property.EnumValues = overrideProperty.EnumValues;

    if (overrideProperty.EnumIsSuggestion.HasValue)
      property.EnumIsSuggestion = overrideProperty.EnumIsSuggestion;

    if (overrideProperty.ValueLookupSource is not null)
      property.ValueLookupSource = overrideProperty.ValueLookupSource;

    if (overrideProperty.DefaultValue is not null)
      property.DefaultValue = overrideProperty.DefaultValue;

    if (overrideProperty.MaxLength.HasValue)
      property.MaxLength = overrideProperty.MaxLength.Value;

    AdoptPropertyCollectionOverrides(property, overrideProperty);
  }
}
