namespace DeriSock.DevTools.ApiDoc.Model;

using System.Collections;
using System.Collections.Generic;

public class ApiDocObjectMap : IDictionary<string, ApiDocObjectMapEntry>
{
  private readonly IDictionary<string, ApiDocObjectMapEntry> _dict;

  public ApiDocObjectMap()
  {
    _dict = new Dictionary<string, ApiDocObjectMapEntry>();
  }

  public ApiDocObjectMap(int capacity)
  {
    _dict = new Dictionary<string, ApiDocObjectMapEntry>(capacity);
  }

#region Dictionary

  public IEnumerator<KeyValuePair<string, ApiDocObjectMapEntry>> GetEnumerator()
  {
    return _dict.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_dict).GetEnumerator();
  }

  public void Add(KeyValuePair<string, ApiDocObjectMapEntry> item)
  {
    _dict.Add(item);
  }

  public void Clear()
  {
    _dict.Clear();
  }

  public bool Contains(KeyValuePair<string, ApiDocObjectMapEntry> item)
  {
    return _dict.Contains(item);
  }

  public void CopyTo(KeyValuePair<string, ApiDocObjectMapEntry>[] array, int arrayIndex)
  {
    _dict.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<string, ApiDocObjectMapEntry> item)
  {
    return _dict.Remove(item);
  }

  public int Count => _dict.Count;

  public bool IsReadOnly => _dict.IsReadOnly;

  public void Add(string key, ApiDocObjectMapEntry value)
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

  public bool TryGetValue(string key, out ApiDocObjectMapEntry value)
  {
    return _dict.TryGetValue(key, out value!);
  }

  public ApiDocObjectMapEntry this[string key]
  {
    get => _dict[key];
    set => _dict[key] = value;
  }

  public ICollection<string> Keys => _dict.Keys;

  public ICollection<ApiDocObjectMapEntry> Values => _dict.Values;

#endregion
}
