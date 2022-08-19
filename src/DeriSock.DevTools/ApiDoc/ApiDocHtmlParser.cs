namespace DeriSock.DevTools.ApiDoc;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

using DeriSock.DevTools.ApiDoc.Model;

using HtmlAgilityPack;

public class ApiDocHtmlParser
{
  private const string TimestampDataType = "DateTime";
  private static readonly string[] TimestampConverters = new[] { "MillisecondsTimestampConverter" };
  private readonly HtmlDocument _htmlDoc;

  private string _version = "0.0.0";
  private readonly ApiDocFunctionCollection _methods = new(130);
  private readonly ApiDocFunctionCollection _subscriptions = new(40);

  private ApiDocHtmlParser(HtmlDocument htmlDoc)
  {
    _htmlDoc = htmlDoc;
  }

  public static ApiDocDocument CreateDocument(HtmlDocument htmlDoc)
  {
    var parser = new ApiDocHtmlParser(htmlDoc);

    parser.ParseVersion();
    parser.ParseMethods();
    parser.ParseSubscriptions();

    var apiDoc = new ApiDocDocument(parser._version, parser._methods, parser._subscriptions);
    apiDoc.UpdateRelations();

    return apiDoc;
  }

  public void ParseVersion()
  {
    var versionNode = FindVersionTitle(_htmlDoc);

    if (versionNode is null)
      return;

    _version = versionNode.InnerText["Deribit API v".Length..];
  }

  public void ParseMethods()
  {
    foreach (var categoryTitleNode in FindAllCategoryTitles(_htmlDoc)) {
      foreach (var methodTitleNode in FindAllCategoryMethodTitles(categoryTitleNode)) {
        var (key, value) = ParseMethod(categoryTitleNode, methodTitleNode);
        _methods.Add(key, value);
      }
    }
  }

  public void ParseSubscriptions()
  {
    foreach (var subscriptionTitleNode in FindAllSubscriptionTitles(_htmlDoc)) {
      var (key, value) = ParseSubscription(subscriptionTitleNode);
      _subscriptions.Add(key, value);
    }
  }

  private static KeyValuePair<string, ApiDocFunction> ParseMethod(HtmlNode categoryNode, HtmlNode titleNode)
  {
    // <h1 id="methods">
    // ...
    // ...
    // ...
    // <h2 id="public-method">/public/method</h2>
    // <pre> elements with classes: highlight <language> tab-<language>
    // also (maybe) a blockquote element
    // [0..n] <p> elements with method description
    // [0..1] <aside> When it's a private method
    // <h3 id="parameters[-n]">
    // <table> | <p> > <em>This method takes no parameters</em>
    // <h3 id="response[-n]>
    // <table>
    // ...
    // ... repeat from h2
    // ...

    var categoryName = categoryNode.InnerText;
    var methodName = titleNode.InnerText[1..];

    var result = new ApiDocFunction
    {
      FunctionType = ApiDocFunctionType.Method,
      Category = categoryName
    };

    foreach (var descNode in GetAllDescriptionNodes(titleNode)) {
      if (descNode.Name == "li")
        result.Description += "- ";

      result.Description += descNode.ToMarkdown() + "\n";
    }

    result.Description = HttpUtility.HtmlDecode(result.Description.TrimEnd());

    result.Deprecated = GetSectionDeprecated(titleNode);

    var hasParameters = true;
    var hasResponse = true;
    var paramsTableBody = titleNode.NextSibling;

    while (paramsTableBody.Name != "table") {
      if (paramsTableBody.Name == "p" && paramsTableBody.InnerText == "This method takes no parameters") {
        hasParameters = false;
        break;
      }

      paramsTableBody = paramsTableBody.NextSibling;
    }

    var responseTableBody = paramsTableBody.NextSibling;

    while (responseTableBody.Name != "table") {
      if (responseTableBody.Name == "p" && responseTableBody.InnerText == "This method has no response body") {
        hasResponse = false;
        break;
      }

      responseTableBody = responseTableBody.NextSibling;
    }

    if (hasParameters)
      result.Request = ParameterTableParser.Parse(paramsTableBody);

    if (hasResponse)
      result.Response = ResponseTableParser.Parse(responseTableBody, "result");

    return KeyValuePair.Create(methodName, result);
  }

