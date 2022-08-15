namespace DeriSock.DevTools.ApiDoc.Analysis;

using System.Collections.Generic;

using DeriSock.DevTools.ApiDoc.Model;

public record ApiDocAnalyzeResultEntry
{
  public ApiDocAnalyzeResultType Type { get; set; } = ApiDocAnalyzeResultType.Undefined;
  public string Message { get; set; } = string.Empty;
  public LinkedList<IApiDocPropertyNode> ItemTree { get; set; } = new();
};
