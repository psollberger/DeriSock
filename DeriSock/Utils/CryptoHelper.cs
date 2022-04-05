namespace DeriSock.Utils;

using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

public static class CryptoHelper
{
  private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
  private static readonly Random Random = new();

  public static SignatureData CreateSignature(string clientSecret, string data = "")
  {
    var result = new SignatureData
    {
      Timestamp = DateTime.UtcNow.AsMilliseconds(), Nonce = CreateNonce(8), Data = data
    };
    var signature = $"{result.Timestamp}\n{result.Nonce}\n{result.Data}";
    using (var hash = new HMACSHA256(Encoding.ASCII.GetBytes(clientSecret)))
    {
      result.Signature = BitConverter.ToString(hash.ComputeHash(Encoding.ASCII.GetBytes(signature))).Replace("-", "").ToLower();
    }

    return result;
  }

  public static string CreateNonce(int length)
  {
    return new string(Enumerable.Repeat(Chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
  }
}
