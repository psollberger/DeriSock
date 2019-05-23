namespace DeriSock.Model
{
    using System;

    public class OrderResponse
    {
        public string order_id;
        public string order_state;
        public double price;
        public long amount;
        public long filled_amount;
        public string direction;
        public DateTime timestamp;
        public string label;

        public override string ToString()
        {
            return $"Timestamp: {timestamp:dd.MM.yyyy HH:mm:ss.fff} ; Id: {order_id} ; State: {order_state} ; Price: {price:N2} ; Direction: {direction}";
        }
    }
}
