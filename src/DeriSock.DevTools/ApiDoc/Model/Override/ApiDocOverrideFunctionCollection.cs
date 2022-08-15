namespace DeriSock.DevTools.ApiDoc.Model.Override;

using System.Collections;
using System.Collections.Generic;

using JetBrains.Annotations;

public class ApiDocOverrideFunctionCollection : IDictionary<string, ApiDocOverrideFunction>
{
  private readonly IDictionary<string, ApiDocOverrideFunction> _dict;

  [UsedImplicitly]
  public ApiDocOverrideFunctionCollection()
  {
    _dict = new Dictionary<string, ApiDocOverrideFunction>();
  }
  
#region Dictionary

  public IEnumerator<KeyValuePair<string, ApiDocOverrideFunction>> GetEnumerator()
  {
    return _dict.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_dict).GetEnumerator();
  }

  public void Add(KeyValuePair<string, ApiDocOverrideFunction> item)
  {
    _dict.Add(item);
  }

  public void Clear()
  {
    _dict.Clear();
  }

  public bool Contains(KeyValuePair<string, ApiDocOverrideFunction> item)
  {
    return _dict.Contains(item);
  }

  public void CopyTo(KeyValuePair<string, ApiDocOverrideFunction>[] array, int arrayIndex)
  {
    _dict.CopyTo(array, arrayIndex);
  }

  public bool Remove(KeyValuePair<string, ApiDocOverrideFunction> item)
  {
    return _dict.Remove(item);
  }

  public int Count => _dict.Count;

  public bool IsReadOnly => _dict.IsReadOnly;

  public void Add(string key, ApiDocOverrideFunction value)
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

  public bool TryGetValue(string key, out ApiDocOverrideFunction value)
  {
    return _dict.TryGetValue(key, out value!);
  }

  public ApiDocOverrideFunction this[string key]
  {
    get => _dict[key];
    set => _dict[key] = value;
  }

  public ICollection<string> Keys => _dict.Keys;

  public ICollection<ApiDocOverrideFunction> Values => _dict.Values;

#endregion
}
