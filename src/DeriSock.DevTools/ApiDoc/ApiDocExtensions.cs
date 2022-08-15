namespace DeriSock.DevTools.ApiDoc;

using System;
using System.Collections.Generic;
using System.Linq;
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

  public static string GetPropertyList(this IApiDocPropertyNode node, bool extended = false)
  {
    if (node.Properties == null)
      return string.Empty;

    if (node.Properties.Count < 1)
      return string.Empty;

    if (extended)
      return string.Join(',', node.Properties.Select(x => $"{x.Value.Name},{(x.Value.Required ? "required" : "optional")},{(x.Value.Deprecated ? "deprecated" : "current")},{x.Value.DataType},{x.Value.ArrayDataType},{string.Join(',', x.Value.EnumValues ?? Array.Empty<string>())},{x.Value.EnumIsSuggestion},{x.Value.DefaultValue},{x.Value.MaxLength ?? 0},{x.Value.Description}"));

    return string.Join(", ", node.Properties.Select(x => $"{x.Value.Name} ({(!string.IsNullOrEmpty(x.Value.ArrayDataType) ? x.Value.ArrayDataType : x.Value.DataType)})"));
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

    string[] pathParts;

    if (isMethod) {
      pathParts = path.Split('.');
    }
    else {
      var idxNameSplit = path.LastIndexOf('}');

      if (idxNameSplit == -1)
        idxNameSplit = path.IndexOf('.');

      var subscriptionName = path.Substring(0, idxNameSplit + 1);
      var subscriptionPathParts = path.Substring(idxNameSplit + 2).Split('.');

      pathParts = new string[subscriptionPathParts.Length + 1];
      pathParts[0] = subscriptionName;
      subscriptionPathParts.CopyTo(pathParts, 1);
    }

    return (isMethod, pathParts);
  }
}