  private static KeyValuePair<string, ApiDocFunction> ParseSubscription(HtmlNode titleNode)
  {
    // <h1 id="subscriptions">
    // ...
    // ...
    // ...
    // <h2 id="subscription-name">subscription-name-with-variables-in-curly-braces</h2>
    // <blockquote> telling subscriptions only with websockets
    // <pre> elements with classes: highlight <language> tab-<language>
    // also (maybe) a blockquote element
    // [0..n] <p> elements with method description
    // <h3 id="channel-parameters[-n]">
    // <table> | <p> > <em>This channel takes no parameters</em>
    // <h3 id="response[-n]>
    // <table>
    // ...
    // ... repeat from h2
    // ...


    var channelName = titleNode.InnerText;

    var result = new ApiDocFunction
    {
      FunctionType = ApiDocFunctionType.Subscription
    };

    foreach (var descNode in GetAllDescriptionNodes(titleNode)) {
      if (descNode.Name == "li")
        result.Description += "- ";

      result.Description += descNode.ToMarkdown() + "\n";
    }

    result.Description = HttpUtility.HtmlDecode(result.Description.TrimEnd());

    var hasParameters = true;
    var paramsTableBody = titleNode.NextSibling;

    while (paramsTableBody.Name != "table") {
      if (paramsTableBody.Name == "p" && paramsTableBody.InnerText is "This channel takes no parameters" or "This method takes no parameters") {
        hasParameters = false;
        break;
      }

      paramsTableBody = paramsTableBody.NextSibling;
    }

    var responseTableBody = paramsTableBody.NextSibling;

    while (responseTableBody.Name != "table")
      responseTableBody = responseTableBody.NextSibling;

    if (hasParameters)
      result.Request = ParameterTableParser.Parse(paramsTableBody);

    result.Response = ResponseTableParser.Parse(responseTableBody, "data");

    return KeyValuePair.Create(channelName, result);
  }

  private static HtmlNode? FindVersionTitle(HtmlDocument htmlDoc)
  {
    var overviewTitleNode = htmlDoc.GetElementbyId("overview");
    return overviewTitleNode?.PreviousSibling;
  }

  private static IEnumerable<HtmlNode> FindAllCategoryTitles(HtmlDocument htmlDoc)
  {
    var methodsTitleNode = htmlDoc.GetElementbyId("methods");

    if (methodsTitleNode == null)
      yield break;

    var curNode = methodsTitleNode;

    while (true) {
      curNode = curNode.NextSibling;

      if (curNode == null)
        break;

      if (curNode.Id == "subscriptions")
        yield break;

      if (curNode.Name != "h1")
        continue;

      yield return curNode;
    }
  }

  private static IEnumerable<HtmlNode> FindAllCategoryMethodTitles(HtmlNode categoryTitleNode)
  {
    var curNode = categoryTitleNode;

    while (true) {
      curNode = curNode.NextSibling;

      if (curNode == null)
        break;

      if (curNode.Name == "h1")
        yield break;

      if (curNode.Name != "h2")
        continue;

      yield return curNode;
    }
  }

  private static IEnumerable<HtmlNode> GetAllDescriptionNodes(HtmlNode titleNode)
  {
    var curNode = titleNode.NextSibling;

    while (curNode.Name != "p")
      curNode = curNode.NextSibling;

    while (curNode.Name != "h3") {
      switch (curNode.Name) {
        case "#text":
          // do nothing with those nodes
          break;

        case "aside":
          // A notice.
          // With class multi-notice: This is a private method; it can only be used after authentication.
          // With class warning: This method is deprecated and will be removed in the future.
          break;

        case "ul":
          foreach (var liNode in curNode.ChildNodes) {
            if (liNode.Name == "#text")
              continue;

            yield return liNode;
          }

          break;

        default:
          if (curNode.Name == "p" && curNode.InnerText.Equals("Try in API console"))
            break;

          yield return curNode;

          break;
      }

      curNode = curNode.NextSibling;
    }
  }

