namespace DeriSock.Model
{
    using System.Collections.Generic;

    public class RawBookResponse
    {
        public string instrument_name;
        public List<object[]> bids;
        public List<object[]> asks;
        public long prev_change_id;
        public long date;
        public long change_id;
    }
}
