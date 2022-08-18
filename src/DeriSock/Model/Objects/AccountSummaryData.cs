namespace DeriSock.Model;

using Newtonsoft.Json;

public partial class AccountSummaryData
{
  /// <summary>
  ///   The sum of position deltas + INCLUDING your collateral.
  /// </summary>
  [JsonIgnore]
  public decimal DeltaTotalWithCollateral => DeltaTotal + Equity;
}
