namespace DeriSock.Model;

using System;

public class InstrumentName
{
  public string Name { get; set; } = string.Empty;

  public InstrumentType Type => Name.GetInstrumentType();

  public OptionType OptionType => Name.GetOptionType();

  /// <summary>
  ///   Expiry date of option contract
  /// </summary>
  public DateTime ExpiryDate => Name.ToInstrumentExpiration();

  /// <summary>
  ///   Days until expiry of option contrat
  /// </summary>
  public double DaysToExpiry => ExpiryDate.ToTotalDaysFromNow();

  /// <summary>
  ///   Strike price of option contract
  /// </summary>
  public decimal Strike => GetStrikePrice();

  private decimal GetStrikePrice()
    => Type == InstrumentType.Option ? Name.GetStrikePrice() : 0;
}
