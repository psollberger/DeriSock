namespace DeriSock.DevTools.ApiDoc.Analysis;

using System.Collections.Generic;
using System.Linq;

public class ApiDocAnalyzeResult
{
  public List<ApiDocAnalyzeResultEntry> Entries { get; set; } = new();

  public bool HasType(ApiDocAnalyzeResultType type)
    => Entries.Any(e => e.Type == type);

  public IEnumerable<ApiDocAnalyzeResultEntry> GetEntries(ApiDocAnalyzeResultType type)
    => Entries.Where(e => e.Type == type);

  public IEnumerable<IGrouping<ApiDocAnalyzeResultType, ApiDocAnalyzeResultEntry>> GetEntriesGroupedByType()
    => Entries.GroupBy(e => e.Type);
}
