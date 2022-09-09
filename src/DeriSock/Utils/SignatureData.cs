namespace DeriSock.Utils;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using DeriSock.Model;

internal class SignatureData
{
  private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
  private static readonly Random Random = new();

  private readonly string _clientSecret;

  public DateTime Timestamp { get; set; }
  public string Nonce { get; set; } = string.Empty;
  public string? Data { get; set; }
  public string Signature { get; set; } = string.Empty;

  private SignatureData(string clientSecret)
  {
    _clientSecret = clientSecret;
  }

  public static SignatureData Create(string clientSecret, string data = "")
  {
    var result = new SignatureData(clientSecret)
    {
      Data = string.IsNullOrEmpty(data) ? null : data
    };

    result.Refresh();

    return result;
  }

  private static string CreateNonce(int length)
  {
    return new string(Enumerable.Repeat(Chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
  }

  public void Refresh()
  {
    Timestamp = DateTime.UtcNow;
    Nonce = CreateNonce(8);

    var signature = $"{Timestamp.ToUnixTimeMilliseconds()}\n{Nonce}\n{Data}";
    using var hash = new HMACSHA256(Encoding.ASCII.GetBytes(_clientSecret));

    Signature = BitConverter.ToString(hash.ComputeHash(Encoding.ASCII.GetBytes(signature))).Replace("-", "").ToLower();
  }

  public void Apply(PublicAuthRequest request)
  {
    request.Timestamp = Timestamp;
    request.Signature = Signature;
    request.Nonce = Nonce;
    request.Data = Data;
  }
}
