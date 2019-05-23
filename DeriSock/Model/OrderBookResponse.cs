namespace DeriSock.Model
{
    using System.Collections.Generic;

    public class OrderBookResponse
    {
        public string state;
        public double settlementPrice;
        public string instrument;
        public List<OrderBookRow> asks;
        public List<OrderBookRow> bids;
        public long tstamp;
        public double last;
        public double low;
        public double high;
        public double mark;
        public double min;
        public double max;
    }
}
