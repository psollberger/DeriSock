namespace DeriSock.Model
{
    using Newtonsoft.Json.Linq;

    public class BuySellResponse
    {
        public JArray trades;
        public OrderInfo order;
    }
}
