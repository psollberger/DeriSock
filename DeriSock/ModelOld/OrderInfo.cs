namespace DeriSock.Model
{
  public class OrderInfo
  {
    public long amount;
    public long creation_timestamp;
    public string direction;
    public long last_update_timestamp;

    public string order_id;
    public string order_state;
    public string order_type;
    public bool post_only;
    public decimal price;
    public string time_in_force;
  }
}
