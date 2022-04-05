namespace DeriSock.Model;

using Newtonsoft.Json;

public class TradeVolume
{
  /// <summary>
  ///   Total 24h trade volume for call options. This is expressed in the base currency, e.g. BTC for btc_usd
  /// </summary>
  [JsonProperty("calls_volume")]
  public decimal CallsVolume { get; set; }

  /// <summary>
  ///   Currency pair: "btc_usd" or "eth_usd"
  /// </summary>
  [JsonProperty("currency_pair")]
  public string CurrencyPair { get; set; }

  /// <summary>
  ///   Total 24h trade volume for futures. This is expressed in the base currency, e.g. BTC for btc_usd
  /// </summary>
  [JsonProperty("futures_volume")]
  public decimal FuturesVolume { get; set; }

  /// <summary>
  ///   Total 24h trade volume for put options. This is expressed in the base currency, e.g. BTC for btc_usd
  /// </summary>
  [JsonProperty("puts_volume")]
  public decimal PutsVolume { get; set; }
}
