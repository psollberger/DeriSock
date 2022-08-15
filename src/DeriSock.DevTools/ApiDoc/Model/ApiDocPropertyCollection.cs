namespace DeriSock.DevTools.ApiDoc.Model;

using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

public class ApiDocPropertyCollection : IDictionary<string, ApiDocProperty>
{
  public static readonly ApiDocPropertyCollection Empty = new();

  private readonly IDictionary<string, ApiDocProperty> _properties;

  [UsedImplicitly]
  public ApiDocPropertyCollection()
  {
    _properties = new Dictionary<string, ApiDocProperty>();
  }

  public ApiDocPropertyCollection(int capacity)
  {
    _properties = new Dictionary<string, ApiDocProperty>(capacity);
  }

#region Interface Implementations

  public IEnumerator<KeyValuePair<string, ApiDocProperty>> GetEnumerator()
    => _properties.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
    => ((IEnumerable)_properties).GetEnumerator();

  public void Add(KeyValuePair<string, ApiDocProperty> item)
  {
    _properties.Add(item);
  }

  public void Clear()
  {
    _properties.Clear();
  }

  public bool Contains(KeyValuePair<string, ApiDocProperty> item)
    => _properties.Contains(item);

  public void CopyTo(KeyValuePair<string, ApiDocProperty>[] array, int arrayIndex)
  {
    _properties.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<string, ApiDocProperty> item)
    => _properties.Remove(item);

  public int Count => _properties.Count;

  public bool IsReadOnly => _properties.IsReadOnly;

  public void Add(string key, ApiDocProperty value)
  {
    _properties.Add(key, value);
  }

  public bool ContainsKey(string key)
    => _properties.ContainsKey(key);

  public bool Remove(string key)
    => _properties.Remove(key);

  public bool TryGetValue(string key, out ApiDocProperty value)
    => _properties.TryGetValue(key, out value!);

  public ApiDocProperty this[string key]
  {
    get => _properties[key];
    set => _properties[key] = value;
  }

  public ICollection<string> Keys => _properties.Keys;

  public ICollection<ApiDocProperty> Values => _properties.Values;

#endregion
}
