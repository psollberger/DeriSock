namespace DeriSock.Model
{
  using System;

  public class PortfolioResponse
  {
    public double total_pl;
    public double session_upl;
    public double session_rpl;
    public double session_funding;
    public bool portfolio_margining_enabled;
    public double margin_balance;
    public double maintenance_margin;
    public double initial_margin;
    public double futures_session_upl;
    public double futures_session_rpl;
    public double futures_pl;
    public double equity;
    public double delta_total;
    public string currency;
    public double balance;
    public double available_withdrawal_funds;
    public double available_funds;
  }
}
