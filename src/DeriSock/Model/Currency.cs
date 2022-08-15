namespace DeriSock.Model;

using Newtonsoft.Json;

public class Currency
{
  /// <summary>
  ///   The type of the currency.
  /// </summary>
  [JsonProperty("coin_type")]
  public string CoinType { get; set; }

  /// <summary>
  ///   The abbreviation of the currency. This abbreviation is used elsewhere in the API to identify the currency.
  /// </summary>
  [JsonProperty("currency")]
  public string CurrencyName { get; set; }

  /// <summary>
  ///   The full name for the currency.
  /// </summary>
  [JsonProperty("currency_long")]
  public string CurrencyFullName { get; set; }

  /// <summary>
  ///   False if deposit address creation is disabled
  /// </summary>
  [JsonProperty("disabled_deposit_address_creation")]
  public bool DisabledDepositAddressCreation { get; set; }

  /// <summary>
  ///   fee precision
  /// </summary>
  [JsonProperty("fee_precision")]
  public int FeePrecision { get; set; }

  /// <summary>
  ///   Minimum number of block chain confirmations before deposit is accepted.
  /// </summary>
  [JsonProperty("min_confirmations")]
  public int MinConfirmations { get; set; }

  /// <summary>
  ///   The minimum transaction fee paid for withdrawals
  /// </summary>
  [JsonProperty("min_withdrawal_fee")]
  public decimal MinWithdrawalFee { get; set; }

  /// <summary>
  ///   The total transaction fee paid for withdrawals
  /// </summary>
  [JsonProperty("withdrawal_fee")]
  public decimal WithdrawalFee { get; set; }

  [JsonProperty("withdrawal_priorities")]
  public CurrencyWithdrawalPriority[] WithdrawalPriorities { get; set; }
}
