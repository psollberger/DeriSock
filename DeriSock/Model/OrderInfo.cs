namespace DeriSock.Model
{
    public class OrderInfo
    {
        public long creation_timestamp;
        public long last_update_timestamp;

        public string order_id;
        public string order_type;
        public string direction;
        public long amount;
        public decimal price;
        public bool post_only;
        public string time_in_force;
        public string order_state;
    }
}
