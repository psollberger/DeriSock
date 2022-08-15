namespace DeriSock.DevTools.ApiDoc.Analysis;

using DeriSock.DevTools.ApiDoc.Model;

public class ApiDocObjectWithoutManagedTypeAnalyzer : ApiDocBaseAnalyzer
{
  public override void VisitProperty(ApiDocProperty property)
  {
    if (!property.IsObject)
      return;

    AddEntry(ApiDocAnalyzeResultType.PropertyAsObject, "Property with object data type", property);
  }
}