  private static bool GetSectionDeprecated(HtmlNode titleNode)
  {
    var curNode = titleNode.NextSibling;

    while (curNode.Name != "p")
      curNode = curNode.NextSibling;

    while (curNode.Name != "h3") {
      switch (curNode.Name) {
        case "aside":
          // A notice.
          // With class multi-notice: This is a private method; it can only be used after authentication.
          // With class warning: This method is deprecated and will be removed in the future.
          if (curNode.HasClass("warning") && curNode.InnerText.Contains("is deprecated and will be removed", StringComparison.OrdinalIgnoreCase))
            return true;

          break;
      }

      curNode = curNode.NextSibling;
    }

    return false;
  }

  private static IEnumerable<HtmlNode> FindAllSubscriptionTitles(HtmlDocument htmlDoc)
  {
    var subscriptionsTitleNode = htmlDoc.GetElementbyId("subscriptions");

    if (subscriptionsTitleNode == null)
      yield break;

    var curNode = subscriptionsTitleNode;

    while (true) {
      curNode = curNode.NextSibling;

      if (curNode == null)
        break;

      if (curNode.Id == "rpc-error-codes")
        yield break;

      if (curNode.Name != "h2")
        continue;

      yield return curNode;
    }
  }

  private static void ApplyTimestampFieldValues(ApiDocProperty property)
  {
    property.DataType = TimestampDataType;
    property.Converters = TimestampConverters.Concat(property.Converters ?? Enumerable.Empty<string>()).ToArray();
  }

  private static class ParameterTableParser
  {
    private const string ArrayTypeMarker = "array of ";
    private const string ObjectArrayParamMarker = "&nbsp;&nbsp;&rsaquo;&nbsp;&nbsp;";

    private static readonly Regex[] DefaultValuePattern =
    {
      new(@"default: (?<value>\S+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"default - (?<value>[^)]+).*", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"by default (?<value>\S+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"by default (?<value>\S+)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\(default (?<value>\S+)\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\. Default (?<value>\S+)<", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled)
    };

    public static ApiDocProperty Parse(HtmlNode tableNode)
    {
      var apiRequestProperties = new ApiDocPropertyCollection(10);
      var apiRequestSubProperties = new ApiDocPropertyCollection(10);

      tableNode = tableNode.FirstChild.NextSibling;
      var paramsTableRows = tableNode.SelectNodes("tr");

      ApiDocProperty? objectArrayParam = null;

      foreach (var row in paramsTableRows) {
        var rowColumns = row.SelectNodes("td");

        var paramName = rowColumns[0].InnerText;

        var param = new ApiDocProperty
        {
          Required = rowColumns[1].InnerText == "true",
          ApiDataType = rowColumns[2].InnerText,
          DataType = rowColumns[2].InnerText,
          EnumValues = rowColumns[3].InnerHtml.Split("<br>"),
          Description = rowColumns[4].ToMarkdown()
        };

        var isObjectArrayParam = paramName.StartsWith(ObjectArrayParamMarker, StringComparison.Ordinal);

        if (isObjectArrayParam)
          paramName = paramName.Replace(ObjectArrayParamMarker, string.Empty);

        if (param.EnumValues.Length == 1 && string.IsNullOrEmpty(param.EnumValues[0]))
          param.EnumValues = null;
        else
          for (var i = 0; i < param.EnumValues.Length; i++)
            param.EnumValues[i] = param.EnumValues[i].Replace("<code>", string.Empty).Replace("</code>", string.Empty);

        param.DefaultValue = GetDefaultValue(param);

        if (isObjectArrayParam && objectArrayParam is not null) {
          apiRequestSubProperties.Add(paramName, param);
        }
        else {
          if (objectArrayParam is not null) {
            objectArrayParam.Properties = apiRequestSubProperties;
            objectArrayParam = null;
          }

          var isArray = param.ApiDataType.StartsWith(ArrayTypeMarker);

          if (isArray) {
            var arrayTypeName = param.ApiDataType[ArrayTypeMarker.Length..];

            if (arrayTypeName?.Equals("objects", StringComparison.OrdinalIgnoreCase) ?? false)
              arrayTypeName = "object";

            param.DataType = "array";
            param.ArrayDataType = arrayTypeName;

            apiRequestSubProperties.Clear();
            objectArrayParam = param;
          }

          if (paramName.Contains("timestamp"))
            ApplyTimestampFieldValues(param);

          apiRequestProperties.Add(paramName, param);
        }
      }

      if (objectArrayParam is not null)
        objectArrayParam.Properties = apiRequestSubProperties;

      return new ApiDocProperty
      {
        Name = "request",
        Properties = apiRequestProperties
      };
    }

