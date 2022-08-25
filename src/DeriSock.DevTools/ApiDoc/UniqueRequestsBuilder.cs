namespace DeriSock.DevTools.ApiDoc;

using System.Collections.Generic;

using DeriSock.DevTools.ApiDoc.Model;

public class UniqueRequestsBuilder : IApiDocDocumentVisitor
{
  private readonly ApiDocDocument _apiDoc;

  public Dictionary<string, ApiDocProperty> UniqueProperties { get; } = new();
  public Dictionary<string, List<ApiDocProperty>> PropertiesPerHash { get; } = new();

  public UniqueRequestsBuilder(ApiDocDocument apiDoc)
  {
    _apiDoc = apiDoc;
  }

  public void Build()
  {
    _apiDoc.Accept(this);
  }

  public void VisitDocument(ApiDocDocument document) { }

  public void VisitFunction(ApiDocFunction function)
  {
    if (function.Request is null)
      return;

    var property = function.Request;
    var propertyHash = property.ComputePropertyIdHash();

    if (UniqueProperties.TryAdd(propertyHash, property))
      PropertiesPerHash.Add(propertyHash, new List<ApiDocProperty>(5));

    PropertiesPerHash[propertyHash].Add(property);
  }

  public void VisitProperty(ApiDocProperty property) { }
}
