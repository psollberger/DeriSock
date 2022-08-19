namespace DeriSock.Converter;

using System.Collections.Generic;

using Newtonsoft.Json.Linq;

/// <inheritdoc />
public class ListJsonConverter<T> : IJsonConverter<List<T>>
{
  /// <inheritdoc />
  public List<T>? Convert(JToken? value)
  {
    if (value == null)
      return null;

    var list = (JArray)value;
    var result = new List<T>(list.Count);

    foreach (var item in list) {
      if (item is null)
        continue;

      result.Add(item.ToObject<T>()!);
    }

    return result;
  }
}
