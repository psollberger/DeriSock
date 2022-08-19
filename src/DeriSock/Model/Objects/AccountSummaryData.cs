namespace DeriSock.Model;

using Newtonsoft.Json;

// begin-snippet: example-customized-model-class
public partial class AccountSummaryData
{
  /// <summary>
  ///   The sum of position deltas + INCLUDING your collateral.
  /// </summary>
  [JsonIgnore]
  public decimal DeltaTotalWithCollateral => DeltaTotal + Equity;
}
// end-snippet
