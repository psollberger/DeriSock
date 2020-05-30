namespace DeriSock.Model
{
  using System;
  using DeriSock.Extensions;

  public class SettlementInformation
  {
    public string type { get; set; }
    public long timestamp { get; set; }

    public DateTime LocalDateTime => timestamp.AsDateTimeFromMilliseconds();

    public double session_profit_loss { get; set; }
    public double profit_loss { get; set; }
    public int position { get; set; }
    public double mark_price { get; set; }
    public string instrument_name { get; set; }
    public double index_price { get; set; }
    public double funding { get; set; }
  }
}
