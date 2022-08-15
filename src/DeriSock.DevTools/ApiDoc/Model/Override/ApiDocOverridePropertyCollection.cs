namespace DeriSock.DevTools.ApiDoc.Model.Override;

using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

public class ApiDocOverridePropertyCollection : IDictionary<string, ApiDocOverrideProperty>
{
  public static readonly ApiDocOverridePropertyCollection Empty = new();

  private readonly IDictionary<string, ApiDocOverrideProperty> _properties;

  [UsedImplicitly]
  public ApiDocOverridePropertyCollection()
  {
    _properties = new Dictionary<string, ApiDocOverrideProperty>();
  }

#region Interface Implementations

  public IEnumerator<KeyValuePair<string, ApiDocOverrideProperty>> GetEnumerator()
    => _properties.GetEnumerator();

  IEnumerator IEnumerable.GetEnumerator()
    => ((IEnumerable)_properties).GetEnumerator();

  public void Add(KeyValuePair<string, ApiDocOverrideProperty> item)
  {
    _properties.Add(item);
  }

  public void Clear()
  {
    _properties.Clear();
  }

  public bool Contains(KeyValuePair<string, ApiDocOverrideProperty> item)
    => _properties.Contains(item);

  public void CopyTo(KeyValuePair<string, ApiDocOverrideProperty>[] array, int arrayIndex)
  {
    _properties.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<string, ApiDocOverrideProperty> item)
    => _properties.Remove(item);

  public int Count => _properties.Count;

  public bool IsReadOnly => _properties.IsReadOnly;

  public void Add(string key, ApiDocOverrideProperty value)
  {
    _properties.Add(key, value);
  }

  public bool ContainsKey(string key)
    => _properties.ContainsKey(key);

  public bool Remove(string key)
    => _properties.Remove(key);

  public bool TryGetValue(string key, out ApiDocOverrideProperty value)
    => _properties.TryGetValue(key, out value!);

  public ApiDocOverrideProperty this[string key]
  {
    get => _properties[key];
    set => _properties[key] = value;
  }

  public ICollection<string> Keys => _properties.Keys;

  public ICollection<ApiDocOverrideProperty> Values => _properties.Values;

#endregion
}
