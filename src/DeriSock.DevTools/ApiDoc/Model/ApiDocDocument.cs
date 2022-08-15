namespace DeriSock.DevTools.ApiDoc.Model;

using System.Text.Json.Serialization;

using JetBrains.Annotations;

public class ApiDocDocument
{
  [JsonPropertyName("version")]
  public string Version { get; set; }

  [JsonPropertyName("endpoints")]
  public ApiDocEndpoints Endpoints { get; set; } = new();

  [JsonPropertyName("methods")]
  public ApiDocFunctionCollection Methods { get; set; }

  [JsonPropertyName("subscriptions")]
  public ApiDocFunctionCollection Subscriptions { get; set; }

  [UsedImplicitly]
  public ApiDocDocument()
  {
    Version = string.Empty;
    Methods = new ApiDocFunctionCollection();
    Subscriptions = new ApiDocFunctionCollection();
  }

  public ApiDocDocument(string version, ApiDocFunctionCollection methods, ApiDocFunctionCollection subscriptions)
  {
    Version = version;
    Methods = methods;
    Subscriptions = subscriptions;
  }

  public void Accept(IApiDocDocumentVisitor visitor)
  {
    visitor.VisitDocument(this);

    foreach (var (_, value) in Methods)
      value.Accept(visitor);

    foreach (var (_, value) in Subscriptions)
      value.Accept(visitor);
  }

  internal void UpdateParent()
  {
    foreach (var (key, value) in Methods) {
      value.Name = key;
      value.FunctionType = ApiDocFunctionType.Method;
      value.UpdateParent(null!);
    }

    foreach (var (key, value) in Subscriptions) {
      value.Name = key;
      value.FunctionType = ApiDocFunctionType.Subscription;
      value.UpdateParent(null!);
    }
  }

  public ApiDocProperty? GetPropertyFromPath(string path)
  {
    var (isMethod, pathParts) = path.ToApiDocParts();

    var functionCollection = isMethod ? Methods : Subscriptions;

    if (!functionCollection.TryGetValue(pathParts[0], out var function))
      return null;

    var curProp = pathParts[1] switch
    {
      "request" => function.Request!,
      _         => function.Response!
    };

    if (pathParts.Length < 3)
      return curProp;

    for (var i = 2; i < pathParts.Length; ++i) {
      var nodeName = pathParts[i];

      if (!(curProp.Properties?.TryGetValue(nodeName, out var childProp) ?? false))
        return null;

      curProp = childProp;
    }

    return curProp;
  }
}
