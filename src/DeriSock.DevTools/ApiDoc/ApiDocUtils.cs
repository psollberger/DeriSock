namespace DeriSock.DevTools.ApiDoc;

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using DeriSock.DevTools.ApiDoc.Analysis;
using DeriSock.DevTools.ApiDoc.CodeGeneration;
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

  private static async Task WriteJsonObjectToFile(string path, object obj, CancellationToken cancellationToken)
  {
    await using var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite);

    await JsonSerializer.SerializeAsync(
      fileStream,
      obj,
      SerializerOptions,
      cancellationToken).ConfigureAwait(false);
  }

  private static async Task<T?> ReadJsonObjectFromFile<T>(string path, CancellationToken cancellationToken)
  {
    var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

    await using (fileStream.ConfigureAwait(false))
      return await JsonSerializer.DeserializeAsync<T>(fileStream, cancellationToken: cancellationToken).ConfigureAwait(false);
  }

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

    var apiDoc = await ReadJsonObjectFromFile<ApiDocDocument>(path, cancellationToken).ConfigureAwait(false);

    if (apiDoc is null)
      throw new Exception("API Doc deserialization failed");

    apiDoc.UpdateRelations();

    return apiDoc;
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
    await WriteJsonObjectToFile(path, apiDoc, cancellationToken).ConfigureAwait(false);
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
      var methods = group.Where(x => x.FunctionType == ApiDocFunctionType.Method).ToArray();

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
      var subscriptions = group.Where(x => x.FunctionType == ApiDocFunctionType.Subscription).ToArray();

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

    var map = await ReadJsonObjectFromFile<ApiDocEnumMap>(path, cancellationToken).ConfigureAwait(false);

    if (map is null)
      throw new Exception("Enum Map deserialization failed");

    return map;
  }

  public static async Task WriteEnumOverridesFromMapAsync(ApiDocEnumMap enumMap, string path, CancellationToken cancellationToken = default)
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

    await WriteJsonObjectToFile(path, overrideDoc, cancellationToken).ConfigureAwait(false);
  }

  public static Task PrintObjectMapTemplateAsync(ApiDocDocument apiDoc, ApiDocObjectMap? existingMap, CancellationToken cancellationToken = default)
  {
    var newMap = new ApiDocObjectMap();

    var builder = new UniqueNodesBuilder(apiDoc);
    builder.Build();

    var unknownCount = 0;

    foreach (var (propertyHash, property) in builder.UniqueProperties) {
      var newEntry = new ApiDocObjectMapEntry();
      newEntry.Hash = propertyHash;

      var existingMapEntry = existingMap?.FirstOrDefault(x => x.Value.Hash == propertyHash);
      var existingEntryValue = existingMapEntry?.Value;

      var typeName = existingMapEntry?.Key ?? $"<UNKNOWN{++unknownCount}>";

      newMap.Add(typeName, newEntry);

      newEntry.Description = existingEntryValue?.Description;

      if (property.Properties is { Count: > 0 }) {
        newEntry.Properties = new ApiDocObjectMapPropertyCollection(property.Properties.Count);

        foreach (var (objectPropertyKey, objectProperty) in property.Properties) {
          var existingValue = existingEntryValue?.Properties?.FirstOrDefault(x => x.Key == objectPropertyKey);

          var newProperty = new ApiDocObjectMapProperty();
          newEntry.Properties.Add(objectPropertyKey, newProperty);

          newProperty.Description = existingValue?.Value.Description;
        }
      }

      var methods = builder.PropertiesPerHash[propertyHash].Where(x => x.FunctionType == ApiDocFunctionType.Method).Select(x => x.GetPath()).ToArray();

      if (methods.Length > 0)
        newEntry.MethodPaths = methods;

      var subscriptions = builder.PropertiesPerHash[propertyHash].Where(x => x.FunctionType == ApiDocFunctionType.Subscription).Select(x => x.GetPath()).ToArray();

      if (subscriptions.Length > 0)
        newEntry.SubscriptionPaths = subscriptions;
    }

    Console.WriteLine(JsonSerializer.Serialize(newMap, SerializerOptions));

    return Task.CompletedTask;
  }

  public static async Task<ApiDocObjectMap> ReadObjectMapAsync(string path, CancellationToken cancellationToken = default)
  {
    if (!File.Exists(path))
      throw new FileNotFoundException(path);

    var map = await ReadJsonObjectFromFile<ApiDocObjectMap>(path, cancellationToken).ConfigureAwait(false);

    if (map is null)
      throw new Exception("Enum Map deserialization failed");

    return map;
  }

  public static async Task WriteObjectOverridesFromMapAsync(ApiDocDocument apiDoc, ApiDocObjectMap map, string path, CancellationToken cancellationToken = default)
  {
    var overrideDoc = new ApiDocOverrideDocument
    {
      Methods = new ApiDocOverrideFunctionCollection(),
      Subscriptions = new ApiDocOverrideFunctionCollection()
    };

    foreach (var (typeName, mapEntry) in map) {
      var methodPaths = mapEntry.MethodPaths ?? Array.Empty<string>();
      var subscriptionPaths = mapEntry.SubscriptionPaths ?? Array.Empty<string>();
      var allPaths = methodPaths.Concat(subscriptionPaths).ToArray();

      foreach (var propertyPath in allPaths) {
        var apiDocProperty = apiDoc.GetPropertyFromPath(propertyPath);

        if (apiDocProperty is null)
          continue;

        var (isMethod, pathParts) = propertyPath.ToApiDocParts();
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

        for (var i = 2; i < pathParts.Length; i++) {
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

        if (!string.IsNullOrEmpty(mapEntry.Description))
          curProp.Description = mapEntry.Description;

        if (apiDocProperty.DataType == "array")
          curProp.ArrayDataType = typeName;
        else
          curProp.DataType = typeName;

        if (mapEntry.Properties is { Count: > 0 })
          foreach (var (key, value) in mapEntry.Properties) {
            if (string.IsNullOrEmpty(value.Description))
              continue;

            curProp.Properties ??= new ApiDocOverridePropertyCollection();

            var newProp = new ApiDocOverrideProperty();
            newProp.Description = value.Description;
            curProp.Properties.Add(key, newProp);
          }
      }
    }

    await WriteJsonObjectToFile(path, overrideDoc, cancellationToken).ConfigureAwait(false);
  }

  /// <summary>
  ///   Generates code for the enumeration classes contained in the enum map, placing them in subfolder Enums
  /// </summary>
  /// <param name="map">The enum map that will be used.</param>
  /// <param name="directoryName">The directory where the resulting file should be written to.</param>
  /// <param name="cancellationToken"></param>
  public static async Task GenerateEnumCodeAsync(ApiDocDocument apiDoc, ApiDocEnumMap map, string directoryName, CancellationToken cancellationToken = default)
  {
    var generator = new ValueEnumerationCodeGenerator
    {
      Namespace = "DeriSock.Model",
      Document = apiDoc,
      EnumMap = map
    };

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, "Enums", $"{s}{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);
  }

  /// <summary>
  ///   Generates code for various object classes, placing them in subfolder Objects
  /// </summary>
  /// <param name="map">The object map that will be used.</param>
  /// <param name="directoryName">The directory where the resulting subfolder will be written to.</param>
  /// <param name="cancellationToken"></param>
  public static async Task GenerateObjectCodeAsync(ApiDocDocument apiDoc, ApiDocObjectMap map, string directoryName, CancellationToken cancellationToken = default)
  {
    var generator = new ValueObjectCodeGenerator
    {
      Namespace = "DeriSock.Model",
      Document = apiDoc,
      ObjectMap = map
    };

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, "Objects", $"{s}{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);
  }

  /// <summary>
  ///   Generates code for all requests and responses, placing them in subfolders Requests and Responses
  /// </summary>
  /// <param name="apiDoc">The API documentation document.</param>
  /// <param name="directoryName">The directory where the resulting subfolders should be written to.</param>
  /// <param name="cancellationToken"></param>
  public static async Task GenerateRequestAndResponseClassesAsync(ApiDocDocument apiDoc, string directoryName, CancellationToken cancellationToken = default)
  {
    // Generating the request classes for methods
    var generator = new PropertyClassCodeGenerator
    {
      Namespace = "DeriSock.Model",
      Type = PropertyClassCodeGenerator.GenType.MethodRequest,
      Document = apiDoc
    };

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, "Requests", $"{s}{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);

    // Generating the response classes for methods
    generator.Type = PropertyClassCodeGenerator.GenType.MethodResponse;

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, "Responses", $"{s}{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);

    // Generating the channel classes for subscriptions
    generator.Type = PropertyClassCodeGenerator.GenType.SubscriptionChannel;

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, "Channels", $"{s}{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);

    // Generating the notification classes for subscriptions
    generator.Type = PropertyClassCodeGenerator.GenType.SubscriptionNotification;

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, "Notifications", $"{s}{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);
  }

  /// <summary>
  ///   Generates the interfaces for the API (e.g. categoris, private, public, ...).
  /// </summary>
  /// <param name="apiDoc">The API documentation document.</param>
  /// <param name="directoryName">The directory in which the resulting files will be written to.</param>
  /// <param name="cancellationToken"></param>
  public static async Task GenerateApiInterfaces(ApiDocDocument apiDoc, string directoryName, CancellationToken cancellationToken = default)
  {
    var generator = new ApiInterfaceCodeGenerator
    {
      Namespace = "DeriSock.Api",
      Document = apiDoc
    };

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, $"I{s.ToPublicCodeName()}Api{generator.FileExtension}"),
        bool b   => Path.Combine(directoryName, $"I{(b ? "Private" : "Public")}Api{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    generator.Type = ApiInterfaceCodeGenerator.GenType.Categories;
    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);

    generator.Type = ApiInterfaceCodeGenerator.GenType.Scopes;
    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);

    generator.Type = ApiInterfaceCodeGenerator.GenType.Summary;
    await generator.GenerateToAsync(Path.Combine(directoryName, $"ICategoriesApi{generator.FileExtension}"), cancellationToken).ConfigureAwait(false);
  }

  /// <summary>
  ///   Generates the interface implementations for the API
  /// </summary>
  /// <param name="apiDoc">The API documentation document.</param>
  /// <param name="directoryName">The directory in which the resulting files will be written to.</param>
  /// <param name="cancellationToken"></param>
  public static async Task GenerateApiImplementations(ApiDocDocument apiDoc, string directoryName, CancellationToken cancellationToken = default)
  {
    var generator = new ApiInterfaceImplementationCodeGenerator
    {
      Namespace = "DeriSock",
      Document = apiDoc
    };

    generator.DefinePathCallback = item =>
    {
      return item switch
      {
        string s => Path.Combine(directoryName, $"{s.ToPublicCodeName()}ApiImpl{generator.FileExtension}"),
        bool b   => Path.Combine(directoryName, $"{(b ? "Private" : "Public")}ApiImpl{generator.FileExtension}"),
        _        => string.Empty
      };
    };

    generator.Type = ApiInterfaceImplementationCodeGenerator.GenType.Categories;
    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);

    generator.Type = ApiInterfaceImplementationCodeGenerator.GenType.Scopes;
    await generator.GenerateToCustomAsync(cancellationToken).ConfigureAwait(false);

    generator.Type = ApiInterfaceImplementationCodeGenerator.GenType.Summary;
    await generator.GenerateToAsync(Path.Combine(directoryName, $"ICategoriesApiImpl{generator.FileExtension}"), cancellationToken).ConfigureAwait(false);
  }
}
