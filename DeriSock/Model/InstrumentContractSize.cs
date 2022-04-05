namespace DeriSock.Model;

using Newtonsoft.Json;

public class InstrumentContractSize
{
  [JsonProperty("contract_size")]
  public int ContractSize { get; set; }
}
