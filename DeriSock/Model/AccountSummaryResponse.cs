namespace DeriSock.Model
{
  /// <summary>
  ///   See: https://docs.deribit.com/v2/#private-get_account_summary
  /// </summary>
  public class AccountSummaryResponse
  {
    public double available_funds;
    public double available_withdrawal_funds;
    public double balance;
    public string currency;
    public double delta_total;
    public string deposit_address;
    public string email;
    public double equity;
    public double futures_pl;
    public double futures_session_rpl;
    public double futures_session_upl;
    public int id;
    public double initial_margin;
    public double maintenance_margin;
    public double margin_balance;
    public double session_funding;
    public double session_rpl;
    public double session_upl;
    public string system_name;
    public bool tfa_enabled;
    public double total_pl;
    public string type;
    public string username;
  }
}
