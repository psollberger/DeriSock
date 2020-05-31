namespace DeriSock.Model
{
  using System.Collections.Generic;

  public class RawBookResponse
  {
    public List<object[]> asks;
    public List<object[]> bids;
    public long change_id;
    public long date;
    public string instrument_name;
    public long prev_change_id;
  }
}
