namespace DeriSock.Extensions
{
  using System;

  public static class StringExtensions
  {
    public static string[] SubArray(this string[] data, int index, int length)
    {
      var result = new string[length];
      Array.Copy(data, index, result, 0, length);
      return result;
    }
  }
}
