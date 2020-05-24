namespace DeriSock.Model
{
  using System.Collections.Generic;

  public class BookResponse
  {
    public List<double[]> asks;
    public List<double[]> bids;
    public long change_id;

    public double Bid { get => bids[0][0]; }
    public double Ask { get => asks[0][0]; }

    public override string ToString()
    {
      return $"{change_id}, BID: {Bid:0.00}, ASK: {Ask:0.00}";
    }
  }
}
