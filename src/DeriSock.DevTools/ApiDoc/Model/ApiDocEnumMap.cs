namespace DeriSock.DevTools.ApiDoc.Model;

using System.Collections;
using System.Collections.Generic;

public class ApiDocEnumMap : IDictionary<string, ApiDocEnumMapEntry>
{
  private readonly IDictionary<string, ApiDocEnumMapEntry> _dict;

  public ApiDocEnumMap()
  {
    _dict = new Dictionary<string, ApiDocEnumMapEntry>();
  }

  public ApiDocEnumMap(int capacity)
  {
    _dict = new Dictionary<string, ApiDocEnumMapEntry>(capacity);
  }

#region Dictionary

  public IEnumerator<KeyValuePair<string, ApiDocEnumMapEntry>> GetEnumerator()
  {
    return _dict.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_dict).GetEnumerator();
  }

  public void Add(KeyValuePair<string, ApiDocEnumMapEntry> item)
  {
    _dict.Add(item);
  }

  public void Clear()
  {
    _dict.Clear();
  }

  public bool Contains(KeyValuePair<string, ApiDocEnumMapEntry> item)
  {
    return _dict.Contains(item);
  }

  public void CopyTo(KeyValuePair<string, ApiDocEnumMapEntry>[] array, int arrayIndex)
  {
    _dict.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<string, ApiDocEnumMapEntry> item)
  {
    return _dict.Remove(item);
  }

  public int Count => _dict.Count;

  public bool IsReadOnly => _dict.IsReadOnly;

  public void Add(string key, ApiDocEnumMapEntry value)
  {
    _dict.Add(key, value);
  }

  public bool ContainsKey(string key)
  {
    return _dict.ContainsKey(key);
  }

  public bool Remove(string key)
  {
    return _dict.Remove(key);
  }

  public bool TryGetValue(string key, out ApiDocEnumMapEntry value)
  {
    return _dict.TryGetValue(key, out value!);
  }

  public ApiDocEnumMapEntry this[string key]
  {
    get => _dict[key];
    set => _dict[key] = value;
  }

  public ICollection<string> Keys => _dict.Keys;

  public ICollection<ApiDocEnumMapEntry> Values => _dict.Values;

#endregion
}
