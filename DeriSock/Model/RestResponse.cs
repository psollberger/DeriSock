namespace DeriSock.Model
{
  using Newtonsoft.Json.Linq;

  public class RestResponse
  {
    public long usOut;
    public long usIn;
    public long usDiff;
    public bool testnet;
    public bool success;
    public JToken result;
    public string message;
    public int error;
  }
}
