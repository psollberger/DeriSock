namespace DeriSock;

using System;
using System.Text;

internal static class InternalHelperExtensions
{
  public static string GetString(this Encoding encoding, ArraySegment<byte> value, int index, int count)
  {
    if (value.Array is null)
      return string.Empty;

    return encoding.GetString(value.Array, index, count);
  }

#if !NETSTANDARD2_0
  public static string GetString(this Encoding encoding, Memory<byte> value, int index, int count)
  {
    return encoding.GetString(value.Span.Slice(index, count));
  }
#endif
}
