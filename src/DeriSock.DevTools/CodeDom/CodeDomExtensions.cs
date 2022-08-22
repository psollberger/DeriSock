namespace DeriSock.DevTools.CodeDom;

using System.CodeDom;
using System.Globalization;
using System.Text;

using DeriSock.DevTools.ApiDoc.Model;
using DeriSock.Model;

public static class CodeDomExtensions
{
  private static readonly TextInfo EnglishTextInfo = new CultureInfo("en-us", false).TextInfo;

  public static DataTypeInfo GetDataTypeInfo(this ApiDocProperty property)
  {
    var type = property.DataType switch
    {
      "number"           => typeof(decimal).FullName,
      "float"            => typeof(double).FullName,
      "long"             => typeof(long).FullName,
      "integer"          => typeof(int).FullName,
      "boolean"          => typeof(bool).FullName,
      "string"           => typeof(string).FullName,
      "text"             => typeof(string).FullName,
      "object or string" => typeof(ObjectOrStringItem).FullName,
      "array" => property.ArrayDataType switch
      {
        "number"                  => typeof(decimal).FullName,
        "float"                   => typeof(double).FullName,
        "integer"                 => typeof(int).FullName,
        "boolean"                 => typeof(bool).FullName,
        "string"                  => typeof(string).FullName,
        "text"                    => typeof(string).FullName,
        "[price, amount]"         => typeof(PriceAmountItem).FullName,
        "[action, price, amount]" => typeof(ActionPriceAmountItem).FullName,
        "[timestamp, value]"      => typeof(TimestampValueItem).FullName,
        _                         => property.ArrayDataType
      },
      _ => property.DataType
    } ?? string.Empty;

    return new DataTypeInfo(type, property.IsArray, !property.Required);
  }

  public static string ToPublicCodeName(this string value)
    => ToCodeName(value, false);

  public static string ToPrivateCodeName(this string value)
    => ToCodeName(value, true);

  private static string ToCodeName(string value, bool isPrivate)
  {
    if (string.IsNullOrEmpty(value))
      return value;

    var sb = new StringBuilder(EnglishTextInfo.ToTitleCase(value));

    if (isPrivate)
      sb[0] = EnglishTextInfo.ToLower(sb[0]);

    for (var i = 0; i < sb.Length; ++i) {
      var c = sb[i];

      switch (c) {
        case >= 'a' and <= 'z':
        case >= 'A' and <= 'Z':
        case >= '0' and <= '9':
          continue;

        default:
          sb.Remove(i, 1);
          i--;
          break;
      }
    }

    value = sb.ToString();

    if (string.IsNullOrEmpty(value))
      return value;

    if (char.IsNumber(value[0]))
      return $"_{value}";

    return value;
  }

  public static int GetNestedArrayDepth(this CodeTypeReference value)
    => value.ArrayElementType is null ? 0 : 1 + value.GetNestedArrayDepth();

  public static bool HasCharAt(this string value, int index, char character)
    => index < value.Length && value[index] == character;
}
