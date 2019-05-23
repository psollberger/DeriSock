namespace DeriSock.Model
{
  using Newtonsoft.Json.Linq;

  public class JsonRpcResponse
  {
    public int id;
    public JToken result;
    public JToken error; //in api v2 it will be JsonRpcError, but in method proxied from v1 it will be int
    public bool testnet;
    public long usIn;
    public long usOut;
    public long usDiff;
  }
}
