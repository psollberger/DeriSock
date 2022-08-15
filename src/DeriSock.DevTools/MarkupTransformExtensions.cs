namespace DeriSock.DevTools;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

using HtmlAgilityPack;

internal static class MarkupTransformExtensions
{
  private static readonly Regex HyperLinkPattern = new(@"\[(?<text>[^\]]+)\]\((?<link>[^\)]+)\)", RegexOptions.Compiled | RegexOptions.Singleline);

  public static IEnumerable<string> ToXmlDocParagraphs(this string value)
  {
    if (string.IsNullOrEmpty(value))
      yield break;

    var textLines = value.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    var longestLineLength = textLines.Max(l => l.Length);

    var builder = new StringBuilder(longestLineLength);

    foreach (var textLine in textLines) {
      builder.Clear();
      builder.Append(SecurityElement.Escape(textLine));
      ConvertMarkdown(builder, "`", "<c>", "</c>");
      ConvertMarkdown(builder, "__", "<b>", "</b>");
      ConvertMarkdownHyperlinksToXmlDocLinks(builder);
      yield return builder.ToString();
    }
  }

  private static void ConvertMarkdown(StringBuilder haystack, string needle, string openTag, string closeTag)
  {
    var needleLength = needle.Length;

    var inTag = false;

    for (var i = haystack.Length - needleLength; i >= 0; --i) {
      if (i == needleLength - 1 && !inTag)
        break;

      var match = true;

      for (var j = 0; j < needleLength; ++j) {
        if (haystack[i + j] != needle[j]) {
          match = false;
          break;
        }
      }

      if (!match)
        continue;

      haystack.Replace(needle, inTag ? openTag : closeTag, i, needleLength);
      inTag = !inTag;
    }
  }

  private static void ConvertMarkdownHyperlinksToXmlDocLinks(StringBuilder haystack)
  {
    var matches = HyperLinkPattern.Matches(haystack.ToString());

    foreach (Match match in matches) {
      haystack.Replace(match.Value, $"<a href=\"{match.Groups["link"]}\">{match.Groups["text"]}</a>");
    }
  }

  public static string ToMarkdown(this HtmlNode node)
  {
    var sb = new StringBuilder(HttpUtility.HtmlDecode(node.InnerHtml));
    sb.Replace("\n", " ");
    sb.Replace("\r", "");

    sb.Replace("<code>", "`");
    sb.Replace("</code>", "`");
    sb.Replace("<strong>", "__");
    sb.Replace("</strong>", "__");
    sb.Replace("<br>", "\n");
    sb.Replace("</br>", "\n");
    sb.Replace("<p>", "");
    sb.Replace("</p>", "\n\n");

    if (sb.Length > 0)
      while (sb[^1] == '\n')
        sb.Remove(sb.Length - 1, 1);

    node.InnerHtml = sb.ToString();

    var childCount = node.ChildNodes.Count;

    // Convert Anchor Tags
    for (var i = childCount - 1; i >= 0; --i) {
      var childNode = node.ChildNodes[i];

      if (childNode.Name != "a")
        continue;

      var linkText = childNode.InnerText;
      var linkHref = string.Concat("https://docs.deribit.com/", childNode.Attributes["href"].Value);

      sb.Replace(childNode.OuterHtml, $"[{linkText}]({linkHref})");
    }

    // Convert unordered lists
    for (var i = childCount - 1; i >= 0; --i) {
      var childNode = node.ChildNodes[i];

      if (childNode.Name != "ul")
        continue;

      var listBuilder = new StringBuilder(childNode.InnerText.Length);
      var listNodes = childNode.ChildNodes;

      var firstListElement = true;

      foreach (var listNode in listNodes) {
        if (listNode.Name != "li")
          continue;

        if (!firstListElement)
          listBuilder.Append("\n");
        firstListElement = false;
        listBuilder.Append(string.Concat("- ", listNode.InnerText));
      }

      sb.Replace(childNode.OuterHtml, listBuilder.ToString());
    }

    return sb.ToString();
  }
}
