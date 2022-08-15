namespace DeriSock.DevTools;

using System.Linq;

public static class Extensions
{
  public static bool IsRegexPattern(this string value)
  {
    return value.Any(c => c is '^' or '$' or '*' or '?' or '+' or '[' or ']' or '(' or ')' or '|');
  }
}
