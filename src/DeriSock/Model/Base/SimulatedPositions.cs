namespace DeriSock.Model;

using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Object with positions in following form: <c>{InstrumentName1: Position1, InstrumentName2: Position2...}</c>, for example <c>{"BTC-PERPETUAL": -1000.0}</c> (or corresponding URI-encoding for GET). For futures in USD, for options in base currency.
/// </summary>
public class SimulatedPositions : IDictionary<string, decimal>
{
  private readonly IDictionary<string, decimal> _dict;

  /// <summary>
  /// Initializes a new instance of the <see cref="SimulatedPositions" /> class.
  /// </summary>
  public SimulatedPositions()
  {
    _dict = new Dictionary<string, decimal>();
  }

#region Dictionary

  /// <inheritdoc />
  public IEnumerator<KeyValuePair<string, decimal>> GetEnumerator()
  {
    return _dict.GetEnumerator();
  }

  IEnumerator IEnumerable.GetEnumerator()
  {
    return ((IEnumerable)_dict).GetEnumerator();
  }

  /// <inheritdoc />
  public void Add(KeyValuePair<string, decimal> item)
  {
    _dict.Add(item);
  }

  /// <inheritdoc />
  public void Clear()
  {
    _dict.Clear();
  }

  /// <inheritdoc />
  public bool Contains(KeyValuePair<string, decimal> item)
  {
    return _dict.Contains(item);
  }

  /// <inheritdoc />
  public void CopyTo(KeyValuePair<string, decimal>[] array, int arrayIndex)
  {
    _dict.CopyTo(array, arrayIndex);
  }

  /// <inheritdoc />
  public bool Remove(KeyValuePair<string, decimal> item)
  {
    return _dict.Remove(item);
  }

  /// <inheritdoc />
  public int Count => _dict.Count;

  /// <inheritdoc />
  public bool IsReadOnly => _dict.IsReadOnly;

  /// <inheritdoc />
  public void Add(string key, decimal value)
  {
    _dict.Add(key, value);
  }

  /// <inheritdoc />
  public bool ContainsKey(string key)
  {
    return _dict.ContainsKey(key);
  }

  /// <inheritdoc />
  public bool Remove(string key)
  {
    return _dict.Remove(key);
  }

  /// <inheritdoc />
  public bool TryGetValue(string key, out decimal value)
  {
    return _dict.TryGetValue(key, out value!);
  }

  /// <inheritdoc />
  public decimal this[string key]
  {
    get => _dict[key];
    set => _dict[key] = value;
  }

  /// <inheritdoc />
  public ICollection<string> Keys => _dict.Keys;

  /// <inheritdoc />
  public ICollection<decimal> Values => _dict.Values;

#endregion
}
