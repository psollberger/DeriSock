namespace DeriSock.Model
{
  using Newtonsoft.Json;

  public class Greeks
  {
    /// <summary>
    ///   (Only for option) The delta value for the option
    /// </summary>
    [JsonProperty("delta")]
    public decimal Delta { get; set; }

    /// <summary>
    ///   (Only for option) The gamma value for the option
    /// </summary>
    [JsonProperty("gamma")]
    public decimal Gamma { get; set; }

    /// <summary>
    ///   (Only for option) The rho value for the option
    /// </summary>
    [JsonProperty("rho")]
    public decimal Rho { get; set; }

    /// <summary>
    ///   (Only for option) The theta value for the option
    /// </summary>
    [JsonProperty("theta")]
    public decimal Theta { get; set; }

    /// <summary>
    ///   (Only for option) The vega value for the option
    /// </summary>
    [JsonProperty("vega")]
    public decimal Vega { get; set; }
  }
}
