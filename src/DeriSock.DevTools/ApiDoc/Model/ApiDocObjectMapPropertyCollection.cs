namespace DeriSock.DevTools.ApiDoc.Model;

using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

public class ApiDocObjectMapPropertyCollection : IDictionary<string, ApiDocObjectMapProperty>
{
  public static readonly ApiDocObjectMapPropertyCollection Empty = new();

  private readonly IDictionary<string, ApiDocObjectMapProperty> _properties;

  [UsedImplicitly]
  public ApiDocObjectMapPropertyCollection()
  {
    _properties = new Dictionary<string, ApiDocObjectMapProperty>();
  }

  public ApiDocObjectMapPropertyCollection(int capacity)
  {
    _properties = new Dictionary<string, ApiDocObjectMapProperty>(capacity);
  }

#region Interface Implementations

  public IEnumerator<KeyValuePair<string, ApiDocObjectMapProperty>> GetEnumerator()
    => _properties.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
    => ((IEnumerable)_properties).GetEnumerator();

  public void Add(KeyValuePair<string, ApiDocObjectMapProperty> item)
  {
    _properties.Add(item);
  }

  public void Clear()
  {
    _properties.Clear();
  }

  public bool Contains(KeyValuePair<string, ApiDocObjectMapProperty> item)
    => _properties.Contains(item);

  public void CopyTo(KeyValuePair<string, ApiDocObjectMapProperty>[] array, int arrayIndex)
  {
    _properties.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<string, ApiDocObjectMapProperty> item)
    => _properties.Remove(item);

  public int Count => _properties.Count;

  public bool IsReadOnly => _properties.IsReadOnly;

  public void Add(string key, ApiDocObjectMapProperty value)
  {
    _properties.Add(key, value);
  }

  public bool ContainsKey(string key)
    => _properties.ContainsKey(key);

  public bool Remove(string key)
    => _properties.Remove(key);

  public bool TryGetValue(string key, out ApiDocObjectMapProperty value)
    => _properties.TryGetValue(key, out value!);

  public ApiDocObjectMapProperty this[string key]
  {
    get => _properties[key];
    set => _properties[key] = value;
  }

  public ICollection<string> Keys => _properties.Keys;

  public ICollection<ApiDocObjectMapProperty> Values => _properties.Values;

#endregion
}
