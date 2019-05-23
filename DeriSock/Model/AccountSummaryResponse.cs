namespace DeriSock.Model
{
  /// <summary>
  /// See: https://docs.deribit.com/v2/#private-get_account_summary
  /// </summary>
  public class AccountSummaryResponse
  {
    public string username;
    public string type;
    public double total_pl;
    public bool tfa_enabled;
    public string system_name;
    public double session_upl;
    public double session_rpl;
    public double session_funding;
    public double margin_balance;
    public double maintenance_margin;
    public double initial_margin;
    public int id;
    public double futures_session_upl;
    public double futures_session_rpl;
    public double futures_pl;
    public double equity;
    public string email;
    public string deposit_address;
    public double delta_total;
    public string currency;
    public double balance;
    public double available_withdrawal_funds;
    public double available_funds;
  }
}
