namespace DeriSock.DevTools.ApiDoc.Analysis;

using DeriSock.DevTools.ApiDoc.Model;

public class ApiDocFunctionWithoutChildrenAnalyzer : ApiDocBaseAnalyzer
{
  public override void VisitFunction(ApiDocFunction function)
  {
    if (function.Request is {Properties.Count: > 0})
      return;

    if(function.Response is {Properties.Count: > 0})
      return;

    if (!string.IsNullOrEmpty(function.Response?.DataType))
      return;

    AddEntry(ApiDocAnalyzeResultType.FunctionWithoutChildren, "Missing child elements on function", function);
  }
}