    private static object? GetDefaultValue(ApiDocProperty prop)
    {
      Match m;
      var idxPattern = 0;

      if (string.IsNullOrEmpty(prop.Description))
        return null;

      do {
        m = DefaultValuePattern[idxPattern++].Match(prop.Description);
      } while (!m.Success && idxPattern < DefaultValuePattern.Length);

      if (!m.Success)
        return null;

      var value = m.Groups["value"].Value;
      value = value.Replace("`", "");
      value = value.Replace("\"", "");

      if (bool.TrueString.Equals(value, StringComparison.OrdinalIgnoreCase))
        return true;

      if (bool.FalseString.Equals(value, StringComparison.OrdinalIgnoreCase))
        return false;

      if (value.Contains('.') && double.TryParse(value, out var numNumeric))
        return numNumeric;

      if (int.TryParse(value, out var numInteger))
        return numInteger;

      return value;
    }
  }

  private static class ResponseTableParser
  {
    private const string ArrayTypeMarker = "array of ";
    private const string ObjectArrayParamMarker = "&nbsp;&nbsp;&rsaquo;&nbsp;&nbsp;";

    private static readonly Regex[] DeprecatedParamPattern =
    {
      new(@"\(field is deprecated and will be removed in the future\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"^\[DEPRECATED\] ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled)
    };

