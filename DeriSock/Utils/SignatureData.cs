namespace DeriSock.Utils;

public class SignatureData
{
  public long Timestamp { get; set; }
  public string Nonce { get; set; } = string.Empty;
  public string Data { get; set; } = string.Empty;
  public string Signature { get; set; }
}
