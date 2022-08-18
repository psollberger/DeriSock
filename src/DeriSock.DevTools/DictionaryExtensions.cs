namespace DeriSock.DevTools;

using System.Collections.Generic;
using System.Linq;

internal static class DictionaryExtensions
{
  public static void RenameKey<TValue>(this IDictionary<string, TValue> dictionary, string oldKey, string newKey)
  {
    if (!dictionary.ContainsKey(oldKey))
      return;

    var arrKeys = dictionary.Keys.ToArray();
    var arrValues = dictionary.Values.ToArray();

    dictionary.Clear();

    for (var i = 0; i < arrKeys.Length; ++i) {
      if (arrKeys[i].Equals(oldKey))
        dictionary[newKey] = arrValues[i];
      else
        dictionary[arrKeys[i]] = arrValues[i];
    }
  }

  public static void InsertBefore<TValue>(this IDictionary<string, TValue> dictionary, string beforeKey, string key, TValue value)
  {
    var oldItems = dictionary.ToArray();

    dictionary.Clear();

    foreach (var (oldKey, oldValue) in oldItems) {
      if (oldKey == beforeKey) {
        dictionary.Add(key, value);
      }

      dictionary.Add(oldKey, oldValue);
    }
  }
}
