namespace DeriSock.Model;

using Newtonsoft.Json;

public class CurrencyWithdrawalPriority
{
  [JsonProperty("name")]
  public string Name { get; set; }

  [JsonProperty("value")]
  public decimal Value { get; set; }
}
