namespace DeriSock.DevTools.ApiDoc.Analysis;

using DeriSock.DevTools.ApiDoc.Model;

public class ApiDocArrayWithoutArrayTypeAnalyzer : ApiDocBaseAnalyzer
{
  public override void VisitProperty(ApiDocProperty property)
  {
    if (property.DataType != "array")
      return;

    if (!string.IsNullOrEmpty(property.ArrayDataType))
      return;

    AddEntry(ApiDocAnalyzeResultType.ArrayWithoutArrayType, "Array without array type", property);
  }
}
