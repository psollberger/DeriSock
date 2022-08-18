namespace DeriSock.DevTools;

using System;
using System.Collections.Generic;

using DeriSock.DevTools.ApiDoc.Model;

public class FilterPropertiesVisitor : IApiDocDocumentVisitor
{
  private readonly Func<ApiDocProperty, bool> _predicate;

  public List<ApiDocProperty> Properties { get; } = new();

  public FilterPropertiesVisitor(Func<ApiDocProperty, bool> predicate)
  {
    _predicate = predicate;
  }

  public void VisitDocument(ApiDocDocument document) { }

  public void VisitFunction(ApiDocFunction function) { }

  public void VisitProperty(ApiDocProperty property)
  {
    if (_predicate(property))
      Properties.Add(property);
  }
}
