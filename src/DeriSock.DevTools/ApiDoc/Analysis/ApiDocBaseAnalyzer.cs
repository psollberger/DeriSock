namespace DeriSock.DevTools.ApiDoc.Analysis;

using System.Collections.Generic;

using DeriSock.DevTools.ApiDoc.Model;

public abstract class ApiDocBaseAnalyzer : IApiDocDocumentVisitor
{
  public IList<ApiDocAnalyzeResultEntry> Result { get; } = new List<ApiDocAnalyzeResultEntry>();

  protected void AddEntry(ApiDocAnalyzeResultType type, string message, IApiDocPropertyNode node)
  {
    var entry = new ApiDocAnalyzeResultEntry
    {
      Type = type,
      Message = message
    };

    var ll = new LinkedList<IApiDocPropertyNode>();

    for (var curNode = node; curNode != null; curNode = curNode.Parent)
      ll.AddFirst(curNode);

    entry.ItemTree = ll;
    Result.Add(entry);
  }

  public virtual void VisitDocument(ApiDocDocument document) { }

  public virtual void VisitFunction(ApiDocFunction function) { }

  public virtual void VisitProperty(ApiDocProperty property) { }
}
