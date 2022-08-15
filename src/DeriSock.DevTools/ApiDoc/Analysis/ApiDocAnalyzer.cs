namespace DeriSock.DevTools.ApiDoc.Analysis;

using System.Linq;

using DeriSock.DevTools.ApiDoc.Model;

public class ApiDocAnalyzer
{
  private readonly ApiDocDocument _apiDoc;

  public ApiDocAnalyzer(ApiDocDocument apiDoc)
  {
    _apiDoc = apiDoc;
  }

  public ApiDocAnalyzeResult Analyze()
  {
    var analyzers = new ApiDocBaseAnalyzer[]
    {
      new ApiDocFunctionWithoutChildrenAnalyzer(),
      new ApiDocObjectWithoutManagedTypeAnalyzer(),
      new ApiDocArrayWithoutArrayTypeAnalyzer()
    };

    foreach (var analyzer in analyzers)
      _apiDoc.Accept(analyzer);

    var result = new ApiDocAnalyzeResult();

    foreach (var resultEntries in analyzers.Where(x => x.Result is { Count: > 0 }).Select(x => x.Result))
      result.Entries.AddRange(resultEntries);

    return result;
  }
}
