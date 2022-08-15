namespace DeriSock.Utils;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

internal class SignatureData
{
  private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
  private static readonly Random Random = new();

  public DateTime Timestamp { get; set; }
  public string Nonce { get; set; } = string.Empty;
  public string? Data { get; set; }
  public string Signature { get; set; } = string.Empty;

  private SignatureData() { }

  public static SignatureData Create(string clientId, string clientSecret, string data = "")
  {
    var result = new SignatureData
    {
      Timestamp = DateTime.UtcNow,
      Nonce = CreateNonce(8),
      Data = string.IsNullOrEmpty(data) ? null : data
    };

    var signature = $"{result.Timestamp.ToUnixTimeMilliseconds()}\n{result.Nonce}\n{result.Data}";
    using var hash = new HMACSHA256(Encoding.ASCII.GetBytes(clientSecret));

    result.Signature =
      BitConverter.ToString(hash.ComputeHash(Encoding.ASCII.GetBytes(signature))).Replace("-", "").ToLower();

    return result;
  }

  private static string CreateNonce(int length)
  {
    return new string(Enumerable.Repeat(Chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
  }
}
