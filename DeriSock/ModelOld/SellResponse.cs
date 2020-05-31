namespace DeriSock.Model
{
  using Newtonsoft.Json.Linq;

  public class SellResponse
  {
    public OrderInfo order;
    public JArray trades;
  }
}
