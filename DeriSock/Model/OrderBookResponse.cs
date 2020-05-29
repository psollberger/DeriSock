namespace DeriSock.Model
{
  using System.Collections.Generic;

  public class OrderBookResponse
  {
    public List<OrderBookRow> asks;
    public List<OrderBookRow> bids;
    public double high;
    public string instrument;
    public double last;
    public double low;
    public double mark;
    public double max;
    public double min;
    public double settlementPrice;
    public string state;
    public long tstamp;
  }
}
