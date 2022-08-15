namespace DeriSock.DevTools.ApiDoc;

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Analysis;
using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.DevTools.ApiDoc.Model.Override;
using DeriSock.DevTools.CodeDom;

using HtmlAgilityPack;

public static class ApiDocUtils
{
  private static readonly JsonSerializerOptions SerializerOptions = new()
  {
    WriteIndented = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
  };

  public static async Task<ApiDocDocument> BuildApiDocumentAsync(string url, CancellationToken cancellationToken = default)
  {
    var htmlWeb = new HtmlWeb();
    var htmlDoc = await htmlWeb.LoadFromWebAsync(url, cancellationToken).ConfigureAwait(false);
    return ApiDocHtmlParser.CreateDocument(htmlDoc);
  }

  public static async Task<ApiDocDocument> ReadApiDocumentAsync(string path, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(path))
      throw new FileNotFoundException(path);

    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    await using (fileStream.ConfigureAwait(false)) {
      var apiDoc = await JsonSerializer.DeserializeAsync<ApiDocDocument>(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);

      if (apiDoc is null)
        throw new Exception("API Doc deserialization failed");

      apiDoc.UpdateParent();

      return apiDoc;
    }
  }

  public static Task AnalyzeApiDocumentAsync(ApiDocDocument apiDoc, CancellationToken cancellationToken = default)
  {
    var analyzer = new ApiDocAnalyzer(apiDoc);
    var analyzeResult = analyzer.Analyze();

    var groupedEntries = analyzeResult.GetEntriesGroupedByType();

    foreach (var groupEntry in groupedEntries) {
      Console.WriteLine(new string('-', 75));
      Console.Write("--  ");
      Console.WriteLine(groupEntry.Key);
      Console.WriteLine(new string('-', 75));
      Console.WriteLine();

      var entryCount = 0;

      foreach (var entry in groupEntry) {
        entryCount++;

        foreach (var treeItem in entry.ItemTree) {
          if (treeItem.Parent != null)
            Console.Write(".");

          Console.Write(treeItem.Name);
        }

        Console.WriteLine($" : {entry.Message}");
      }

      Console.WriteLine();
      Console.WriteLine($"-- Count: {entryCount}");
      Console.WriteLine();
    }

    return Task.CompletedTask;
  }

  public static async Task ApplyOverridesAsync(ApiDocDocument apiDoc, string path, CancellationToken cancellationToken = default)
  {
    await ApiDocOverrideAdopter.AdoptAsync(apiDoc, path, cancellationToken);
  }

  public static async Task WriteApiDocumentAsync(ApiDocDocument apiDoc, string path, CancellationToken cancellationToken = default)
  {
    await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await JsonSerializer.SerializeAsync(
      fileStream,
      apiDoc,
      SerializerOptions,
      cancellationToken).ConfigureAwait(false);
  }


  public static Task PrintEnumMapTemplateAsync(ApiDocDocument apiDoc, ApiDocEnumMap? existingMap, CancellationToken cancellationToken = default)
  {
    var filteredProperties = new FilterPropertiesVisitor(x => x.EnumValues != null);
    apiDoc.Accept(filteredProperties);

    var groupedEntries = filteredProperties.Properties.GroupBy(x => string.Join(", ", x.EnumValues!)).ToArray();

    var sb = new StringBuilder();
    var itw = new IndentedTextWriter(new StringWriter(sb), "  ");
    itw.WriteLine("{");
    itw.Indent++;

    var lastGroup = groupedEntries[^1];

    foreach (var group in groupedEntries) {
      var firstGroupEntry = group.First();
      var enumValues = firstGroupEntry.EnumValues!;
      var enumDescription = firstGroupEntry.Description ?? string.Empty;

      var existingMapEntry = existingMap?.FirstOrDefault(x => x.Value.EnumValues.SequenceEqual(enumValues));

      itw.WriteLine($"\"{existingMapEntry?.Key}\": {{");
      itw.Indent++;

      itw.WriteLine($"\"description\": \"{JsonEncodedText.Encode(enumDescription)}\",");

      itw.WriteLine("\"enumValues\": [");
      itw.Indent++;

      var lastEnumValue = enumValues[^1];

      foreach (var enumValue in enumValues) {
        var isLast = ReferenceEquals(enumValue, lastEnumValue);
        itw.WriteLine($"\"{enumValue}\"{(isLast ? string.Empty : ",")}");
      }

      itw.Indent--;
      itw.WriteLine("],");

      itw.WriteLine("\"methods\": [");
      itw.Indent++;
      var methods = group.Where(x => x.GetRootParent()!.FunctionType == ApiDocFunctionType.Method).ToArray();

      if (methods.Length > 0) {
        var lastEntry = methods[^1];

        foreach (var property in methods) {
          var isLast = ReferenceEquals(property, lastEntry);
          itw.WriteLine($"\"{property.GetPath()}\"{(isLast ? string.Empty : ",")}");
        }
      }

      itw.Indent--;
      itw.WriteLine("],");

      itw.WriteLine("\"subscriptions\": [");
      itw.Indent++;
      var subscriptions = group.Where(x => x.GetRootParent()!.FunctionType == ApiDocFunctionType.Subscription).ToArray();

      if (subscriptions.Length > 0) {
        var lastEntry = subscriptions[^1];

        foreach (var property in subscriptions) {
          var isLast = ReferenceEquals(property, lastEntry);
          itw.WriteLine($"\"{property.GetPath()}\"{(isLast ? string.Empty : ",")}");
        }
      }

      itw.Indent--;
      itw.WriteLine("]");

      itw.Indent--;

      var isLastGroup = ReferenceEquals(group, lastGroup);
      itw.WriteLine($"}}{(isLastGroup ? string.Empty : ",")}");
    }

    itw.Indent--;
    itw.WriteLine("}");

    Console.WriteLine(sb.ToString());

    return Task.CompletedTask;
  }

  public static async Task<ApiDocEnumMap> ReadEnumMapAsync(string path, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(path))
      throw new FileNotFoundException(path);

    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    await using (fileStream.ConfigureAwait(false)) {
      var map = await JsonSerializer.DeserializeAsync<ApiDocEnumMap>(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);

      if (map is null)
        throw new Exception("Enum Map deserialization failed");

      return map;
    }
  }

  public static Task PrintEnumOverridesFromMapAsync(ApiDocEnumMap enumMap, CancellationToken cancellationToken = default)
  {
    var overrideDoc = new ApiDocOverrideDocument();

    foreach (var (typeName, mapEntry) in enumMap.Where(x => x.Value.Methods.Any())) {
      overrideDoc.Methods ??= new ApiDocOverrideFunctionCollection();

      foreach (var methodPath in mapEntry.Methods) {
        var pathParts = methodPath.Split('.');

        var methodName = pathParts[0];
        var reqOrResValue = pathParts[1];

        if (!overrideDoc.Methods.TryGetValue(methodName, out var thisMethod)) {
          thisMethod = new ApiDocOverrideFunction();
          overrideDoc.Methods.Add(methodName, thisMethod);
        }

        if (reqOrResValue == "request")
          thisMethod.Request ??= new ApiDocOverrideProperty();
        else
          thisMethod.Response ??= new ApiDocOverrideProperty();

        var curProp = reqOrResValue switch
        {
          "request" => thisMethod.Request!,
          _         => thisMethod.Response!
        };

        for (var i = 2; i < pathParts.Length; ++i) {
          var nodeName = pathParts[i];

          curProp.Properties ??= new ApiDocOverridePropertyCollection();

          if (curProp.Properties.TryGetValue(nodeName, out var childProp)) {
            curProp = childProp;
            continue;
          }

          var childProps = curProp.Properties;

          curProp = new ApiDocOverrideProperty();
          childProps.Add(nodeName, curProp);
        }

        curProp.DataType = typeName;
      }
    }

    foreach (var (typeName, mapEntry) in enumMap.Where(x => x.Value.Subscriptions.Any())) {
      overrideDoc.Subscriptions ??= new ApiDocOverrideFunctionCollection();

      foreach (var subscriptionPath in mapEntry.Subscriptions) {
        var idxNameSplit = subscriptionPath.LastIndexOf('}');

        if (idxNameSplit == -1)
          idxNameSplit = subscriptionPath.IndexOf('.');

        var subscriptionName = subscriptionPath.Substring(0, idxNameSplit + 1);
        var pathParts = subscriptionPath.Substring(idxNameSplit + 2).Split('.');

        var reqOrResValue = pathParts[0];

        if (!overrideDoc.Subscriptions.TryGetValue(subscriptionName, out var thisSubscription)) {
          thisSubscription = new ApiDocOverrideFunction();
          overrideDoc.Subscriptions.Add(subscriptionName, thisSubscription);
        }

        if (reqOrResValue == "request")
          thisSubscription.Request ??= new ApiDocOverrideProperty();
        else
          thisSubscription.Response ??= new ApiDocOverrideProperty();

        var curProp = reqOrResValue switch
        {
          "request" => thisSubscription.Request!,
          _         => thisSubscription.Response!
        };

        for (var i = 1; i < pathParts.Length; ++i) {
          var nodeName = pathParts[i];

          curProp.Properties ??= new ApiDocOverridePropertyCollection();

          if (curProp.Properties.TryGetValue(nodeName, out var childProp)) {
            curProp = childProp;
            continue;
          }

          var childProps = curProp.Properties;

          curProp = new ApiDocOverrideProperty();
          childProps.Add(nodeName, curProp);
        }

        curProp.DataType = typeName;
      }
    }

    var jsonString = JsonSerializer.Serialize(overrideDoc, SerializerOptions);
    Console.WriteLine(jsonString);

    return Task.CompletedTask;
  }

  /// <summary>
  ///   Generates code for the enumeration classes contained in the enum map.
  /// </summary>
  /// <param name="map">The enum map that will be used.</param>
  /// <param name="directoryName">The directory where the resulting file should be written to.</param>
  /// <param name="fileNameWithoutExtension">The file name without extension where all classes are written to.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The extension used for the filenames.</returns>
  public static async Task<string> GenerateEnumCodeAsync(ApiDocEnumMap map, string directoryName, string fileNameWithoutExtension, CancellationToken cancellationToken = default)
  {
    var apiCodeProvider = new ApiDocCodeProvider();
    var path = Path.Combine(directoryName, $"{fileNameWithoutExtension}{apiCodeProvider.FileExtension}");

    foreach (var (typeName, mapEntry) in map)
      apiCodeProvider.AddEnumValueClass(typeName, mapEntry);

    var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await using (fs.ConfigureAwait(false)) {
      await fs.WriteAsync(Encoding.UTF8.GetBytes(await apiCodeProvider.GenerateAsync()), cancellationToken).ConfigureAwait(false);
      await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    return apiCodeProvider.FileExtension;
  }


  public static Task PrintObjectMapTemplateAsync(ApiDocDocument apiDoc, ApiDocObjectMap? existingMap, CancellationToken cancellationToken = default)
  {
    var filteredProperties = new FilterPropertiesVisitor(x => x.Name is not "request" or "response" && (x.DataType == "object" || x.ArrayDataType == "object"));
    apiDoc.Accept(filteredProperties);

    var groupedEntries = (from p in filteredProperties.Properties
                          group p by p.GetPropertyList()
                          into gp
                          orderby gp.Key
                          select gp).ToArray();

    var sb = new StringBuilder();
    var itw = new IndentedTextWriter(new StringWriter(sb), "  ");
    itw.WriteLine("{");
    itw.Indent++;

    var lastGroup = groupedEntries[^1];

    foreach (var group in groupedEntries) {
      var firstGroupEntry = group.First();

      if (firstGroupEntry.Properties == null)
        continue;

      var entryValues = firstGroupEntry.Properties!;
      var entryDescription = firstGroupEntry.Description;

      var existingMapEntry = existingMap?.FirstOrDefault(x => x.Value.Properties.SequenceEqual(entryValues));
      var existingEntryKey = existingMapEntry?.Key ?? string.Empty;
      var existingEntryValue = existingMapEntry?.Value;

      if (string.IsNullOrEmpty(entryDescription))
        entryDescription = existingEntryValue?.Description;

      itw.WriteLine($"\"{existingEntryKey}\": {{");
      itw.Indent++;

      itw.WriteLine($"\"description\": \"{JsonEncodedText.Encode(entryDescription ?? string.Empty)}\",");

      itw.WriteLine("\"properties\": {");
      itw.Indent++;

      var lastValueEntry = entryValues.Last().Value;

      foreach (var (key, value) in entryValues) {
        var isLast = ReferenceEquals(value, lastValueEntry);

        var existingValue = existingEntryValue?.Properties.FirstOrDefault(x => x.Value.Name == value.Name);

        itw.WriteLine($"\"{key}\": {{");
        itw.Indent++;

        var description = existingValue?.Value.Description;

        if (string.IsNullOrWhiteSpace(description))
          description = value.Description;

        var dataType = existingValue?.Value.DataType;

        if (string.IsNullOrEmpty(dataType))
          dataType = value.DataType;

        var arrayDataType = existingValue?.Value.ArrayDataType;

        if (string.IsNullOrEmpty(arrayDataType))
          arrayDataType = value.ArrayDataType;

        itw.WriteLine($"\"name\": \"{value.Name}\",");
        itw.WriteLine($"\"description\": \"{JsonEncodedText.Encode(description ?? string.Empty)}\",");
        itw.WriteLine($"\"required\": {value.Required.ToString().ToLower()},");
        itw.WriteLine($"\"apiDataType\": \"{value.ApiDataType}\",");
        itw.WriteLine($"\"dataType\": \"{dataType}\",");
        itw.Write($"\"arrayDataType\": \"{arrayDataType}\"");

        if (value is { Converters.Length: > 0 }) {
          itw.WriteLine(",");
          itw.WriteLine("\"converters\": [");
          itw.Indent++;
          var lastConverter = value.Converters[^1];

          foreach (var converter in value.Converters) {
            var isLastConverter = ReferenceEquals(converter, lastConverter);
            itw.WriteLine($"\"{converter}\"{(isLastConverter ? string.Empty : ",")}");
          }

          itw.Indent--;
          itw.Write("]");
        }

        itw.WriteLine();

        itw.Indent--;
        itw.WriteLine($"}}{(isLast ? string.Empty : ",")}");
      }

      itw.Indent--;
      itw.WriteLine("},");

      itw.WriteLine("\"methods\": [");
      itw.Indent++;
      var methods = group.Where(x => x.GetRootParent()!.FunctionType == ApiDocFunctionType.Method).ToArray();

      if (methods.Length > 0) {
        var lastEntry = methods[^1];

        foreach (var property in methods) {
          var isLast = ReferenceEquals(property, lastEntry);
          itw.WriteLine($"\"{property.GetPath()}\"{(isLast ? string.Empty : ",")}");
        }
      }

      itw.Indent--;
      itw.WriteLine("],");

      itw.WriteLine("\"subscriptions\": [");
      itw.Indent++;
      var subscriptions = group.Where(x => x.GetRootParent()!.FunctionType == ApiDocFunctionType.Subscription).ToArray();

      if (subscriptions.Length > 0) {
        var lastEntry = subscriptions[^1];

        foreach (var property in subscriptions) {
          var isLast = ReferenceEquals(property, lastEntry);
          itw.WriteLine($"\"{property.GetPath()}\"{(isLast ? string.Empty : ",")}");
        }
      }

      itw.Indent--;
      itw.WriteLine("]");

      itw.Indent--;

      var isLastGroup = ReferenceEquals(group, lastGroup);
      itw.WriteLine($"}}{(isLastGroup ? string.Empty : ",")}");
    }

    itw.Indent--;
    itw.WriteLine("}");

    Console.WriteLine(sb.ToString());

    return Task.CompletedTask;
  }

  public static async Task<ApiDocObjectMap> ReadObjectMapAsync(string path, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(path))
      throw new FileNotFoundException(path);

    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    await using (fileStream.ConfigureAwait(false)) {
      var map = await JsonSerializer.DeserializeAsync<ApiDocObjectMap>(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);

      if (map is null)
        throw new Exception("Enum Map deserialization failed");

      foreach (var (_, value) in map) {
        foreach (var (propName, propValue) in value.Properties)
          propValue.Name = propName;
      }

      return map;
    }
  }

  public static Task PrintObjectOverridesFromMapAsync(ApiDocDocument apiDoc, ApiDocObjectMap map, CancellationToken cancellationToken = default)
  {
    var overrideDoc = new ApiDocOverrideDocument
    {
      Methods = new ApiDocOverrideFunctionCollection(),
      Subscriptions = new ApiDocOverrideFunctionCollection()
    };

    foreach (var (typeName, mapEntry) in map) {
      foreach (var path in mapEntry.Methods.Concat(mapEntry.Subscriptions)) {
        var apiDocProperty = apiDoc.GetPropertyFromPath(path);

        if (apiDocProperty == null)
          continue;

        var (isMethod, pathParts) = path.ToApiDocParts();
        var functionName = pathParts[0];

        var apiFunctionCollection = isMethod ? apiDoc.Methods : apiDoc.Subscriptions;
        var overrideFunctionCollection = isMethod ? overrideDoc.Methods : overrideDoc.Subscriptions;

        if (!apiFunctionCollection.TryGetValue(functionName, out var _))
          continue;

        if (!overrideFunctionCollection.TryGetValue(functionName, out var overrideFunction)) {
          overrideFunction = new ApiDocOverrideFunction();
          overrideFunctionCollection.Add(functionName, overrideFunction);
        }

        var reqOrResValue = pathParts[1];

        if (reqOrResValue == "request")
          overrideFunction.Request ??= new ApiDocOverrideProperty();
        else
          overrideFunction.Response ??= new ApiDocOverrideProperty();

        var curProp = reqOrResValue switch
        {
          "request" => overrideFunction.Request!,
          _         => overrideFunction.Response!
        };

        for (var i = 2; i < pathParts.Length; ++i) {
          var nodeName = pathParts[i];

          curProp.Properties ??= new ApiDocOverridePropertyCollection();

          if (curProp.Properties.TryGetValue(nodeName, out var childProp)) {
            curProp = childProp;
            continue;
          }

          var childProps = curProp.Properties;

          curProp = new ApiDocOverrideProperty();
          childProps.Add(nodeName, curProp);
        }

        if (apiDocProperty.DataType == "array")
          curProp.ArrayDataType = typeName;
        else
          curProp.DataType = typeName;
      }
    }

    var jsonString = JsonSerializer.Serialize(overrideDoc, SerializerOptions);
    Console.WriteLine(jsonString);

    return Task.CompletedTask;
  }

  /// <summary>
  ///   Generates code for various object classes.
  /// </summary>
  /// <param name="map">The object map that will be used.</param>
  /// <param name="directoryName">The directory where the resulting file should be written to.</param>
  /// <param name="fileNameWithoutExtension">The file name without extension where all classes are written to.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The extension used for the filenames.</returns>
  public static async Task<string> GenerateObjectCodeAsync(ApiDocObjectMap map, string directoryName, string fileNameWithoutExtension, CancellationToken cancellationToken = default)
  {
    var apiCodeProvider = new ApiDocCodeProvider();
    var path = Path.Combine(directoryName, $"{fileNameWithoutExtension}{apiCodeProvider.FileExtension}");

    foreach (var (typeName, mapEntry) in map)
      apiCodeProvider.AddObjectValueClass(typeName, mapEntry);

    var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await using (fs.ConfigureAwait(false)) {
      await fs.WriteAsync(Encoding.UTF8.GetBytes(await apiCodeProvider.GenerateAsync()), cancellationToken).ConfigureAwait(false);
      await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    return apiCodeProvider.FileExtension;
  }


  /// <summary>
  ///   Generates code for all the requests.
  /// </summary>
  /// <param name="apiDoc">The API documentation document.</param>
  /// <param name="directoryName">The directory where the resulting file should be written to.</param>
  /// <param name="fileNameWithoutExtension">The file name without extension, where all classes are written to.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The extension used for the filenames.</returns>
  public static async Task<string> GenerateRequestClassesAsync(ApiDocDocument apiDoc, string directoryName, string fileNameWithoutExtension, CancellationToken cancellationToken = default)
  {
    var apiCodeProvider = new ApiDocCodeProvider();
    var path = Path.Combine(directoryName, $"{fileNameWithoutExtension}{apiCodeProvider.FileExtension}");

    var allFunctions = apiDoc.Methods.Concat(apiDoc.Subscriptions).Select(x => x.Value);

    foreach (var function in allFunctions) {
      if (function.Request == null)
        continue;

      apiCodeProvider.AddFunctionPropertyClass(string.Concat(function.Name.ToPublicCodeName(), "Request"), function.Request);
    }

    var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await using (fs.ConfigureAwait(false)) {
      await fs.WriteAsync(Encoding.UTF8.GetBytes(await apiCodeProvider.GenerateAsync()), cancellationToken).ConfigureAwait(false);
      await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    return apiCodeProvider.FileExtension;
  }


  /// <summary>
  ///   Generates code for all the responses.
  /// </summary>
  /// <param name="apiDoc">The API documentation document.</param>
  /// <param name="directoryName">The directory where the resulting file should be written to.</param>
  /// <param name="fileNameWithoutExtension">The file name without extension, where all classes are written to.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The extension used for the filenames.</returns>
  public static async Task<string> GenerateResponseClassesAsync(ApiDocDocument apiDoc, string directoryName, string fileNameWithoutExtension, CancellationToken cancellationToken = default)
  {
    var apiCodeProvider = new ApiDocCodeProvider();
    var path = Path.Combine(directoryName, $"{fileNameWithoutExtension}{apiCodeProvider.FileExtension}");

    var allFunctions = apiDoc.Methods.Concat(apiDoc.Subscriptions).Select(x => x.Value);

    foreach (var function in allFunctions) {
      if (function.Response == null)
        continue;

      apiCodeProvider.AddFunctionPropertyClass(string.Concat(function.Name.ToPublicCodeName(), "Response"), function.Response);
    }

    var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await using (fs.ConfigureAwait(false)) {
      await fs.WriteAsync(Encoding.UTF8.GetBytes(await apiCodeProvider.GenerateAsync()), cancellationToken).ConfigureAwait(false);
      await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
    }

    return apiCodeProvider.FileExtension;
  }


  /// <summary>
  ///   Generates the interfaces for the API (e.g. categoris, private, public, ...).
  /// </summary>
  /// <param name="apiDoc">The API documentation document.</param>
  /// <param name="directoryName">The directory in which the resulting files will be written to.</param>
  /// <param name="cancellationToken"></param>
  /// <returns>The extension used for the filenames.</returns>
  [SuppressMessage("ReSharper", "PossibleMultipleEnumeration")]
  public static async Task<string> GenerateApiInterfaces(ApiDocDocument apiDoc, string directoryName, CancellationToken cancellationToken = default)
  {
    if (!Directory.Exists(directoryName))
      throw new DirectoryNotFoundException();

    var apiCodeProvider = new ApiDocCodeProvider("DeriSock.Api");

    var functionsPerCategory = apiDoc.Methods.GroupBy(x => x.Value.Category);

    await CodeGenEnumerate(
      apiCodeProvider,
      functionsPerCategory,
      category =>
      {
        Debug.Assert(!string.IsNullOrEmpty(category.Key));

        return Path.Combine(directoryName, $"I{category.Key.ToPublicCodeName()}Api{apiCodeProvider.FileExtension}");
      },
      (code, category) =>
      {
        code.AddPartialInterface($"I{category.Key!.ToPublicCodeName()}Api", category.Select(x => x.Value));
      },
      cancellationToken).ConfigureAwait(false);


    var functionsPerScope = apiDoc.Methods.GroupBy(x => x.Value.IsPrivate);

    await CodeGenEnumerate(
      apiCodeProvider,
      functionsPerScope,
      scope => Path.Combine(directoryName, $"I{(scope.Key ? "Private" : "Public")}Api{apiCodeProvider.FileExtension}"),
      (code, scope) =>
      {
        code.AddPartialInterface($"I{(scope.Key ? "Private" : "Public")}Api", scope.Select(x => x.Value));
      },
      cancellationToken).ConfigureAwait(false);


    var categoryNames = new[] { functionsPerCategory.Select(x => x.Key!) };

    await CodeGenEnumerate(
      apiCodeProvider,
      categoryNames,
      _ => Path.Combine(directoryName, $"ICategoriesApi{apiCodeProvider.FileExtension}"),
      (code, categoryNames) =>
      {
        code.BeginCategoriesInterface("ICategoriesApi");
        code.AddCategoriesInterfaceProperty("Public");
        code.AddCategoriesInterfaceProperty("Private");
        foreach (var categoryName in categoryNames) {          
          code.AddCategoriesInterfaceProperty(categoryName);
        }
        code.EndCategoriesInterface();
      },
      cancellationToken).ConfigureAwait(false);

    return apiCodeProvider.FileExtension;
  }

  private static async Task CodeGenEnumerate<T>(ApiDocCodeProvider apiCodeProvider, IEnumerable<T> list, Func<T, string> definePath, Action<ApiDocCodeProvider, T> generate, CancellationToken cancellationToken)
  {
    foreach (var item in list) {
      var path = definePath(item);

      if (string.IsNullOrEmpty(path))
        continue;

      generate(apiCodeProvider, item);

      var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

      await using (fs.ConfigureAwait(false)) {
        await fs.WriteAsync(Encoding.UTF8.GetBytes(await apiCodeProvider.GenerateAsync()), cancellationToken).ConfigureAwait(false);
        await fs.FlushAsync(cancellationToken).ConfigureAwait(false);
      }

      apiCodeProvider.Clear();
    }
  }
}
