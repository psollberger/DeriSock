using Newtonsoft.Json.Linq;

namespace DeriSock.Model
{
    public class SellResponse
    {
        public JArray trades;
        public OrderInfo order;
    }
}
