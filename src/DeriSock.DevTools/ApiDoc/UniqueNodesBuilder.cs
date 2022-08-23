namespace DeriSock.DevTools.ApiDoc;

using System;
using System.Collections.Generic;

using DeriSock.DevTools.ApiDoc.Model;

public class UniqueNodesBuilder : IApiDocDocumentVisitor
{
  private readonly ApiDocDocument _apiDoc;

  public Dictionary<string, ApiDocProperty> UniqueProperties { get; } = new();
  public Dictionary<string, List<ApiDocProperty>> PropertiesPerHash { get; } = new();

  public UniqueNodesBuilder(ApiDocDocument apiDoc)
  {
    _apiDoc = apiDoc;
  }

  public void Build()
  {
    _apiDoc.Accept(this);
  }

  public void VisitDocument(ApiDocDocument document) { }

  public void VisitFunction(ApiDocFunction function) { }

  public void VisitProperty(ApiDocProperty property)
  {
    if (!(property.ApiDataType?.Contains("object", StringComparison.OrdinalIgnoreCase) ?? false))
      return;

    var propertyHash = property.ComputePropertyIdHash();

    if (UniqueProperties.TryAdd(propertyHash, property))
      PropertiesPerHash.Add(propertyHash, new List<ApiDocProperty>(5));

    PropertiesPerHash[propertyHash].Add(property);
  }
}
