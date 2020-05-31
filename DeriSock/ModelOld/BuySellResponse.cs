namespace DeriSock.Model
{
  using Newtonsoft.Json.Linq;

  public class BuySellResponse
  {
    public OrderInfo order;
    public JArray trades;
  }
}
