namespace DeriSock.Model
{
  public class TickerResponse
  {
    public double best_ask_amount;
    public double best_ask_price;
    public double best_ask_qty;
    public double best_bid_amount;
    public double best_bid_price;
    public double best_bid_qty;
    public double current_funding;
    public double funding_8h;
    public double index_price;
    public string instrument_name;
    public double last_price;
    public double mark_price;
    public double max_price;
    public double min_price;
    public double open_interest;
    public double settlement_price;
    public string state;
    public TickerStats stats;
    public long time;
  }
}
