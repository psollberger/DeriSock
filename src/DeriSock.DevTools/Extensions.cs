namespace DeriSock.DevTools;

using System.IO;
using System.Linq;

internal static class Extensions
{
  public static bool IsRegexPattern(this string value)
  {
    return value.Any(c => c is '^' or '$' or '*' or '?' or '+' or '[' or ']' or '(' or ')' or '|');
  }

  public static void DeleteFiles(this string path, string searchPattern)
  {
    try
    {
      var filePaths = Directory.EnumerateFiles(path, searchPattern);

      foreach (var filePath in filePaths)
        File.Delete(filePath);
    }
    catch
    {
      // ignore
    }
  }
}
