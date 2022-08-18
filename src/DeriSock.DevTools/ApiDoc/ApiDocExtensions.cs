namespace DeriSock.DevTools.ApiDoc;

using System;
using System.Text;

using DeriSock.DevTools.ApiDoc.Model;

public static class ApiDocExtensions
{
  public static string GetPath(this IApiDocPropertyNode node)
  {
    var sb = new StringBuilder(100);

    var first = true;

    for (var item = node; item != null; item = item.Parent) {
      if (!first)
        sb.Insert(0, ".");

      first = false;
      sb.Insert(0, item.Name);
    }

    return sb.ToString();
  }

  public static ApiDocFunction? GetRootParent(this IApiDocPropertyNode node)
  {
    while (node.Parent != null)
      node = node.Parent;

    return node as ApiDocFunction;
  }

  public static (bool isMethod, string[] pathParts) ToApiDocParts(this string path)
  {
    var isMethod = path.Contains('/');

    var reqResIndex = path.IndexOf(".request", StringComparison.OrdinalIgnoreCase);

    if (reqResIndex < 0)
      reqResIndex = path.IndexOf(".response", StringComparison.OrdinalIgnoreCase);

    if (reqResIndex < 0)
      return (isMethod, new[] { path });

    string[] pathParts;

    if (isMethod) {
      var methodName = path[..reqResIndex];
      var methodPathParts = path[(reqResIndex + 1)..].Split('.');
      pathParts = new string[methodPathParts.Length + 1];
      pathParts[0] = path[..reqResIndex];
      methodPathParts.CopyTo(pathParts, 1);
      return (isMethod, pathParts);
    }

    var subscriptionName = path[..reqResIndex];
    var subscriptionPathParts = path[(reqResIndex + 1)..].Split('.');

    pathParts = new string[subscriptionPathParts.Length + 1];
    pathParts[0] = subscriptionName;
    subscriptionPathParts.CopyTo(pathParts, 1);

    return (isMethod, pathParts);
  }
}