    private static readonly Regex[] OptionalParamPattern =
    {
      new(@"^Optional .+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"^Optional, .+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\(available when ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\(optional\)[\.]?$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\. Optional field$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@", optional for ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\(optional, only for ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"field is omitted", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"only for ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\. Only when ", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"\(options only\)", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@"Field not included if", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@" only\)$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled),
      new(@", null otherwise$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled)
    };

    public static ApiDocProperty Parse(HtmlNode tableNode, string dataObjectPropertyName)
    {
      var (resultNode, isComplex, apiTypeName, dataType, arrayDataType, description, required) = FindDataObjectEntry(tableNode, dataObjectPropertyName);

      var result = new ApiDocProperty
      {
        Name = "response",
        Required = required,
        Description = string.IsNullOrEmpty(description) ? null : description,
        ApiDataType = apiTypeName,
        DataType = dataType,
        ArrayDataType = arrayDataType
      };

      if (isComplex)
        result.Properties = ParseComplexType(resultNode, 1);

      if (isComplex && result.Properties is { Count: < 1 }) {
        result.Properties = null;
      }

      if (TimestampDataType.Equals(result.DataType))
        ApplyTimestampFieldValues(result);

      return result;
    }

    private static (HtmlNode resultNode, bool isComplex, string apiDataType, string dataType, string? arrayDataType, string description, bool required) FindDataObjectEntry(HtmlNode tableNode, string dataObjectPropertyName)
    {
      tableNode = tableNode.FirstChild.NextSibling;
      var responseTableRows = tableNode.SelectNodes("tr");

      HtmlNode? resultPropertyRow = null;
      HtmlNode[]? resultPropertyRowColumns = null;

      (resultPropertyRow, resultPropertyRowColumns) = FindRowWithName(responseTableRows, dataObjectPropertyName, resultPropertyRow, resultPropertyRowColumns);

      if (resultPropertyRowColumns is null && dataObjectPropertyName == "result") {
        // In 'private/get_user_trades_by_order' the result row is kind of missing
        // The amount property is half in the row as "array of | amount | number"
        // Seems as if 'result | array of object' was somehow cut
        (resultPropertyRow, resultPropertyRowColumns) = FindRowWithName(responseTableRows, "array of", resultPropertyRow, resultPropertyRowColumns);
        TryRepairTableIfNeeded(ref resultPropertyRow, ref resultPropertyRowColumns);
      }

      Debug.Assert(resultPropertyRow is not null && resultPropertyRowColumns is not null, $"Response without a {dataObjectPropertyName} row");

      var colValueType = resultPropertyRowColumns[1].InnerText;
      var description = resultPropertyRowColumns[2].ToMarkdown();

      var isArray = colValueType.StartsWith(ArrayTypeMarker);
      var typeName = isArray ? "array" : colValueType;
      var arrayTypeName = isArray ? colValueType[ArrayTypeMarker.Length..] : null;

      if (arrayTypeName?.Equals("objects", StringComparison.OrdinalIgnoreCase) ?? false)
        arrayTypeName = "object";

      if (description.Contains("milliseconds since the UNIX epoch"))
        typeName = TimestampDataType;

      var isComplex = typeName.Equals("object") || (arrayTypeName?.Equals("object") ?? false);

      return (resultPropertyRow, isComplex, colValueType, typeName, arrayTypeName, description, !GetDescriptionIndicatesOptionality(description));
    }

    private static ApiDocPropertyCollection ParseComplexType(HtmlNode resultNode, int level)
    {
      var props = new ApiDocPropertyCollection(10);
      var resultSiblings = new List<HtmlNode>();

      for (var row = resultNode.NextSibling; row is not null; row = row.NextSibling) {
        if (row.Name != "tr")
          continue;

        resultSiblings.Add(row);
      }

      var parentRowSiblingsCount = resultSiblings.Count;

      for (var i = 0; i < parentRowSiblingsCount; ++i) {
        var rowColumns = resultSiblings[i].SelectNodes("td");

        var colValueName = rowColumns[0].InnerText;
        var colValueType = rowColumns[1].InnerText;

        if (string.IsNullOrEmpty(colValueName))
          continue;

        if (colValueName.IndexOf(ObjectArrayParamMarker, StringComparison.OrdinalIgnoreCase) != -1) {
          var oldColValueName = colValueName;
          colValueName = colValueName.Replace(ObjectArrayParamMarker, string.Empty);
          var rowLevel = (oldColValueName.Length - colValueName.Length) / ObjectArrayParamMarker.Length;

          if (rowLevel < level)
            break;
        }

        var isArray = colValueType.StartsWith(ArrayTypeMarker);
        var typeName = isArray ? "array" : colValueType;
        var arrayTypeName = isArray ? colValueType[ArrayTypeMarker.Length..] : null;

        if (arrayTypeName?.Equals("objects", StringComparison.OrdinalIgnoreCase) ?? false)
          arrayTypeName = "object";

        var propName = colValueName;

        var prop = new ApiDocProperty
        {
          Name = propName,
          Description = rowColumns[2].ToMarkdown(),
          ApiDataType = colValueType,
          DataType = typeName,
          ArrayDataType = arrayTypeName
        };

        if (propName.Contains("timestamp"))
          ApplyTimestampFieldValues(prop);

        else if (prop.Description.Contains("milliseconds since the UNIX epoch"))
          ApplyTimestampFieldValues(prop);

        prop.Deprecated = GetDescriptionIndicatesDeprecated(prop.Description);
        prop.Required = !GetDescriptionIndicatesOptionality(prop.Description);

        if (typeName.Equals("object") || (arrayTypeName?.Equals("object") ?? false)) {
          var rowsConsumed = ParseComplexType(resultSiblings[i], level + 1, prop);
          i += rowsConsumed;
        }

        props.Add(propName, prop);
      }

      return props;
    }

    private static int ParseComplexType(HtmlNode siblingRow, int level, ApiDocProperty param)
    {
      var siblingRowSiblings = new List<HtmlNode>();

      for (var row = siblingRow.NextSibling; row is not null; row = row.NextSibling) {
        if (row.Name != "tr")
          continue;

        siblingRowSiblings.Add(row);
      }

      var parentRowSiblingsCount = siblingRowSiblings.Count;

      var props = new ApiDocPropertyCollection(parentRowSiblingsCount);

      var i = 0;

      for (; i < parentRowSiblingsCount; ++i) {
        var rowColumns = siblingRowSiblings[i].SelectNodes("td").ToArray();

        var colValueName = rowColumns[0].InnerText;
        var colValueType = rowColumns[1].InnerText;

        if (string.IsNullOrEmpty(colValueName))
          continue;

        if (colValueName.IndexOf(ObjectArrayParamMarker, StringComparison.OrdinalIgnoreCase) != -1) {
          var oldColValueName = colValueName;
          colValueName = colValueName.Replace(ObjectArrayParamMarker, string.Empty);
          var rowLevel = (oldColValueName.Length - colValueName.Length) / ObjectArrayParamMarker.Length;

          if (rowLevel < level)
            break;
        }

        var isArray = colValueType.StartsWith(ArrayTypeMarker);
        var typeName = isArray ? "array" : colValueType;
        var arrayTypeName = isArray ? colValueType[ArrayTypeMarker.Length..] : null;

        if (arrayTypeName?.Equals("objects", StringComparison.OrdinalIgnoreCase) ?? false)
          arrayTypeName = "object";

        var propName = colValueName;

        var prop = new ApiDocProperty
        {
          Name = propName,
          Description = rowColumns[2].ToMarkdown(),
          ApiDataType = colValueType,
          DataType = typeName,
          ArrayDataType = arrayTypeName
        };

        if (propName.Contains("timestamp"))
          ApplyTimestampFieldValues(prop);

        else if (prop.Description.Contains("milliseconds since the UNIX epoch"))
          ApplyTimestampFieldValues(prop);

        prop.Deprecated = GetDescriptionIndicatesDeprecated(prop.Description);
        prop.Required = !GetDescriptionIndicatesOptionality(prop.Description);

        if (typeName.Equals("object") || (arrayTypeName?.Equals("object") ?? false)) {
          var rowsConsumed = ParseComplexType(siblingRowSiblings[i], level + 1, prop);
          i += rowsConsumed;
        }

        props.Add(propName, prop);
      }

      param.Properties = props;
      return i;
    }

    private static (HtmlNode? resultPropertyRow, HtmlNode[]? resultPropertyRowColumns) FindRowWithName(IEnumerable<HtmlNode> tableRows, string name, HtmlNode? targetRow, HtmlNode[]? targetRowColumns)
    {
      foreach (var row in tableRows) {
        var rowColumns = row.SelectNodes("td");

        var colValueName = rowColumns[0].InnerText;

        if (string.IsNullOrWhiteSpace(colValueName) || !colValueName.Equals(name))
          continue;

        targetRow = row;
        targetRowColumns = rowColumns.ToArray();
        break;
      }

      return (targetRow, targetRowColumns);
    }

    private static void TryRepairTableIfNeeded(ref HtmlNode? resultPropertyRow, ref HtmlNode[]? resultPropertyRowColumns)
    {
      if (resultPropertyRow is null || resultPropertyRowColumns is null)
        return;

      if (resultPropertyRowColumns[1].InnerText.Equals("amount")) {
        var nameColumn = resultPropertyRow.OwnerDocument.CreateElement("td");
        var typeColumn = resultPropertyRow.OwnerDocument.CreateElement("td");
        var descriptionColumn = resultPropertyRow.OwnerDocument.CreateElement("td");

        nameColumn.InnerHtml = "result";
        typeColumn.InnerHtml = "array of object";

        var resultRow = resultPropertyRow.OwnerDocument.CreateElement("tr");
        resultRow.AppendChild(nameColumn);
        resultRow.AppendChild(typeColumn);
        resultRow.AppendChild(descriptionColumn);

        resultPropertyRow.ParentNode.InsertBefore(resultRow, resultPropertyRow);

        resultPropertyRowColumns[0].InnerHtml = resultPropertyRowColumns[1].InnerHtml;
        resultPropertyRowColumns[1].InnerHtml = resultPropertyRowColumns[2].InnerHtml;
        resultPropertyRowColumns[2].InnerHtml = "";

        resultPropertyRow = resultRow;
        resultPropertyRowColumns = resultRow.SelectNodes("td").ToArray();
      }
    }

    private static bool GetDescriptionIndicatesDeprecated(string? description)
    {
      if (string.IsNullOrEmpty(description))
        return false;

      Match m;
      var idxPattern = 0;

      do {
        m = DeprecatedParamPattern[idxPattern++].Match(description);
      } while (!m.Success && idxPattern < DeprecatedParamPattern.Length);

      return m.Success;
    }

    private static bool GetDescriptionIndicatesOptionality(string? description)
    {
      if (string.IsNullOrEmpty(description))
        return false;

      Match m;
      var idxPattern = 0;

      do {
        m = OptionalParamPattern[idxPattern++].Match(description);
      } while (!m.Success && idxPattern < OptionalParamPattern.Length);

      return m.Success;
    }
  }
}
