namespace DeriSock.Utils
{
  using System;
  using System.Linq;
  using System.Security.Cryptography;
  using System.Text;
  using DeriSock.Extensions;

  public static class SignatureCreator
  {
    private const string Chars = "abcdefghijklmnopqrstuvwxyz0123456789";
    private static readonly Random Random = new Random();

    public static long Timestamp { get; private set; }
    public static string Nonce { get; private set; }
    public static string Data { get; set; } = string.Empty;
    public static string Signature { get; private set; }

    public static void Create(string clientSecret)
    {
      Timestamp = DateTime.UtcNow.AsMilliseconds();
      Nonce = CreateNonce(8);
      var signature = $"{Timestamp}\n{Nonce}\n{Data}";
      using (var hash = new HMACSHA256(Encoding.ASCII.GetBytes(clientSecret)))
      {
        Signature = BitConverter.ToString(hash.ComputeHash(Encoding.ASCII.GetBytes(signature))).Replace("-", "").ToLower();
      }
    }

    private static string CreateNonce(int length)
    {
      return new string(Enumerable.Repeat(Chars, length).Select(s => s[Random.Next(s.Length)]).ToArray());
    }
  }
}
